using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureAuric;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureAuric
{
    public class AuricRepulserPanel : PlaceableTileItem<AuricRepulserPanelTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void AddRecipes()
        {
            CreateRecipe(400).
                AddIngredient<ActivatedAuricPanel>(400).
                AddIngredient<AscendantSpiritEssence>().
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
