using CalamityMod.TileEntities;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class HostileFireTurret : HostileTargetingTurretTile<TEHostileFireTurret>
    {
        protected override string TurretTexture => "CalamityMod/Tiles/PlayerTurrets/PlayerFireTurret";
        protected override string TurretHeadTexture => "CalamityMod/Tiles/PlayerTurrets/FireTurretHead";
    }
}
