using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeTwins")]
    public class LoreTwins : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RetinazerTrophy).
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.SpazmatismTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
