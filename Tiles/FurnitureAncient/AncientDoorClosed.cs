using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAncient
{
    public class AncientDoorClosed : BaseFurnitureDoorTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorClosed(ModContent.ItemType<Items.Placeables.FurnitureAncient.AncientDoor>(), true);
            TileID.Sets.OpenDoorID[Type] = ModContent.TileType<AncientDoorOpen>();
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.FurnitureAncient.AncientDoor>();
        }
    }
}
