using CalamityMod.Dusts.Furniture;
using CalamityMod.Items.Placeables.FurnitureOtherworldly;
using Microsoft.Xna.Framework;
using CalamityMod.Tiles.BaseTiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureOtherworldly
{
    [LegacyName("OccultDoorOpen")]
    public class OtherworldlyDoorOpen : BaseFurnitureDoorTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorOpen(ModContent.ItemType<OtherworldlyDoor>(), true);
            TileID.Sets.CloseDoorID[Type] = ModContent.TileType<OtherworldlyDoorClosed>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Stone, 0f, 0f, 1, new Color(125, 94, 128), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<OtherworldlyTileCloth>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<OtherworldlyDoor>();
        }
    }
}
