using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeSkeletronPrime")]
    public class LoreSkeletronPrime : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SkeletronPrimeTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
