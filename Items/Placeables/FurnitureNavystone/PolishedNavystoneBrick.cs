using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureNavystone
{
    public class PolishedNavystoneBrick : PlaceableTileItem<Tiles.FurnitureNavystone.PolishedNavystoneBrick>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Navystone>(2).
                AddTile(TileID.Furnaces).
                Register();
        }
    }
}
