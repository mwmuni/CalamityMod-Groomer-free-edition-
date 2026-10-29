using Terraria.ID;

namespace CalamityMod.Items.LoreItems
{
    public class LoreQueenSlime : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.QueenSlimeTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
