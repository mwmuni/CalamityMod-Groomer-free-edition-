using CalamityMod.Items.Placeables.Walls;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.Astral
{
    public class AstralDirt : PlaceableTileItem<Tiles.Astral.AstralDirt>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 1, ItemID.DirtBlock, 1);
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralDirtWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
