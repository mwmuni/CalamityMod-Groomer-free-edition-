using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Dyes
{
    public class DaedalusDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/DaedalusDyeShader"), "DyePass").
            UseColor(new Color(59, 183, 255)).UseSecondaryColor(new Color(181, 45, 196)).UseImage(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/VoronoiShapes3"));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<CryonicOre>(4).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
