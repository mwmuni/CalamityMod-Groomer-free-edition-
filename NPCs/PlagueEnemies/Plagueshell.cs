using System;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class Plagueshell : PlagueEnemyBase
    {
        protected override string BestiaryKey => "Mods.CalamityMod.Bestiary.Plagueshell";

        protected override string GoreName => nameof(Plagueshell);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 2f;
            NPC.damage = 80;
            NPC.aiStyle = NPCAIStyleID.GiantTortoise;
            NPC.width = 46;
            NPC.height = 32;
            NPC.defense = 50;
            NPC.lifeMax = 1200;
            NPC.knockBackResist = 0.2f;
            AnimationType = NPCID.GiantTortoise;
            NPC.value = Item.buyPrice(silver: 15);
            NPC.HitSound = SoundID.NPCHit24;
            NPC.noGravity = false;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<PlagueshellBanner>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            NPC.damage = NPC.ai[0] == 3f ? (int)Math.Round(NPC.defDamage * 1.4) : 0;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<PlagueCellCanister>(), 1, 3, 4);
    }
}
