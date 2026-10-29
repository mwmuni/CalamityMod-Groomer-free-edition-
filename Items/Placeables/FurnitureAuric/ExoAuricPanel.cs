using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureAuric;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureAuric
{
    public class ExoAuricPanel : PlaceableTileItem<ExoAuricPanelTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void AddRecipes()
        {
            CreateRecipe(400).
                AddIngredient<AuricPanel>(400).
                AddIngredient<ExoPrism>().
                AddTile<DraedonsForge>().
                Register();
            /*CreateRecipe().
                AddIngredient<ExoAuricPanelWallItem>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();*/
        }
    }
}
