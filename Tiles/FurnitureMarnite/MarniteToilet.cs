using Microsoft.Xna.Framework;
using CalamityMod.Tiles.BaseTiles;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureMarnite
{
    public class MarniteToilet : BaseToiletTile
    {
        protected override int AssociatedItemType => ModContent.ItemType<Items.Placeables.FurnitureMarnite.MarniteToilet>();

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            // Toilets count as Chairs
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            AddMapEntry(new Color(191, 142, 111), Language.GetText("MapObject.Toilet"));
            TileID.Sets.CanBeSatOnForNPCs[Type] = true;
            TileID.Sets.CanBeSatOnForPlayers[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            AdjTiles = new int[] { TileID.Chairs };
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Granite, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

    }
}
