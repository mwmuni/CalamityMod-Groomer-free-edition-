using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureOtherworldly
{
    [LegacyName("OccultStone")]
    public class OtherworldlyStone : PlaceableTileItem<Tiles.FurnitureOtherworldly.OtherworldlyStone>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(200).
                AddRecipeGroup("AnyStoneBlock", 200).
                AddIngredient<DarkPlasma>().
                AddIngredient<ArmoredShell>().
                AddIngredient<TwistingNether>().
                AddIngredient(ItemID.Silk, 10).
                AddTile(TileID.AdamantiteForge).
                Register();
            CreateRecipe().
                AddIngredient<OtherworldlyStoneWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<OtherworldlyPlatform>(2).
                DisableDecraft().
                Register();
        }
    }
}
