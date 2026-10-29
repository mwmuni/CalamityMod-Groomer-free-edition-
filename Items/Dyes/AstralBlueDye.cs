using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    public class AstralBlueDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/AstralBlueDyeShader"), "DyePass").
            UseColor(new Color(109, 242, 197)).UseSecondaryColor(new Color(42, 147, 154));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<AstralOre>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
