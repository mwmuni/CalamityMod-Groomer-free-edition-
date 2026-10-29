using CalamityMod.World;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Typeless
{
    public class OnyxLabSeeker : LabSeekerBase
    {
        public override string Texture => "CalamityMod/Items/LabFinders/OnyxSeekingMechanism";

        protected override Vector2 Destination => CalamityWorld.CavernLabCenter;
        protected override Color DustColor => Color.Magenta;
    }
}
