using CalamityMod.Items.Placeables.FurnitureStratus;
using Microsoft.Xna.Framework;
using CalamityMod.Tiles.BaseTiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureStratus
{
    public class StratusDoorOpen : BaseFurnitureDoorTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorOpen(ModContent.ItemType<StratusDoor>(), true);
            TileID.Sets.CloseDoorID[Type] = ModContent.TileType<StratusDoorClosed>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Stone, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Firework_Blue, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<StratusDoor>();
        }
    }
}
