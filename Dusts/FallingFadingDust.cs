using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts
{
    public abstract class FallingFadingDust : ModDust
    {
        protected abstract float DownwardAcceleration { get; }

        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            dust.noGravity = false;
            dust.scale = 1.2f;
            dust.alpha = 100;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity.Y += DownwardAcceleration;
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X;
            dust.scale -= 0.03f;
            if (dust.scale < 0.5f)
            {
                dust.active = false;
            }
            return false;
        }
    }
}
