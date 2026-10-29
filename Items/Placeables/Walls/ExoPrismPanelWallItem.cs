using CalamityMod.Items.Placeables.FurnitureExo;
using CalamityMod.Walls;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Walls
{
    public class ExoPrismPanelWallItem : PlaceableWallItem<ExoPrismPanelWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<ExoPrismPanel>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
