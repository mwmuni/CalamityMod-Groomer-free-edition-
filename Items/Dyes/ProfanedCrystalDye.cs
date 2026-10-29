using CalamityMod.Items.Placeables.FurnitureProfaned;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    public class ProfanedCrystalDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/ProfanedCrystalDyeShader"), "DyePass").
            UseColor(new Color(255, 0, 89));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<ProfanedCrystal>(15).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
