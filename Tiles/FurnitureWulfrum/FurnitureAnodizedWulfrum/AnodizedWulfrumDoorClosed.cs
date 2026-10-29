using Microsoft.Xna.Framework;
using CalamityMod.Tiles.BaseTiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureWulfrum.FurnitureAnodizedWulfrum
{
    public class AnodizedWulfrumDoorClosed : BaseFurnitureDoorTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorClosed(ModContent.ItemType<Items.Placeables.FurnitureWulfrum.FurnitureAnodizedWulfrum.AnodizedWulfrumDoor>(), true);
            TileID.Sets.OpenDoorID[Type] = ModContent.TileType<AnodizedWulfrumDoorOpen>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Tungsten, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.FurnitureWulfrum.FurnitureAnodizedWulfrum.AnodizedWulfrumDoor>();
        }
    }
}
