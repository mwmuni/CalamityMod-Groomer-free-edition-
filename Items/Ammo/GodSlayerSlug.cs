using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class GodSlayerSlug : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 22, 22, 18, 3f,
                Item.sellPrice(copper: 28), ModContent.RarityType<CosmicPurple>(),
                ModContent.ProjectileType<GodSlayerSlugProj>(), 6f, ItemID.MusketBall);

        public override void AddRecipes()
        {
            CreateRecipe(999).
                AddIngredient(ItemID.MoonlordBullet, 999).
                AddIngredient<CosmiliteBar>().
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
