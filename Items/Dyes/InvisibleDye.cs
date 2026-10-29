using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class InvisibleDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/InvisibleDyeShader"), "DyePass");

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient(ItemID.Lens).
                AddTile(TileID.DyeVat).
                Register();
        }

        #region On Edit Hell To Actually Make Player Body Not Draw
        public override void Load()
        {
            On_PlayerDrawLayers.DrawPlayer_21_Head += StopHeadDrawing;
            On_PlayerDrawLayers.DrawPlayer_12_Skin += StopBodyAndLegDrawing;
            On_PlayerDrawLayers.DrawPlayer_13_Leggings += StopLegClothesDrawing;
            On_PlayerDrawLayers.DrawPlayer_17_Torso += StopBodyClothesDrawing;
            On_PlayerDrawLayers.DrawPlayer_12_SkinComposite_BackArmShirt += StopCompositeArmDrawing;
            On_PlayerDrawLayers.DrawPlayer_28_ArmOverItem += StopBackArmAndUndershirtDrawing;
        }

        private static void StopHeadDrawing(On_PlayerDrawLayers.orig_DrawPlayer_21_Head orig, ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.cHead == GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<InvisibleDye>()))
                return;
            orig(ref drawInfo);
        }
        private static void StopBodyAndLegDrawing(On_PlayerDrawLayers.orig_DrawPlayer_12_Skin orig, ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.cBody == GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<InvisibleDye>()))
                drawInfo.hidesTopSkin = true;
            if (drawInfo.drawPlayer.cLegs == GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<InvisibleDye>()))
                drawInfo.hidesBottomSkin = true;
            orig(ref drawInfo);
        }
        private static void StopLegClothesDrawing(On_PlayerDrawLayers.orig_DrawPlayer_13_Leggings orig, ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.cLegs == GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<InvisibleDye>()))
                return;
            orig(ref drawInfo);
        }
        private static void StopBodyClothesDrawing(On_PlayerDrawLayers.orig_DrawPlayer_17_Torso orig, ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.cBody == GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<InvisibleDye>()))
                return;
            orig(ref drawInfo);
        }
        private static void StopCompositeArmDrawing(On_PlayerDrawLayers.orig_DrawPlayer_12_SkinComposite_BackArmShirt orig, ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.cBody == GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<InvisibleDye>()))
                return;
            orig(ref drawInfo);
        }
        private static void StopBackArmAndUndershirtDrawing(On_PlayerDrawLayers.orig_DrawPlayer_28_ArmOverItem orig, ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.cBody == GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<InvisibleDye>()))
                return;
            orig(ref drawInfo);
        }
        #endregion
    }
}
