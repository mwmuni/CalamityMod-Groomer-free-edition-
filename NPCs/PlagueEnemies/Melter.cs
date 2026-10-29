using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class Melter : PlagueEnemyBase
    {
        protected override string BestiaryKey => "Mods.CalamityMod.Bestiary.Melter";

        protected override string GoreName => nameof(Melter);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers();
            value.PortraitPositionYOverride = -32f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 0.5f;
            NPC.aiStyle = NPCAIStyleID.Bat;
            AIType = NPCID.GiantFlyingFox;
            AnimationType = NPCID.GiantFlyingFox;
            NPC.damage = 55;
            NPC.width = 38;
            NPC.height = 34;
            NPC.defense = 20;
            NPC.lifeMax = 800;
            NPC.knockBackResist = 0.3f;
            NPC.value = Item.buyPrice(silver: 10);
            NPC.HitSound = SoundID.NPCHit1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<MelterBanner>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<PlagueCellCanister>(), 1, 1, 2);
    }
}
