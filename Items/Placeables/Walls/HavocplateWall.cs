using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    [LegacyName("ChaosplateWall")]
    public class HavocplateWall : PlaceableWallItem<WallTiles.HavocplateWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<Plates.Havocplate>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
