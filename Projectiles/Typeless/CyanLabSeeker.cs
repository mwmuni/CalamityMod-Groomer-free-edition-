using CalamityMod.World;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Typeless
{
    public class CyanLabSeeker : LabSeekerBase
    {
        public override string Texture => "CalamityMod/Items/LabFinders/CyanSeekingMechanism";

        protected override Vector2 Destination => CalamityWorld.SunkenSeaLabCenter;
        protected override Color DustColor => Color.Cyan;
    }
}
