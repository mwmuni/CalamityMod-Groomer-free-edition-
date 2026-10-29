using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public abstract class PlaceableTileItem<TTile> : ModItem, ILocalizedModType where TTile : ModTile
    {
        public new string LocalizationCategory => "Items.Placeables";

        public override void SetDefaults() => Item.DefaultToPlaceableTile(ModContent.TileType<TTile>());
    }
}
