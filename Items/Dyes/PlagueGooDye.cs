using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Dyes
{
    public class PlagueGooDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/PlagueGooDyeShader"), "DyePass").
            UseColor(new Color(26, 288, 0)).UseImage(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons"));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<PlagueCellCanister>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
