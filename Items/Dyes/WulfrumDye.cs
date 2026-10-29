using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Dyes
{
    public class WulfrumDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/WulfrumDyeShader"), "DyePass").
            UseColor(new Color(89, 247, 166)).UseSecondaryColor(new Color(102, 242, 255)).UseImage(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise"));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<WulfrumMetalScrap>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
