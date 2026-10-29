using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeQueenBee")]
    public class LoreQueenBee : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.QueenBeeTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
