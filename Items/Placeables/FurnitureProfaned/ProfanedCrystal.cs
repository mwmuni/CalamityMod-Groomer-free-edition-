using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class ProfanedCrystal : PlaceableTileItem<Tiles.FurnitureProfaned.ProfanedCrystal>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddIngredient(ItemID.Glass, 50).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.AdamantiteForge).
                Register();
            CreateRecipe().
                AddIngredient<ProfanedCrystalWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
