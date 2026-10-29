using CalamityMod.Items.Placeables.DraedonStructures;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryDoorOpen : LaboratoryDoorDecorBase
    {
        protected override bool IsOpen => true;
        protected override int DoorItemType => ModContent.ItemType<LaboratoryDoorItem>();
        protected override int OppositeDoorType => ModContent.TileType<LaboratoryDoorClosed>();
    }
}
