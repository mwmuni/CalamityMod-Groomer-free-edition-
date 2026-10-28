using CalamityMod.Projectiles.Turret;
using CalamityMod.Tiles.DraedonStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public class TEHostilePlagueTurret : TEHostileTargetingTurret
    {
        // Tile which hosts this tile entity
        public override int TileType => ModContent.TileType<HostilePlagueTurret>();
        public override int HostTileWidth => HostilePlagueTurret.Width;
        public override int HostTileHeight => HostilePlagueTurret.Height;

        // Projectile variables
        public override int ProjectileType => ModContent.ProjectileType<PlagueShotBuffer>();
        public override int ProjectileDamage => Main.expertMode ? 36 : 48;
        public override float ProjectileKnockback => 0f;
        public override float ShootSpeed => 16f;
        public override int FiringStartupDelay => 50;
        public override int FiringUseTime => 50;

        // Projectile spawn location variables
        public override Vector2 TurretCenterOffset => new Vector2(22f + 4f * Direction, -2f);
        protected override float ShootForwardsOffset => 24f;

        // Targeting variables
        public override float MaxRange => 900f;
        protected override float MaxTargetAngleDeviance => MathHelper.ToRadians(50f);
        protected override float MaxDeltaAnglePerFrame => MathHelper.ToRadians(6f);
        protected override float CloseAimThreshold => MathHelper.ToRadians(12f);
        protected override float CloseAimLerpFactor => 0.08f;

        // Lab turrets specifically must be placed on the top left corner of their host tile.
        // This restriction is not shared by general turrets for flexibility reasons.
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == TileType && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

    }
}
