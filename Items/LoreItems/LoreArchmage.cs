using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCryogen")]
    public class LoreArchmage : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryogenTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
