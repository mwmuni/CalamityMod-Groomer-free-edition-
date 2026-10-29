using CalamityMod.Items.Placeables.Abyss;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class AbyssGravelWallItem : PlaceableWallItem<WallTiles.SafeAbyssGravelWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<AbyssGravel>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
