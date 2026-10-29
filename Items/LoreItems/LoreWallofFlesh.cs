using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeWallofFlesh")]
    public class LoreWallofFlesh : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WallofFleshTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
