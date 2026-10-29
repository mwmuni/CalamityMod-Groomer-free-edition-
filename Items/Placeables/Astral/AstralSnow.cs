using CalamityMod.Items.Placeables.Walls;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.Astral
{
    public class AstralSnow : PlaceableTileItem<Tiles.AstralSnow.AstralSnow>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 1, ItemID.SnowBlock, 1);
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralSnowWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
