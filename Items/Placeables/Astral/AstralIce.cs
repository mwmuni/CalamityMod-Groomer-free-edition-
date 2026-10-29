using CalamityMod.Items.Placeables.Walls;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.Astral
{
    public class AstralIce : PlaceableTileItem<Tiles.AstralSnow.AstralIce>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 1, ItemID.IceBlock, 1);
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AstralSnow>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralIceWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
