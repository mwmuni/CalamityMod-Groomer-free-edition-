using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles.FurnitureAcidwood;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureAcidwood
{
    public class Acidwood : PlaceableTileItem<AcidwoodTile>
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
                AddIngredient<AcidwoodPlatform>(2).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<AcidwoodWallItem>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
