using System.IO;
using CalamityMod.World;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class CodebreakerSummonStuffPacket : CalamityPacket
    {
        public static CodebreakerSummonStuffPacket Instance { get; private set; }

        public static void Send(int toClient = -1, int ignoreClient = -1)
        {
            var packet = Instance.CreateBasePacket();
            packet.Write(CalamityWorld.DraedonSummonCountdown);
            packet.WriteVector2(CalamityWorld.DraedonSummonPosition);
            packet.Write(CalamityWorld.DraedonMechdusa);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            CalamityWorld.DraedonSummonCountdown = packet.ReadInt32();
            CalamityWorld.DraedonSummonPosition = packet.ReadVector2();
            CalamityWorld.DraedonMechdusa = packet.ReadBoolean();

            if (Main.netMode == NetmodeID.Server)
                Send(ignoreClient: sender);
        }
    }
}
