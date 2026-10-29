using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.Abyss
{
    public class PyreMantleMolten : PlaceableTileItem<Tiles.Abyss.PyreMantleMolten>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddIngredient(ItemID.LavaBucket).
                AddIngredient<PyreMantle>(25).
                AddTile(TileID.Furnaces).
                Register();
        }
    }
}
