using System;
using System.IO;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class SyncNPCPosAndRotOnlyPacket : CalamityPacket
    {
        public static SyncNPCPosAndRotOnlyPacket Instance { get; private set; }

        public static void Send(NPC npc, int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode != NetmodeID.Server || npc is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(npc);
            packet.WriteVector2(npc.position);
            packet.Write((Half)npc.rotation); //rotation unit is radian (-π/2 ≤ rotation ≤ π/2) so Half precision should works
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                CalamityMod.Log.Warn("Ignored a non-server NPC position snapshot.");
                return;
            }

            var npc = packet.ReadNPC();
            var position = packet.ReadVector2();
            var rotation = (float)packet.ReadHalf();

            if (npc is null)
                return;

            npc.position = position;
            npc.rotation = rotation;

        }
    }
}
