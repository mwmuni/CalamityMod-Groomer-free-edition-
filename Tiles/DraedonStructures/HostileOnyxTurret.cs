using CalamityMod.TileEntities;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class HostileOnyxTurret : HostileTargetingTurretTile<TEHostileOnyxTurret>
    {
        protected override string TurretTexture => "CalamityMod/Tiles/PlayerTurrets/PlayerOnyxTurret";
        protected override string TurretHeadTexture => "CalamityMod/Tiles/PlayerTurrets/OnyxTurretHead";
    }
}
