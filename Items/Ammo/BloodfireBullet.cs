using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class BloodfireBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 14, 30, 17, 4.5f,
                Item.sellPrice(copper: 24), ModContent.RarityType<Turquoise>(),
                ModContent.ProjectileType<BloodfireBulletProj>(), 0.1f, ItemID.MusketBall);

        public override void AddRecipes()
        {
            CreateRecipe(333).
                AddIngredient<Bloodstone>().
                AddIngredient<BloodOrb>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
