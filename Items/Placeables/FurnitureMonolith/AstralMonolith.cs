using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureMonolith
{
    public class AstralMonolith : PlaceableTileItem<Tiles.FurnitureMonolith.AstralMonolith>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Wood;
        }


        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            // It's not really wood... but it comes from trees!
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Wood;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralMonolithWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();

            CreateRecipe().
                AddIngredient<MonolithPlatform>(2).
                DisableDecraft().
                Register();
        }
    }
}
