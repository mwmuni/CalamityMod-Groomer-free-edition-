using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories;
using CalamityMod.Sounds;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public abstract class WulfrumNPCBase<TBanner> : ModNPC where TBanner : ModItem
    {
        protected abstract int FrameCount { get; }
        protected abstract string BestiaryEntryKey { get; }
        protected abstract int HitDustCount { get; }
        protected abstract int DeathDustCount { get; }
        protected abstract string GoreName { get; }
        protected virtual bool NeedsExpertScaling => false;
        protected virtual string AdditionalGoreName => string.Empty;
        protected virtual bool SpawnHitDustOnDedicatedServer => false;

        protected virtual void AdditionalHitEffects(NPC.HitInfo hit)
        {
        }

        public override void SetStaticDefaults()
        {
            if (NeedsExpertScaling)
                NPCID.Sets.NeedsExpertScaling[Type] = true;

            Main.npcFrameCount[Type] = FrameCount;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                SpriteDirection = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            AIType = -1;
            NPC.aiStyle = -1;
            NPC.HitSound = WulfrumAmplifier.Hit;
            NPC.DeathSound = CommonCalamitySounds.WulfrumNPCDeathSound;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<TBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement(BestiaryEntryKey)
            });
        }

        protected bool CanSpawnInWulfrumZones(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur ||
                (!spawnInfo.Player.ZoneOverworldHeight && !Main.remixWorld) ||
                (!spawnInfo.Player.ZoneNormalCaverns && spawnInfo.Player.ZoneGlowshroom && Main.remixWorld))
            {
                return false;
            }

            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (!Main.dedServ || SpawnHitDustOnDedicatedServer)
            {
                for (int k = 0; k < HitDustCount; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GrassBlades, hit.HitDirection, -1f, 0, default, 1f);
                }

                if (NPC.life <= 0)
                {
                    for (int k = 0; k < DeathDustCount; k++)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GrassBlades, hit.HitDirection, -1f, 0, default, 1f);
                    }
                }
            }

            if (NPC.life <= 0 && !Main.dedServ)
            {
                SpawnGore(GoreName);
                if (AdditionalGoreName.Length > 0)
                    SpawnGore(AdditionalGoreName);

                int randomGoreCount = Main.rand.Next(1, 4);
                for (int i = 0; i < randomGoreCount; i++)
                {
                    SpawnGore("WulfrumEnemyGore" + Main.rand.Next(1, 11).ToString());
                }
            }

            AdditionalHitEffects(hit);
        }

        protected void SpawnGore(string goreName)
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(goreName).Type, 1f);
        }
    }

    public abstract class WulfrumCombatNPCBase<TBanner> : WulfrumNPCBase<TBanner> where TBanner : ModItem
    {
        public float SuperchargeTimer
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        public bool Supercharged => SuperchargeTimer > 0;

        protected virtual bool BonusFrameInZenith => false;

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (Main.zenithWorld && BonusFrameInZenith)
                NPC.frameCounter++;

            int frame = (int)(NPC.frameCounter / 5) % (Main.npcFrameCount[Type] / 2);
            if (Supercharged)
                frame += Main.npcFrameCount[Type] / 2;

            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!CanSpawnInWulfrumZones(spawnInfo))
                return 0f;

            return (Main.remixWorld ? SpawnCondition.Cavern.Chance : SpawnCondition.OverworldDaySlime.Chance) *
                (Main.hardMode ? 0.055f : 0.135f) *
                (NPC.AnyNPCs(ModContent.NPCType<WulfrumAmplifier>()) ? 5.5f : 1f);
        }

        protected void AddCombatLoot(NPCLoot npcLoot, int minimumScrap, int maximumScrap, int additionalItemType = 0)
        {
            npcLoot.Add(ModContent.ItemType<WulfrumMetalScrap>(), 1, minimumScrap, maximumScrap);
            if (additionalItemType != 0)
                npcLoot.Add(additionalItemType, 10);
            npcLoot.Add(ModContent.ItemType<WulfrumBattery>(), new Fraction(7, 100));
            npcLoot.AddIf(info => info.npc.ai[3] > 0f, ModContent.ItemType<EnergyCore>());
        }
    }
}
