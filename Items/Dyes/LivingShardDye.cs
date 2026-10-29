using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Dyes
{
    public class LivingShardDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/LivingShardDyeShader"), "DyePass").
            UseColor(new Color(118, 230, 29)).UseSecondaryColor(new Color(102, 209, 255)).UseImage(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/OrnatePattern"));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<LivingShard>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
