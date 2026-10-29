using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Crags
{
    public class Scryllar : ScryllarBase
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitPositionYOverride = 10f
            };
            value.Position.Y += 20f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 50;
            NPC.width = 80;
            NPC.height = 80;
            NPC.defense = 18;
            NPC.lifeMax = 160;
            NPC.alpha = 100;
            NPC.knockBackResist = 0.7f;
            NPC.value = Item.buyPrice(silver: 5);
            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            if (DownedBossSystem.downedProvidence)
            {
                NPC.damage = 90;
                NPC.defense = 30;
                NPC.lifeMax = 2500;
            }
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ScryllarBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BrimstoneCragsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Scryllar")
            });
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.Calamity().ZoneCalamity ? 0.25f : 0f;
        }

        public static void DefineScryllarLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule hardmode = npcLoot.DefineConditionalDropSet(DropHelper.Hardmode());
            LeadingConditionRule postProv = npcLoot.DefineConditionalDropSet(DropHelper.PostProv());
            hardmode.Add(ModContent.ItemType<EssenceofHavoc>(), 2);
            postProv.Add(ModContent.ItemType<Bloodstone>(), 4);
            postProv.Add(ModContent.ItemType<GuidelightofOblivion>(), 20);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => DefineScryllarLoot(npcLoot);

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Scryllar").Type, NPC.scale);
                }
            }
        }
    }
}
