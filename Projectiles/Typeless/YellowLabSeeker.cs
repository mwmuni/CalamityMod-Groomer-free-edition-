using CalamityMod.World;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Typeless
{
    public class YellowLabSeeker : LabSeekerBase
    {
        public override string Texture => "CalamityMod/Items/LabFinders/YellowSeekingMechanism";

        protected override Vector2 Destination => CalamityWorld.PlanetoidLabCenter;
        protected override Color DustColor => Color.Yellow;
    }
}
