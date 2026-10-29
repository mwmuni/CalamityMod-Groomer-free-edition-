using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Walls
{
    public abstract class PlaceableWallItem<TWall> : ModItem, ILocalizedModType where TWall : ModWall
    {
        public new string LocalizationCategory => "Items.Placeables";

        public override void SetDefaults() => Item.DefaultToPlaceableWall(ModContent.WallType<TWall>());
    }
}
