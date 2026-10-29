using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureSilva
{
    public class SilvaCrystal : PlaceableTileItem<Tiles.FurnitureSilva.SilvaCrystal>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(200).
                AddIngredient(ItemID.Glass, 200).
                AddIngredient<PlantyMush>(4).
                AddIngredient<EffulgentFeather>().
                AddTile<CosmicAnvil>().
                Register();
            CreateRecipe().
                AddIngredient<SilvaWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<SilvaPlatform>(2).
                DisableDecraft().
                Register();
        }
    }
}
