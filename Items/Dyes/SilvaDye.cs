using CalamityMod.Items.Placeables.FurnitureSilva;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    public class SilvaDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/SilvaDyeShader"), "DyePass").
            UseColor(new Color(103, 206, 103)).UseSecondaryColor(new Color(255, 255, 158));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<SilvaCrystal>(15).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
