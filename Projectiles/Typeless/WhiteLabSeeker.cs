using CalamityMod.World;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Typeless
{
    public class WhiteLabSeeker : LabSeekerBase
    {
        public override string Texture => "CalamityMod/Items/LabFinders/WhiteSeekingMechanism";

        protected override Vector2 Destination => CalamityWorld.IceLabCenter;
        protected override Color DustColor => Color.White;
    }
}
