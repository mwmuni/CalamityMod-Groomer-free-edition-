using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCrabulon")]
    public class LoreCrabulon : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrabulonTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
