using CalamityMod.Projectiles.Turret;
using CalamityMod.Tiles.DraedonStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public class TEHostileOnyxTurret : TEHostileTargetingTurret
    {
        // Tile which hosts this tile entity
        public override int TileType => ModContent.TileType<HostileOnyxTurret>();
        public override int HostTileWidth => HostileOnyxTurret.Width;
        public override int HostTileHeight => HostileOnyxTurret.Height;

        // Projectile variables
        public override int ProjectileType => ModContent.ProjectileType<OnyxShotBuffer>();
        public override int ProjectileDamage => Main.expertMode ? 30 : 40;
        public override float ProjectileKnockback => 0f;
        public override float ShootSpeed => 6.5f;
        public override int FiringStartupDelay => 55;
        public override int FiringUseTime => 55;

        // Projectile spawn location variables
        public override Vector2 TurretCenterOffset => new Vector2(22f + 4f * Direction, -2f);
        protected override float ShootForwardsOffset => 30f;

        // Targeting variables
        public override float MaxRange => 300f;
        protected override float MaxTargetAngleDeviance => MathHelper.ToRadians(36f);
        protected override float MaxDeltaAnglePerFrame => MathHelper.ToRadians(5f);
        protected override float CloseAimThreshold => MathHelper.ToRadians(2f);
        protected override float CloseAimLerpFactor => 0.2f;

        // Lab turrets specifically must be placed on the top left corner of their host tile.
        // This restriction is not shared by general turrets for flexibility reasons.
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == TileType && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

    }
}
