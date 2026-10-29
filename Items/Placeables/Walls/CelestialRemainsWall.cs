using CalamityMod.Items.Placeables.Astral;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;

namespace CalamityMod.Items.Placeables.Walls
{
    [LegacyName("AstralFossilWall")]
    public class CelestialRemainsWall : PlaceableWallItem<WallTiles.CelestialRemainsWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }


        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<CelestialRemains>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
