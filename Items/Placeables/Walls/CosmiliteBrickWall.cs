using CalamityMod.Items.Placeables.FurnitureCosmilite;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class CosmiliteBrickWall : PlaceableWallItem<WallTiles.CosmiliteBrickWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<CosmiliteBrick>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
