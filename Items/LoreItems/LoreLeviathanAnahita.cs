using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeLeviathanandSiren", "KnowledgeLeviathanAnahita")]
    public class LoreLeviathanAnahita : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LeviathanTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<AnahitaTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
