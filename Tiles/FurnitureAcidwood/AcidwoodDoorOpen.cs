using CalamityMod.Items.Placeables.FurnitureAcidwood;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAcidwood
{
    public class AcidwoodDoorOpen : BaseFurnitureDoorTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorOpen(ModContent.ItemType<AcidwoodDoor>());
            TileID.Sets.CloseDoorID[Type] = ModContent.TileType<AcidwoodDoorClosed>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.WoodFurniture, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<AcidwoodDoor>();
        }
    }
}
