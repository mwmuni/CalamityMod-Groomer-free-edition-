using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeMoonLord")]
    public class LoreRequiem : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoonLordTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
