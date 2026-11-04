using System.IO;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class SyncVanillaNPCLocalAIArrayPacket : CalamityPacket
    {
        public static SyncVanillaNPCLocalAIArrayPacket Instance { get; private set; }

        public static void Send(NPC npc, int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode != NetmodeID.Server || npc is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(npc);
            packet.Write(npc.localAI[0]);
            packet.Write(npc.localAI[1]);
            packet.Write(npc.localAI[2]);
            packet.Write(npc.localAI[3]);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                CalamityMod.Log.Warn("Ignored a non-server vanilla NPC local AI snapshot.");
                return;
            }

            var npc = packet.ReadNPC();
            var ai0 = packet.ReadSingle();
            var ai1 = packet.ReadSingle();
            var ai2 = packet.ReadSingle();
            var ai3 = packet.ReadSingle();

            if (npc is null)
                return;

            npc.localAI[0] = ai0;
            npc.localAI[1] = ai1;
            npc.localAI[2] = ai2;
            npc.localAI[3] = ai3;
        }
    }
}
