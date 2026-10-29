using CalamityMod.Items.Placeables.Crags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    public class BrimflameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/BrimflameDyeShader"), "DyePass").
            UseColor(new Color(252, 147, 34)).UseSecondaryColor(new Color(216, 41, 26)).UseImage("Images/Misc/Perlin");
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<BrimstoneSlag>(3).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
