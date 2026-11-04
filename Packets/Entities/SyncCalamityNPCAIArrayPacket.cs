using System.IO;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class SyncCalamityNPCAIArrayPacket : CalamityPacket
    {
        // MIGRATED COMMENTS FROM: 'CalamityNetcode.cs'
        // - This code has been edited to fail gracefully when trying to provide data for an invalid NPC.

        public static SyncCalamityNPCAIArrayPacket Instance { get; private set; }

        public static void Send(NPC npc, int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode != NetmodeID.Server || npc is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(npc);

            var calNPC = npc.Calamity();
            packet.Write(calNPC.newAI[0]);
            packet.Write(calNPC.newAI[1]);
            packet.Write(calNPC.newAI[2]);
            packet.Write(calNPC.newAI[3]);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                CalamityMod.Log.Warn("Ignored a non-server Calamity NPC AI snapshot.");
                return;
            }

            var npc = packet.ReadNPC();
            var ai0 = packet.ReadSingle();
            var ai1 = packet.ReadSingle();
            var ai2 = packet.ReadSingle();
            var ai3 = packet.ReadSingle();

            if (npc is null)
                return;

            var calNPC = npc.Calamity();
            calNPC.newAI[0] = ai0;
            calNPC.newAI[1] = ai1;
            calNPC.newAI[2] = ai2;
            calNPC.newAI[3] = ai3;
        }
    }
}
