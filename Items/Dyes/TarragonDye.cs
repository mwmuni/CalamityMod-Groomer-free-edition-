using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Dyes
{
    public class TarragonDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/TarragonDyeShader"), "DyePass").
            UseColor(new Color(20, 117, 70)).UseSecondaryColor(new Color(28, 255, 55)).UseImage(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Bark"));
        public override void SafeSetDefaults()
        {
            Item.rare = RarityType<Turquoise>();
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<UelibloomOre>(4).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
