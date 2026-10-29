using CalamityMod.TileEntities;

namespace CalamityMod.Tiles.PlayerTurrets
{
    public class PlayerLaserTurret : PlayerTargetingTurretTile<TEPlayerLaserTurret>
    {
        protected override string TurretHeadTexture => "CalamityMod/Tiles/PlayerTurrets/LaserTurretHead";
    }
}
