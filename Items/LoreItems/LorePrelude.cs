using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeLunaticCultist")]
    public class LorePrelude : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AncientCultistTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
