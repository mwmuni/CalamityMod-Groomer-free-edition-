using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeAstralInfection")]
    public class LoreAstralInfection : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstrumDeusTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
