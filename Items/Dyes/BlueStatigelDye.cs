using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    public class BlueStatigelDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/SlimeGodDyeShader"), "DyePass").
            UseColor(new Color(80, 170, 206)).UseSecondaryColor(new Color(81, 87, 119)).UseImage("Images/Misc/Perlin");
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<PurifiedGel>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
