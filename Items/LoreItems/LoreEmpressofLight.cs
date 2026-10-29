using Terraria.ID;

namespace CalamityMod.Items.LoreItems
{
    public class LoreEmpressofLight : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FairyQueenTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
