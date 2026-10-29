using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeProfanedGuardians")]
    public class LoreProfanedGuardians : LoreItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Purple;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ProfanedGuardianTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
