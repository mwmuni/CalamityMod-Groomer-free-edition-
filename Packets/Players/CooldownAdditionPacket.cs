using System.IO;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class CooldownAdditionPacket : CalamityPacket
    {
        public static CooldownAdditionPacket Instance { get; private set; }

        public static void Send(CalamityPlayer player, CooldownInstance cd, int toClient = -1, int ignoreClient = -1)
        {
            Send(player, cd.netID, cd.duration, cd.timeLeft, toClient, ignoreClient);
        }

        public static void Send(CalamityPlayer player, ushort netID, int duration, int timeLeft, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write(netID);
            packet.Write(duration);
            packet.Write(timeLeft);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadCalamityPlayer();
            var netID = packet.ReadUInt16();
            var duration = packet.ReadInt32();
            var timeLeft = packet.ReadInt32();

            if (player is null)
                return;

            if (Main.netMode == NetmodeID.Server && player.Player.whoAmI != sender)
            {
                CalamityMod.Log.Warn($"Ignored cooldown addition sent by client {sender} for player {player.Player.whoAmI}.");
                return;
            }

            if (netID >= CooldownRegistry.registry.Length || CooldownRegistry.registry[netID] is null)
                throw new InvalidDataException($"Unknown cooldown ID: {netID}.");

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                CooldownInstance instance = new CooldownInstance(player.Player, netID, duration, timeLeft);

                // Actually assign this freshly synced cooldown to the appropriate player.
                string id = CooldownRegistry.registry[instance.netID].ID;
                player.cooldowns[id] = instance;
            }
            else if (Main.dedServ)
            {
                // The server validates IDs but leaves cooldown handler execution to clients.
                Send(player, netID, duration, timeLeft, ignoreClient: sender);
            }
        }
    }
}
