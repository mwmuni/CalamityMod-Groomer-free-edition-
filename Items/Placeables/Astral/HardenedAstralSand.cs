using CalamityMod.Items.Placeables.Walls;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.Astral
{
    public class HardenedAstralSand : PlaceableTileItem<Tiles.AstralDesert.HardenedAstralSand>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 1, ItemID.HardenedSand, 1);
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AstralSand>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HardenedAstralSandWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
