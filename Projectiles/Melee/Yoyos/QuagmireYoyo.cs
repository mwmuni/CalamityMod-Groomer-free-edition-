using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class QuagmireYoyo : BaseYoyoProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Quagmire>();
        public const int MaxUpdates = 2;
        public ref float Timer => ref Projectile.localAI[2];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = Quagmire.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = Quagmire.Speed / MaxUpdates;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 16;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.localNPCHitCooldown = 10 * MaxUpdates;
        }

        public override void AI()
        {
            Timer++;
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
            if (Main.rand.NextBool(5 * MaxUpdates))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.JungleSpore, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            if (Projectile.owner == Main.myPlayer)
            {
                if (Timer % (10 * MaxUpdates) == 0)
                {
                    Projectile spore = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * Main.rand.NextFloat(0.15f, 0.35f) + Vector2.UnitX.RotatedByRandom(MathHelper.Pi), ProjectileID.SporeGas + Main.rand.Next(3), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
                    spore.DamageType = DamageClass.MeleeNoSpeed;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Venom, 180);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
