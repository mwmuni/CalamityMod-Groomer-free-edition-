using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCrimson")]
    public class LoreCrimson : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BrainofCthulhuTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
