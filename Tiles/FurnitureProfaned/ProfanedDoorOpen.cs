using CalamityMod.Dusts.Furniture;
using CalamityMod.Items.Placeables.FurnitureProfaned;
using Microsoft.Xna.Framework;
using CalamityMod.Tiles.BaseTiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureProfaned
{
    public class ProfanedDoorOpen : BaseFurnitureDoorTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorOpen(ModContent.ItemType<ProfanedDoor>(), true);
            TileID.Sets.CloseDoorID[Type] = ModContent.TileType<ProfanedDoorClosed>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.GoldCoin, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<ProfanedTileRock>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<ProfanedDoor>();
        }
    }
}
