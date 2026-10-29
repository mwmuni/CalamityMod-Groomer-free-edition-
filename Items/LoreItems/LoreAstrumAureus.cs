using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeAstrumAureus")]
    public class LoreAstrumAureus : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstrumAureusTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
