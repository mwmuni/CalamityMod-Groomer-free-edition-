using CalamityMod.Items.Placeables.FurnitureNavystone.FurnitureAncientNavystone;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class AncientSmoothNavystoneWall : PlaceableWallItem<WallTiles.AncientSmoothNavystoneWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<AncientSmoothNavystone>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
