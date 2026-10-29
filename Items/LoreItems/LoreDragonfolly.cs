using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeBumblebirb", "KnowledgeDragonfolly")]
    public class LoreDragonfolly : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Purple;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DragonfollyTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
