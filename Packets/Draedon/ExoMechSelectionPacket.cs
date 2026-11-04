using System.IO;
using CalamityMod.World;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class ExoMechSelectionPacket : CalamityPacket
    {
        public static ExoMechSelectionPacket Instance { get; private set; }

        public static void Send(int toClient = -1, int ignoreClient = -1)
        {
            var packet = Instance.CreateBasePacket();
            packet.Write((int)CalamityWorld.DraedonMechToSummon);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            CalamityWorld.DraedonMechToSummon = (ExoMech)packet.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
                Send(ignoreClient: sender);
        }
    }
}
