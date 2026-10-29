using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgePlantera")]
    public class LorePlantera : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PlanteraTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
