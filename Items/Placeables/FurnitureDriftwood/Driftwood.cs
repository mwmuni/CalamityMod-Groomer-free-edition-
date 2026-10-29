using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureDriftwood
{
    public class Driftwood : PlaceableTileItem<Tiles.FurnitureDriftwood.Driftwood>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Wood;
        }


        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Wood;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DriftwoodPlatform>(2).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<Walls.DriftwoodWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
