using System.IO;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class RequestCooldownSyncPacket : CalamityPacket
    {
        public static RequestCooldownSyncPacket Instance { get; private set; }

        public static void Send(int toClient)
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            var packet = Instance.CreateBasePacket();
            packet.Send(toClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                CalamityMod.Log.Warn("Ignored a non-server cooldown synchronization request.");
                return;
            }

            Main.LocalPlayer.Calamity().SyncCooldownDictionary(false);
        }
    }
}
