using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class SeasSearingSpout : BaseSpoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Melee/BrinySpout";

        protected override int HitCooldown => 30;

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(60, Main.DiscoG, 190, Projectile.alpha);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Venom, 180);
    }
}
