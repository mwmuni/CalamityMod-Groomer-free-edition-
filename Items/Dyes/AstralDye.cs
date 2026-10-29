using CalamityMod.Items.Placeables.Astral;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    public class AstralDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/AstralDyeShader"), "DyePass").
            UseColor(new Color(123, 99, 130)).UseSecondaryColor(new Color(189, 66, 84)).UseImage("Images/Misc/Perlin");
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<AstralStone>(15).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
