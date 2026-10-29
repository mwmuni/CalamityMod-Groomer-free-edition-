using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.Crags
{
    public class ScorchedBone : PlaceableTileItem<Tiles.Crags.ScorchedBone>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Wood;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient<ScorchedBoneWall>(4).
            AddTile(TileID.WorkBenches).
            DisableDecraft().
            Register();
        }
    }
}
