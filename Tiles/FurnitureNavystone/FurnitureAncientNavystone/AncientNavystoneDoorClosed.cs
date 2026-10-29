using Microsoft.Xna.Framework;
using CalamityMod.Tiles.BaseTiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureNavystone.FurnitureAncientNavystone
{
    [LegacyName("EutrophicDoorClosed")]
    public class AncientNavystoneDoorClosed : BaseFurnitureDoorTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorClosed(ModContent.ItemType<Items.Placeables.FurnitureNavystone.FurnitureAncientNavystone.AncientNavystoneDoor>());
            TileID.Sets.OpenDoorID[Type] = ModContent.TileType<AncientNavystoneDoorOpen>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.SnowBlock, 0f, 0f, 1, new Color(54, 69, 72), 1f);
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.FurnitureNavystone.FurnitureAncientNavystone.AncientNavystoneDoor>();
        }
    }
}
