using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class AortaYoyo : BaseYoyoProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Aorta>();
        public const int MaxUpdates = 2;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = Aorta.Duration * MaxUpdates;
            ProjectileID.Sets.YoyosMaximumRange[Type] = Aorta.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = Aorta.Speed / MaxUpdates;

            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 16;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.localNPCHitCooldown = 15 * MaxUpdates;
        }

        public override void AI()
        {
            CalamityUtils.MagnetSphereHitscan(Projectile, 240f, 6f, 180f, 3, ModContent.ProjectileType<Blood2>(), 0.25);
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 120);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}
