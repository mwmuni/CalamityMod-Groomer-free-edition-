using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureAuric;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureAuric
{
    public class AuricAbsorber : PlaceableTileItem<AuricAbsorberTile>
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
                AddIngredient<EndothermicEnergy>().
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
