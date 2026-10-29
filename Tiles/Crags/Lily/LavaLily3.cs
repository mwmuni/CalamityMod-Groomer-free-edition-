using Microsoft.Xna.Framework;

namespace CalamityMod.Tiles.Crags.Lily
{
    public class LavaLily3 : LavaLilyBase
    {
        protected override int LilyNumber => 3;
        protected override Rectangle TopSource => new Rectangle(0, 0, 178, 198);
        protected override Vector2 TopOrigin => new Vector2(98, 199);
    }
}
