using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class Shadowflamethrower : BaseTwinsFireProjectile, ILocalizedModType
    {
        protected override Vector3 LightColor => new Vector3(0.5f, 0.05f, 0.6f);
        protected override Color FireColor1 => new Color(200, 10, 255, 200);
        protected override Color FireColor2 => new Color(200, 50, 180, 70);
        protected override Color FireColor3 => new Color(200, 30, 255, 100);
        protected override Color FireColor4 => new Color(80, 0, 100, 100);

        protected override int GetCinderDustType()
        {
            int dustType = Main.rand.Next(4);
            switch (dustType)
            {
                default:
                case 0:
                case 1:
                    dustType = 27;
                    break;

                case 2:
                    dustType = 112;
                    break;

                case 3:
                    dustType = 173;
                    break;
            }

            return dustType;
        }

        protected override void ApplyDebuff(Player target) => target.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
    }
}
