using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class RunicProfanedBrick : PlaceableTileItem<Tiles.FurnitureProfaned.RunicProfanedBrick>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient<ProfanedRock>(4).
                AddIngredient<ProfanedCrystal>().
                AddTile(TileID.AdamantiteForge).
                Register();
            CreateRecipe().
                AddIngredient<RunicProfanedBrickWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
