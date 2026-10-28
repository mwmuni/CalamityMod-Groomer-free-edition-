using CalamityMod.Projectiles.Turret;
using CalamityMod.Tiles.PlayerTurrets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public class TEPlayerLabTurret : TENPCTargetingTurret
    {
        // Tile which hosts this tile entity
        public override int TileType => ModContent.TileType<PlayerLabTurret>();
        public override int HostTileWidth => PlayerLabTurret.Width;
        public override int HostTileHeight => PlayerLabTurret.Height;

        // Projectile variables
        public override int ProjectileType => ModContent.ProjectileType<DraedonLaserBuffer>();
        public override int ProjectileDamage => 40;
        public override float ProjectileKnockback => 3.5f;
        public override float ShootSpeed => 5f;
        public override int FiringStartupDelay => 10;
        public override int FiringUseTime => 55;

        // Projectile spawn location variables
        public override Vector2 TurretCenterOffset => new Vector2(22f + 4f * Direction, -2f);
        protected override float ShootForwardsOffset => 6f;

        // Targeting variables
        public override float MaxRange => 600f;
        protected override float MaxTargetAngleDeviance => MathHelper.ToRadians(12f);
        protected override float MaxDeltaAnglePerFrame => MathHelper.ToRadians(4f);
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
