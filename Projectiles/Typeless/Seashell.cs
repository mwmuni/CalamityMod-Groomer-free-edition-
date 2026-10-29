using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class Seashell : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.aiStyle = ProjAIStyleID.Boomerang;
            Projectile.timeLeft = 300;
            AIType = ProjectileID.WoodenBoomerang;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] += 0.1f;
            Projectile.BounceOnTileCollision(oldVelocity);
            return false;
        }
    }
}
