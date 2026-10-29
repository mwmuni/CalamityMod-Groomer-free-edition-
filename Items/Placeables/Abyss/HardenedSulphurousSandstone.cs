using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.Abyss
{
    public class HardenedSulphurousSandstone : PlaceableTileItem<Tiles.Abyss.HardenedSulphurousSandstone>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SulphurousSand>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Walls.HardenedSulphurousSandstoneWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
