using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class LoreCynosure : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<HotPink>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ShadowspecBar>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
