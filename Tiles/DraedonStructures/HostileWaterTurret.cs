using CalamityMod.TileEntities;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class HostileWaterTurret : HostileTargetingTurretTile<TEHostileWaterTurret>
    {
        protected override string TurretTexture => "CalamityMod/Tiles/PlayerTurrets/PlayerWaterTurret";
        protected override string TurretHeadTexture => "CalamityMod/Tiles/PlayerTurrets/WaterTurretHead";
    }
}
