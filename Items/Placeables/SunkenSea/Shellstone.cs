using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.SunkenSea
{
    public class Shellstone : PlaceableTileItem<Tiles.SunkenSea.Shellstone>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AridSoil>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ShellstoneWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }

    }
}
