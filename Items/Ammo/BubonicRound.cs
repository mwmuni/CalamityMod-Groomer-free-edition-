using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    [LegacyName("AcidBullet", "AcidRound")]
    public class BubonicRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 8, 8, 16, 1.5f,
                Item.sellPrice(copper: 16), ItemRarityID.Yellow,
                ModContent.ProjectileType<BubonicRoundProj>(), 10f, AmmoID.Bullet);

        public override void AddRecipes()
        {
            CreateRecipe(150).
                AddIngredient(ItemID.MusketBall, 150).
                AddIngredient<PlagueCellCanister>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
