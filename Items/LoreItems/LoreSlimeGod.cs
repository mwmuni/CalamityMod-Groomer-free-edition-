using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeSlimeGod")]
    public class LoreSlimeGod : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SlimeGodTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
