using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCorruption")]
    public class LoreCorruption : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EaterofWorldsTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
