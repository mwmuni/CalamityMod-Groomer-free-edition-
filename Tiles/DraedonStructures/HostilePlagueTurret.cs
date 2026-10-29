using CalamityMod.TileEntities;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class HostilePlagueTurret : HostileTargetingTurretTile<TEHostilePlagueTurret>
    {
        protected override string TurretTexture => "CalamityMod/Tiles/PlayerTurrets/PlayerPlagueTurret";
        protected override string TurretHeadTexture => "CalamityMod/Tiles/PlayerTurrets/PlagueTurretHead";
    }
}
