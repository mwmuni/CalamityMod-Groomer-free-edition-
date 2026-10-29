using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureAbyss
{
    public class SmoothAbyssGravel : PlaceableTileItem<Tiles.FurnitureAbyss.SmoothAbyssGravel>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AbyssGravel>().
                AddTile(TileID.WorkBenches).
                Register();
            CreateRecipe().
                AddIngredient<SmoothAbyssGravelWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<SmoothAbyssGravelPlatform>(2).
                DisableDecraft().
                Register();
        }
    }
}
