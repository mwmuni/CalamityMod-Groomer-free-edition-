using CalamityMod.Items.Placeables.SunkenSea;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureBasalt
{
    public class BasaltSlab : PlaceableTileItem<Tiles.FurnitureBasalt.BasaltSlab>
    {

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Basalt>(1).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }

    }
}
