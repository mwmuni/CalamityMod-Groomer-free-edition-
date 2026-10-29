using CalamityMod.Items.Placeables.FurnitureSacrilegious;
using CalamityMod.Walls;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Walls
{
    public class OccultBrickWallItem : PlaceableWallItem<OccultBrickWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
            AddIngredient<OccultBrickItem>().
            AddTile(TileID.WorkBenches).
            Register();
        }
    }
}
