using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeGolem")]
    public class LoreGolem : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GolemTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
