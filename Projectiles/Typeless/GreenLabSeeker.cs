using CalamityMod.World;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Typeless
{
    public class GreenLabSeeker : LabSeekerBase
    {
        public override string Texture => "CalamityMod/Items/LabFinders/GreenSeekingMechanism";

        protected override Vector2 Destination => CalamityWorld.JungleLabCenter;
        protected override Color DustColor => Color.Green;
    }
}
