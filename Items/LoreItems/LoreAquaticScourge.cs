using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeAquaticScourge")]
    public class LoreAquaticScourge : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 38;
            Item.height = 26;
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AquaticScourgeTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
