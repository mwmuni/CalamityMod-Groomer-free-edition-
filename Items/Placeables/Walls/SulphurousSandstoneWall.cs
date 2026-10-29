using CalamityMod.Items.Placeables.Abyss;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class SulphurousSandstoneWall : PlaceableWallItem<WallTiles.SafeSulphurousSandstoneWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<SulphurousSandstone>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
