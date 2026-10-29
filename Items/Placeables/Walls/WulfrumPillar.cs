using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class WulfrumPillar : PlaceableWallItem<WallTiles.WulfrumPillar>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<WulfrumMetalScrap>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
