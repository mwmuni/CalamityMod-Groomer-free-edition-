using CalamityMod.Items.Placeables.FurnitureAshen;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenDoorOpen : BaseFurnitureDoorTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorOpen(ModContent.ItemType<AshenDoor>(), true);
            TileID.Sets.CloseDoorID[Type] = ModContent.TileType<AshenDoorClosed>();
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<AshenDoor>();
        }
    }
}
