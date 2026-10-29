using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCalamitasClone")]
    public class LoreCalamitasClone : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CalamitasCloneTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
