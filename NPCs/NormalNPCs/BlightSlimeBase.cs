using CalamityMod.Items.Materials;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public abstract class BlightSlimeBase<TBanner> : ModNPC where TBanner : ModItem
    {
        protected abstract IBestiaryInfoElement BiomeTag { get; }
        protected abstract float SpawnConditionChance { get; }
        protected abstract int HitDustType { get; }
        protected abstract int HitDustCount { get; }
        protected abstract int PlayerDebuffType { get; }
        protected abstract int RareDropItemType { get; }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.Slime;
            AIType = NPCID.DungeonSlime;
            NPC.damage = 30;
            NPC.width = 60;
            NPC.height = 42;
            NPC.defense = 8;
            NPC.lifeMax = 130;
            NPC.knockBackResist = 0.3f;
            AnimationType = NPCID.RainbowSlime;
            NPC.value = Item.buyPrice(silver: 1);
            NPC.alpha = 105;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<TBanner>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BiomeTag,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.BlightSlime")
            });
        }

        public override void AI()
        {
            NPC.damage = (NPC.velocity.Y == 0f || NPC.velocity.Length() < 3f) ? 0 : NPC.defDamage;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneAbyss)
            {
                return 0f;
            }

            return SpawnConditionChance * 0.15f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < HitDustCount; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, HitDustType, hit.HitDirection, -1f, 0, default, 1f);
            }

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, HitDustType, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(PlayerDebuffType, 240, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.Gel, 1, 10, 14);
            npcLoot.Add(ItemDropRule.NormalvsExpert(RareDropItemType, 100, 50));
            npcLoot.Add(ModContent.ItemType<BlightedGel>(), 1, 15, 21);
        }
    }
}
