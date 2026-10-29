using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class OnyxplateWall : PlaceableWallItem<WallTiles.OnyxplateWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<Plates.Onyxplate>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
