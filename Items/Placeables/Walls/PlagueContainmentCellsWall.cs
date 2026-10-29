using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class PlagueContainmentCellsWall : PlaceableWallItem<WallTiles.PlagueContainmentCellsWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<Plates.Plagueplate>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
