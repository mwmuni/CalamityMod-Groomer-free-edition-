using CalamityMod.Items.Placeables.DraedonStructures;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class AgedLaboratoryDoorOpen : LaboratoryDoorDecorBase
    {
        protected override bool IsOpen => true;
        protected override int DoorItemType => ModContent.ItemType<AgedLaboratoryDoorItem>();
        protected override int OppositeDoorType => ModContent.TileType<AgedLaboratoryDoorClosed>();
    }
}
