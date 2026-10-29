using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.Abyss
{
    public class SulphurousSandstone : PlaceableTileItem<Tiles.Abyss.SulphurousSandstone>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SulphurousSand>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Walls.SulphurousSandstoneWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
