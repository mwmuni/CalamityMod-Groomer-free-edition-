using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCalamitas")]
    public class LoreCalamitas : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<CalamityRed>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SupremeCalamitasTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
