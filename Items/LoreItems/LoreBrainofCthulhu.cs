using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeBrainofCthulhu")]
    public class LoreBrainofCthulhu : LoreItem
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
