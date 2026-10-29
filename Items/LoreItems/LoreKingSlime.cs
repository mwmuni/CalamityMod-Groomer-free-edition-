using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeKingSlime")]
    public class LoreKingSlime : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.KingSlimeTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
