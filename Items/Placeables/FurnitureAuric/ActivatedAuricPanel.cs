using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureAuric;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureAuric
{
    public class ActivatedAuricPanel : PlaceableTileItem<ActivatedAuricPanelTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void AddRecipes()
        {
            CreateRecipe(400).
                AddRecipeGroup("AnyStoneBlock", 400).
                AddIngredient<AuricOre>().
                AddTile<CosmicAnvil>().
                Register();
            /*CreateRecipe().
                AddIngredient<ActivatedAuricPanelWallItem>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();*/
        }
    }
}
