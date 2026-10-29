using CalamityMod.TileEntities;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class HostileLaserTurret : HostileTargetingTurretTile<TEHostileLaserTurret>
    {
        protected override string TurretTexture => "CalamityMod/Tiles/PlayerTurrets/PlayerLaserTurret";
        protected override string TurretHeadTexture => "CalamityMod/Tiles/PlayerTurrets/LaserTurretHead";
    }
}
