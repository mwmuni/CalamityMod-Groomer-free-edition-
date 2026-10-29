using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeSkeletron")]
    public class LoreSkeletron : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SkeletronTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
