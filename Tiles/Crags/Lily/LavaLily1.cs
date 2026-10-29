using Microsoft.Xna.Framework;

namespace CalamityMod.Tiles.Crags.Lily
{
    public class LavaLily1 : LavaLilyBase
    {
        protected override int LilyNumber => 1;
        protected override Rectangle TopSource => new Rectangle(0, 0, 178, 184);
        protected override Vector2 TopOrigin => new Vector2(98, 191);
    }
}
