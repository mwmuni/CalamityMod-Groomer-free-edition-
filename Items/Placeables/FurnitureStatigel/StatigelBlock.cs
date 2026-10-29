using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureStatigel
{
    public class StatigelBlock : PlaceableTileItem<Tiles.FurnitureStatigel.StatigelBlock>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddIngredient<PurifiedGel>().
                AddTile(TileID.Solidifier).
                Register();
            CreateRecipe().
                AddIngredient<StatigelPlatform>(2).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<StatigelWall>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
