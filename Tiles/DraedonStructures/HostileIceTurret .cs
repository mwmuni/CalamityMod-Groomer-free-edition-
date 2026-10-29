using CalamityMod.TileEntities;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class HostileIceTurret : HostileTargetingTurretTile<TEHostileIceTurret>
    {
        protected override string TurretTexture => "CalamityMod/Tiles/PlayerTurrets/PlayerIceTurret";
        protected override string TurretHeadTexture => "CalamityMod/Tiles/PlayerTurrets/IceTurretHead";
    }
}
