using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class RadiatorPet : BaseFollowingPetProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";

        protected override bool IsPetActive(CalamityPlayer player) => player.radiator;
        protected override void DisablePet(CalamityPlayer player) => player.radiator = false;

        protected override void AddPetLight()
        {
            Lighting.AddLight(Projectile.Center, 0.6f, 0.6f, 0.6f);
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 34;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = true;
        }
    }
}
