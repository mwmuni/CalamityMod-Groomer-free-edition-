using CalamityMod.Items.Placeables.FurniturePlagued;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class PlaguedPlateWall : PlaceableWallItem<WallTiles.PlaguedPlateWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<PlaguedContainmentBrick>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
