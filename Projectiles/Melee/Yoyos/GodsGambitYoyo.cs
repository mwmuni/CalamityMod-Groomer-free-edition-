using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class GodsGambitYoyo : BaseYoyoProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TheGodsGambit>();
        public const int MaxUpdates = 2;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = TheGodsGambit.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = TheGodsGambit.Speed / MaxUpdates;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 22;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.localNPCHitCooldown = 15 * MaxUpdates;
        }

        public override void AI()
        {
            CalamityUtils.MagnetSphereHitscan(Projectile, 300f, 6f, 30f, 5, ModContent.ProjectileType<SlimeStream>(), 0.75, damageType: Projectile.DamageType);
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Slimed, 300);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
