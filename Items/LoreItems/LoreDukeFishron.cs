using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeDukeFishron")]
    public class LoreDukeFishron : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DukeFishronTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
