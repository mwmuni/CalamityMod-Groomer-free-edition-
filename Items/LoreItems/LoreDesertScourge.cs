using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeDesertScourge")]
    public class LoreDesertScourge : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertScourgeTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
