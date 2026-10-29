using Microsoft.Xna.Framework;

namespace CalamityMod.Tiles.Crags.Lily
{
    public class LavaLily2 : LavaLilyBase
    {
        protected override int LilyNumber => 2;
        protected override Rectangle TopSource => new Rectangle(0, 0, 192, 206);
        protected override Vector2 TopOrigin => new Vector2(90, 209);
    }
}
