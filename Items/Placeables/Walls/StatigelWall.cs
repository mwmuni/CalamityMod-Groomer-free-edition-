using CalamityMod.Items.Placeables.FurnitureStatigel;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class StatigelWall : PlaceableWallItem<WallTiles.StatigelWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<StatigelBlock>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
