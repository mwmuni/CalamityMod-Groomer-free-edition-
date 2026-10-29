using CalamityMod.TileEntities;

namespace CalamityMod.Tiles.PlayerTurrets
{
    public class PlayerFireTurret : PlayerTargetingTurretTile<TEPlayerFireTurret>
    {
        protected override string TurretHeadTexture => "CalamityMod/Tiles/PlayerTurrets/FireTurretHead";
    }
}
