using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureStratus
{
    public class StratusBricks : PlaceableTileItem<Tiles.FurnitureStratus.StratusBricks>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(200).
                AddRecipeGroup("AnyStoneBlock", 200).
                AddIngredient<Lumenyl>(3).
                AddIngredient<RuinousSoul>().
                AddIngredient<ExodiumCluster>().
                AddTile<Tiles.Furniture.CraftingStations.VoidCondenser>().
                Register();
            CreateRecipe().
                AddIngredient<StratusWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<StratusPlatform>(2).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<StratusStarPlatformItem>(2).
                DisableDecraft().
                Register();
        }
    }
}
