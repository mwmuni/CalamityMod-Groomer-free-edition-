using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls.UnsafeWalls;

namespace CalamityMod.Items.Placeables.Walls
{
    public class UnsafeNavystoneWall : PlaceableWallItem<WallTiles.UnsafeNavystoneWall>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Items.Placeables.Walls.NavystoneWall>();
        }

    }
}
