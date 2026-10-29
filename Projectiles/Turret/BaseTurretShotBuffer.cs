using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Turret
{
    public abstract class BaseTurretShotBuffer : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        protected abstract int ShotType { get; }
        protected virtual Vector2 SpawnOffset => Vector2.Zero;

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 1;
        }

        protected virtual void SpawnShots(IEntitySource source)
        {
            Projectile.NewProjectile(source, Projectile.Center + SpawnOffset, Projectile.velocity, ShotType, Projectile.damage, Projectile.knockBack, Main.myPlayer);
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var source = Main.LocalPlayer.GetSource_FromThis();
                SpawnShots(source);
                Projectile.Kill();
            }
        }

        public override bool? CanDamage() => false;
    }
}
