using CalamityMod.Items.Placeables.FurnitureAcidwood;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class AcidwoodWallItem : PlaceableWallItem<WallTiles.AcidwoodWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
            AddIngredient<Acidwood>().
            AddTile(TileID.WorkBenches).
            Register();
        }
    }
}
