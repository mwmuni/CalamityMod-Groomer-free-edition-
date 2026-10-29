using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeRavager")]
    public class LoreRavager : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RavagerTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
