using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Dyes
{
    public class AcidRainDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/AcidRainDyeShader"), "DyePass").
            UseColor(new Color(20, 117, 70)).UseSecondaryColor(new Color(208, 254, 39)).UseImage(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Bark"));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<SulphuricScale>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
