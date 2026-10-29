using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class CagedFlamelight : CagedLightTile<CagedLablightItem>
    {
        protected override int ItemDropType => ModContent.ItemType<CagedFlamelightItem>();
        protected override int FixtureDustType => DustID.RedTorch;
        protected override Vector3 LightColor => new Vector3(255f / 255f, 104f / 255f, 104f / 255f);
    }
}
