using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class MortarRound : ModItem, ILocalizedModType
    {
        internal static int TileBlastRadiusNormal = 3;
        internal static int TileBlastRadiusGFB = 5;
        public static int TileBlastRadius => Main.getGoodWorld ? TileBlastRadiusGFB : TileBlastRadiusNormal;
        public static int HitboxBlastRadius => TileBlastRadius * 16 + 12;

        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 20, 14, 8, 7f,
                Item.sellPrice(copper: 20), ItemRarityID.Yellow,
                ModContent.ProjectileType<MortarRoundProj>(), 0f, AmmoID.Bullet);

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.EmptyBullet, 100).
                AddIngredient(ItemID.ExplosivePowder, 4).
                AddIngredient<ScoriaBar>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
