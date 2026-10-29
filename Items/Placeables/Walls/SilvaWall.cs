using CalamityMod.Items.Placeables.FurnitureSilva;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class SilvaWall : PlaceableWallItem<WallTiles.SilvaWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<SilvaCrystal>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
