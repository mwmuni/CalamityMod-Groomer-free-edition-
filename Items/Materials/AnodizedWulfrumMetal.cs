using CalamityMod.Items.Placeables.FurnitureWulfrum;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class AnodizedWulfrumMetal : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 18;
        protected override int MaterialHeight => 18;
        protected override int SellValue => Item.sellPrice(copper: 10);
        protected override int MaterialRarity => ItemRarityID.Blue;

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient<StormlionMandible>().
                AddIngredient<WulfrumMetalScrap>().
                AddTile(TileID.WorkBenches).
                Register();

            CreateRecipe().
                AddIngredient<AnodizedWulfrumPlatform>(2).
                DisableDecraft().
                Register();
        }
        //Added to avoid making the recipes really repetitive and nonsensical. It also makes the new metal color make sense. Adds a bit of flavor to the Wulfrum Recipes
        //Uses Stormlion Manible because in order to anodize a metal you must use a current, the other option was a Power Cell but I feel Stormlion Mandible needed more uses
        //More uses are planned for other blocks and furniture for wulfrum that use this new metal color and gives a bit more depth to the set
    }
}
