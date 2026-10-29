using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class LoreStormWeaver : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WeaverTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
