using System.IO;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class CombatStatsSyncPacket : CalamityPacket
    {
        public static CombatStatsSyncPacket Instance { get; private set; }

        public static void Send(CalamityPlayer player, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write(player.rage);
            packet.Write(player.rageCombatFrames);
            packet.Write(player.adrenaline);
            packet.Write(player.totalDefenseDamage);
            packet.Write(player.defenseDamageRecoveryFrames);
            packet.Write(player.totalDefenseDamageRecoveryFrames);
            packet.Write(player.defenseDamageDelayFrames);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadCalamityPlayer();
            var rage = packet.ReadSingle();
            var rageCombatFrames = packet.ReadInt32();
            var adrenaline = packet.ReadSingle();
            var totalDefenseDamage = packet.ReadInt32();
            var defenseDamageRecoveryFrames = packet.ReadInt32();
            var totalDefenseDamageRecoveryFrames = packet.ReadInt32();
            var defenseDamageDelayFrames = packet.ReadInt32();

            if (player is null)
                return;

            if (Main.netMode == NetmodeID.Server && player.Player.whoAmI != sender)
            {
                CalamityMod.Log.Warn($"Ignored combat stats sent by client {sender} for player {player.Player.whoAmI}.");
                return;
            }

            player.rage = rage;
            player.rageCombatFrames = rageCombatFrames;
            player.adrenaline = adrenaline;
            player.totalDefenseDamage = totalDefenseDamage;
            player.defenseDamageRecoveryFrames = defenseDamageRecoveryFrames;
            player.totalDefenseDamageRecoveryFrames = totalDefenseDamageRecoveryFrames;
            player.defenseDamageDelayFrames = defenseDamageDelayFrames;

            if (Main.netMode == NetmodeID.Server)
                Send(player, ignoreClient: sender);
        }
    }
}
