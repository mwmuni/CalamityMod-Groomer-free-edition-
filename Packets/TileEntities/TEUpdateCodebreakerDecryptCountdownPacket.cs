using System.IO;
using CalamityMod.TileEntities;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class TEUpdateCodebreakerDecryptCountdownPacket : CalamityPacket
    {
        public static TEUpdateCodebreakerDecryptCountdownPacket Instance { get; private set; }

        public static void Send(TECodebreaker codeBreaker, int toClient = -1, int ignoreClient = -1)
        {
            if (codeBreaker is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteTileEntityID(codeBreaker);
            packet.Write(codeBreaker.DecryptionCountdown);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var codeBreaker = packet.ReadTileEntity<TECodebreaker>();
            int countdown = packet.ReadInt32();

            // Verify to ensure that the tile entity is a valid one.
            if (codeBreaker is null)
                return;

            codeBreaker.DecryptionCountdown = countdown;

            if (Main.netMode == NetmodeID.Server)
                Send(codeBreaker, ignoreClient: sender);
        }
    }
}
