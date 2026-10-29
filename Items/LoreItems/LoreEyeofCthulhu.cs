using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeEyeofCthulhu")]
    public class LoreEyeofCthulhu : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EyeofCthulhuTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
