using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeBloodMoon")]
    public class LoreBloodMoon : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BloodMoonStarter).
                AddIngredient(ItemID.SoulofNight, 3).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
