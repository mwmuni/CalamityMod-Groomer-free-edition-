using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Dyes
{
    public class MeldDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/MeldDyeShader"), "DyePass").
            UseColor(new Color(28, 135, 84)).UseSecondaryColor(new Color(52, 235, 149)).UseImage(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Veins"));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
        }

        public override void AddRecipes()
        {
            // Vanilla frag dye amount
            CreateRecipe(1).
                AddIngredient(ItemID.BottledWater, 1).
                AddIngredient<MeldBlob>(10).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
