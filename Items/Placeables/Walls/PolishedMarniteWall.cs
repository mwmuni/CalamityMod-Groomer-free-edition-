using CalamityMod.Items.Placeables.FurnitureMarnite;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class PolishedMarniteWall : PlaceableWallItem<WallTiles.PolishedMarniteWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<PolishedMarniteBlock>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
