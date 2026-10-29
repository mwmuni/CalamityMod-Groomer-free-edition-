using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgePolterghast")]
    public class LorePolterghast : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PolterghastTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
