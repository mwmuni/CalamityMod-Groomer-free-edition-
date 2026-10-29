using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class Astrophage : BaseFollowingPetProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";

        protected override bool IsPetActive(CalamityPlayer player) => player.astrophage;
        protected override void DisablePet(CalamityPlayer player) => player.astrophage = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 6)
            .WithOffset(-10f, 0f).WithSpriteDirection(-1).WhenNotSelected(0, 0);
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = true;
        }
    }
}
