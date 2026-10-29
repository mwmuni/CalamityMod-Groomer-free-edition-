using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeDestroyer")]
    public class LoreDestroyer : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DestroyerTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
