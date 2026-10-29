using CalamityMod.Items.Placeables.FurnitureAncient;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAncient
{
    public class AncientDoorOpen : BaseFurnitureDoorTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorOpen(ModContent.ItemType<AncientDoor>(), true);
            TileID.Sets.CloseDoorID[Type] = ModContent.TileType<AncientDoorClosed>();
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<AncientDoor>();
        }
    }
}
