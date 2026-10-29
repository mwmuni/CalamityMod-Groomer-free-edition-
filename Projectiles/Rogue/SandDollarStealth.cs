using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class SandDollarStealth : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SandDollar";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 28;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = ProjAIStyleID.Boomerang;
            Projectile.timeLeft = 300;
            AIType = ProjectileID.Bananarang;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => OnHitEffects();

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => OnHitEffects();

        private void OnHitEffects()
        {
            int coralAmt = Main.rand.Next(1, 4);
            if (Projectile.owner == Main.myPlayer && Projectile.Calamity().stealthStrike)
            {
                for (int coralCount = 0; coralCount < coralAmt; coralCount++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SmallCoral>(), Projectile.damage / 3, 0f, Projectile.owner);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] += 0.1f;
            Projectile.BounceOnTileCollision(oldVelocity);
            return false;
        }
    }
}
