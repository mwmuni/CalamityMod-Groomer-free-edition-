using CalamityMod.Items.Placeables.FurnitureAbyss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles.BaseTiles;

namespace CalamityMod.Tiles.FurnitureAbyss
{
    public class AbyssDoorOpen : BaseFurnitureDoorTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorOpen(ModContent.ItemType<AbyssDoor>(), true);
            TileID.Sets.CloseDoorID[Type] = ModContent.TileType<AbyssDoorClosed>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Stone, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<AbyssDoor>();
        }
    }
}
