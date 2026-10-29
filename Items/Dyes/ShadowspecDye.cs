using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class ShadowspecDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/ShadowspecDyeShader"), "DyePass").
            UseColor(new Color(46, 27, 60)).UseSecondaryColor(new Color(132, 142, 191)).UseImage("Images/Misc/Perlin");
        public override void SafeSetDefaults()
        {
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 2, silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.BottledWater, 10).
                AddIngredient<ShadowspecBar>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
