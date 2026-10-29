using Terraria.ID;

namespace CalamityMod.Items.LoreItems
{
    public class LoreAwakening : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ArmorStatue).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
