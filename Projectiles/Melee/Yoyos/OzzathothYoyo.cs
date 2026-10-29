using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class OzzathothYoyo : BaseYoyoProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Ozzathoth>();
        public const int MaxUpdates = 3;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = Ozzathoth.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = Ozzathoth.Speed / MaxUpdates;

            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 24;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.localNPCHitCooldown = 2 * MaxUpdates;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(MaxUpdates))
            {
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.2f, 0.55f), ModContent.ProjectileType<CosmicOrb>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0f, 0f);
            }
            if (Main.rand.NextBool((int)(MaxUpdates / 2)))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), -Projectile.velocity * Main.rand.NextFloat(0.3f, 1f), 0, default, Main.rand.NextFloat(1.85f, 2.2f));
                dust.color = Main.rand.NextBool() ? Color.Magenta : Color.HotPink;
                bool b = !Main.rand.NextBool(3);
                dust.scale -= b ? 0.8f : 0;
                dust.velocity *= b ? 0.4f : 1;
                dust.noGravity = !b;
            }
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}
