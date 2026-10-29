using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeExoMechs")]
    public class LoreExoMechs : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<ExoticRainbow>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArtemisTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<ApolloTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<ThanatosTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<AresTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
