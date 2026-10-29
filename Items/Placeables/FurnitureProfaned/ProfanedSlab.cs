using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class ProfanedSlab : PlaceableTileItem<Tiles.FurnitureProfaned.ProfanedSlab>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient<ProfanedRock>(5).
                AddTile(TileID.AdamantiteForge).
                Register();
            CreateRecipe().
                AddIngredient<ProfanedSlabWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
