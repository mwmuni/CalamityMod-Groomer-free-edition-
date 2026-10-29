using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgePlaguebringerGoliath")]
    public class LorePlaguebringerGoliath : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlaguebringerGoliathTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
