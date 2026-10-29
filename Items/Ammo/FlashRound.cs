using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    [LegacyName("FlashBullet")]
    public class FlashRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 12, 18, 6, 6f,
                Item.sellPrice(copper: 1), ItemRarityID.Blue,
                ModContent.ProjectileType<FlashRoundProj>(), 10f, AmmoID.Bullet);

        public override void AddRecipes()
        {
            CreateRecipe(70).
                AddRecipeGroup("AnyCopperBar", 1).
                AddIngredient(ItemID.Glass, 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
