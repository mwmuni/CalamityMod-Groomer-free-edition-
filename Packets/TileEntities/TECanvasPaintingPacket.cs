using System.IO;
using CalamityMod.TileEntities;
using Terraria;

namespace CalamityMod.Packets
{
    internal sealed class TECanvasPaintingPacket : CalamityPacket
    {
        public static TECanvasPaintingPacket Instance { get; private set; }

        public static void Send(TECanvasPainting painting, float posX, float posY, float scale, int toClient = -1, int ignoreClient = -1)
        {
            if (painting is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteTileEntityID(painting);
            packet.Write(posX);
            packet.Write(posY);
            packet.Write(scale);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var painting = packet.ReadTileEntity<TECanvasPainting>();
            float posX = packet.ReadSingle();
            float posY = packet.ReadSingle();
            float scale = packet.ReadSingle();

            if (painting is null)
                return;

            painting.framePosition = new Microsoft.Xna.Framework.Vector2(posX, posY);
            painting.scale = scale;

            // When a server gets this packet, it immediately sends an equivalent packet to all clients.
            if (Main.dedServ)
                Send(painting, posX, posY, scale, ignoreClient: sender);
        }
    }
}
