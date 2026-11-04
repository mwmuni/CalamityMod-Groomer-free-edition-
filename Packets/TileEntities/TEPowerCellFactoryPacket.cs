using System.IO;
using CalamityMod.TileEntities;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class TEPowerCellFactoryPacket : CalamityPacket
    {
        public static TEPowerCellFactoryPacket Instance { get; private set; }

        public static void Send(TEPowerCellFactory cellFactory, long time, short stack, int toClient = -1, int ignoreClient = -1)
        {
            if (cellFactory is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteTileEntityID(cellFactory);
            packet.Write(time);
            packet.Write(stack);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var cellFactory = packet.ReadTileEntity<TEPowerCellFactory>();
            long time = packet.ReadInt64();
            short cellStack = packet.ReadInt16();

            if (cellFactory is null)
                return;

            // Only clients update their timer from this packet. When a server receives this packet it ignores the time variable.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                cellFactory.Time = time;
            cellFactory.Stack_Internal = cellStack;

            // When a server gets this packet, it immediately sends an equivalent packet to all clients.
            if (Main.dedServ)
                Send(cellFactory, cellFactory.Time, cellStack, ignoreClient: sender);
        }
    }
}
