using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class AshenAccentSlab : PlaceableTileItem<Tiles.FurnitureAshen.AshenAccentSlab>
    {
        public override string Texture => "CalamityMod/Items/Placeables/FurnitureAshen/AshenSlab";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddIngredient<SmoothBrimstoneSlag>(50).
                AddTile<AshenAltar>().
                AddCondition(Condition.InGraveyard).
                Register();
        }
    }
}
