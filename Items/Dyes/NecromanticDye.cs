using CalamityMod.Items.TreasureBags.MiscGrabBags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Dyes
{
    public class NecromanticDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/NecromanticDyeShader"), "DyePass").
            UseColor(new Color(71, 23, 26)).UseSecondaryColor(new Color(10, 198, 255)).UseImage(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/SharpNoise"));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<FleshyGeode>().
                AddTile(TileID.DyeVat).
                Register();
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<NecromanticGeode>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
