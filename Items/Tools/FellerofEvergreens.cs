using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class FellerofEvergreens : BaseMeleeTool
    {
        public override void SetDefaults()
        {
            SetMeleeToolDefaults(
                66, 66, 18, 5f, 17, 25, CalamityGlobalItem.RarityGreenBuyPrice, ItemRarityID.Green,
                axePower: 100 / 5);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnySilverBar", 18).
                AddIngredient(ItemID.RichMahogany, 18).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
