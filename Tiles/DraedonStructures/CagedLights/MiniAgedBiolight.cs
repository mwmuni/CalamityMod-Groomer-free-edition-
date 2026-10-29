using CalamityMod.ExtraTextures.GreyscaleGradients;
using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class MiniAgedBiolight : AgedPulsingLightTile<MiniAgedLablightItem>
    {
        protected override bool IsMini => true;
        protected override int FixtureDustType => DustID.GemEmerald;
        protected override float Pulse => GreyscaleGradient.PlagueContainmentCellsPulse.GetRepeat((int)Main.GameUpdateCount);
        protected override Vector3 LightColor => new Vector3(9f / 255f, 195f / 255f, 0f);
    }
}
