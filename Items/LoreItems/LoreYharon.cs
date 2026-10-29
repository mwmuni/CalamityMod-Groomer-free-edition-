using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeYharon")]
    public class LoreYharon : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<YharonTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
