using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class MicrowaveYoyo : BaseYoyoProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TheMicrowave>();
        public const int MaxUpdates = 3;
        private const float Radius = 100f;
        private bool spawnedAura = false;
        public int soundCooldown = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = TheMicrowave.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = TheMicrowave.Speed / MaxUpdates;

            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 16;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.localNPCHitCooldown = 10 * MaxUpdates;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(soundCooldown);
        public override void ReceiveExtraAI(BinaryReader reader) => soundCooldown = reader.ReadInt32();

        public override void AI()
        {
            SingularSoundInstanceSystem.PlaySingleInstance(TheMicrowave.MMMSound, 6, 6, Projectile);

            // Spawn invisible but damaging aura projectile
            if (Projectile.owner == Main.myPlayer && !spawnedAura)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MicrowaveAura>(), (int)(Projectile.damage * 0.35), Projectile.knockBack, Projectile.owner, Projectile.identity, 0f);
                spawnedAura = true;
            }

            // Decrement sound cooldown
            if (soundCooldown > 0 && Projectile.FinalExtraUpdate())
                soundCooldown--;

            // Dust circle appears for all players, even though the aura projectile is only spawned by the owner
            int numDust = (int)(0.2f * MathHelper.TwoPi * Radius);
            float angleIncrement = MathHelper.TwoPi / (float)numDust;
            Vector2 dustOffset = new Vector2(Radius, 0f);
            dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
            for (int i = 0; i < numDust; i++)
            {
                dustOffset = dustOffset.RotatedBy(angleIncrement);
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                        ModContent.DustType<AstralOrange>(),
                        ModContent.DustType<AstralBlue>()
                });
                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType);
                Main.dust[dust].position = Projectile.Center + dustOffset;
                Main.dust[dust].fadeIn = 1f;
                Main.dust[dust].velocity *= 0.2f;
                Main.dust[dust].scale = 0.1599999999f;
            }

            // Delete the projectile if it is farther than 200 blocks away from the player
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Rectangle frame = new Rectangle(0, 0, 20, 16);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/MicrowaveYoyoGlow").Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 0);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            if (target.life <= 0 && soundCooldown <= 0)
            {
                SoundEngine.PlaySound(TheMicrowave.BeepSound, Projectile.Center);
                soundCooldown = 45;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            if (target.statLife <= 0 && soundCooldown <= 0)
            {
                SoundEngine.PlaySound(TheMicrowave.BeepSound, Projectile.Center);
                soundCooldown = 45;
            }
        }
    }
}
