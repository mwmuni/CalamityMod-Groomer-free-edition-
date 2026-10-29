using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Dyes
{
    public class ScoriaDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/ScoriaDyeShader"), "DyePass").
            UseColor(new Color(76, 77, 86)).UseSecondaryColor(new Color(224, 108, 29)).UseImage(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Veins"));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<ScoriaOre>(4).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
