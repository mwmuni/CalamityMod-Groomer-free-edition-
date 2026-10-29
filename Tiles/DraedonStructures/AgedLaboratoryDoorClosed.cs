using CalamityMod.Items.Placeables.DraedonStructures;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class AgedLaboratoryDoorClosed : LaboratoryDoorDecorBase
    {
        protected override bool IsOpen => false;
        protected override int DoorItemType => ModContent.ItemType<AgedLaboratoryDoorItem>();
        protected override int OppositeDoorType => ModContent.TileType<AgedLaboratoryDoorOpen>();
    }
}
