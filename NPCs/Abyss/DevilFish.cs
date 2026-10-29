using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class DevilFish : DevilFishBase
    {
        public static readonly SoundStyle MaskBreakSound = new("CalamityMod/Sounds/Custom/DevilMaskBreak");
        public static Asset<Texture2D> GlowTexture;

        protected override int BannerType => NPC.type;
        protected override Asset<Texture2D> GlowTextureAsset => GlowTexture;
        protected override string GetMaskGoreName(int index) => "DevilFishMask" + index;
        protected override string GetDeathGoreName(int index) => index == 1 ? "Devilfish" : "Devilfish" + index;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitPositionXOverride = 5f
            };
            value.Position.X += 30f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DevilFish")
            });
        }

        public static void DefineDevilFishLoot(NPCLoot npcLoot)
        {
            var postLevi = npcLoot.DefineConditionalDropSet(DropHelper.PostLevi());
            postLevi.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<DepthCells>(), 2, 1, 2, 2, 3));
            npcLoot.AddIf(() => NPC.downedGolemBoss, ModContent.ItemType<ScoriaOre>(), 1, 3, 9);
            npcLoot.Add(ModContent.ItemType<PyreMantle>(), 1, 10, 20);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => DefineDevilFishLoot(npcLoot);
    }
}
