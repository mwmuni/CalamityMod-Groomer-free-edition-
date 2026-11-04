using System.IO;
using CalamityMod.Items;
using CalamityMod.TileEntities;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class TEChargingStationStandardPacket : CalamityPacket
    {
        public static TEChargingStationStandardPacket Instance { get; private set; }

        public static void Send(TEChargingStation chargingStn, short timer, short cellStack, float chargeOrNaN, int toClient = -1, int ignoreClient = -1)
        {
            if (chargingStn is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteTileEntityID(chargingStn);
            packet.Write(timer);
            packet.Write(cellStack);
            packet.Write(chargeOrNaN);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var chargingStn = packet.ReadTileEntity<TEChargingStation>();
            short timer = packet.ReadInt16();
            short cellStack = packet.ReadInt16();
            float chargeOrNaN = packet.ReadSingle();

            if (chargingStn is null)
                return;

            // Only clients update their timer from this packet. When a server receives this packet it ignores the time variable.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                chargingStn.Internal_ChargingTimer = timer;
            chargingStn.Internal_Stack = cellStack;

            // If the charge value sent is not garbage, then try to apply the new charge to the plugged item.
            if (!float.IsNaN(chargeOrNaN))
            {
                bool itemExists = chargingStn.PluggedItem != null && !chargingStn.PluggedItem.IsAir;
                CalamityGlobalItem modItem = itemExists ? chargingStn.PluggedItem.Calamity() : null;
                if (modItem != null && modItem.UsesCharge)
                {
                    if (modItem.Charge != chargeOrNaN && Main.netMode == NetmodeID.MultiplayerClient)
                        chargingStn.ClientChargingDust = true;
                    modItem.Charge = chargeOrNaN;
                }
            }

            if (Main.dedServ)
                Send(chargingStn, chargingStn.Internal_ChargingTimer, cellStack, chargeOrNaN, ignoreClient: sender);
        }
    }
}
