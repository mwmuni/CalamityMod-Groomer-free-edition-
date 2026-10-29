using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    [LegacyName("TerraBullet")]
    public class DryadsTear : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 8, 8, 18, 1.25f,
                Item.sellPrice(copper: 16), ItemRarityID.Lime,
                ModContent.ProjectileType<DryadsTearMain>(), 2f, AmmoID.Bullet);

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.CrystalBullet, 100).
                AddIngredient<LivingShard>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
