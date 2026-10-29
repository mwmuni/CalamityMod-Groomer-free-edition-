using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeDevourerofGods")]
    public class LoreDevourerofGods : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DevourerofGodsTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
