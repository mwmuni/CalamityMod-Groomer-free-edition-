using System.IO;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal sealed class CooldownRemovalPacket : CalamityPacket
    {
        public static CooldownRemovalPacket Instance { get; private set; }

        public static void Send(CalamityPlayer player, ushort[] netIDsToRemove, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write(netIDsToRemove.Length);
            for (int i = 0; i < netIDsToRemove.Length; i++)
            {
                packet.Write(netIDsToRemove[i]);
            }
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadCalamityPlayer();
            var count = packet.ReadInt32();
            if (count < 0 || count > ushort.MaxValue + 1 || count > (packet.BaseStream.Length - packet.BaseStream.Position) / sizeof(ushort))
                throw new InvalidDataException($"Invalid cooldown removal count: {count}.");

            var netIDsToRemove = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                netIDsToRemove[i] = packet.ReadUInt16();
                if (netIDsToRemove[i] >= CooldownRegistry.registry.Length || CooldownRegistry.registry[netIDsToRemove[i]] is null)
                    throw new InvalidDataException($"Unknown cooldown ID: {netIDsToRemove[i]}.");
            }

            if (player is null)
                return;

            if (Main.netMode == NetmodeID.Server && player.Player.whoAmI != sender)
            {
                CalamityMod.Log.Warn($"Ignored cooldown removal sent by client {sender} for player {player.Player.whoAmI}.");
                return;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                foreach (var netID in netIDsToRemove)
                {
                    player.cooldowns.Remove(CooldownRegistry.registry[netID].ID);
                }
            }
            else if (Main.dedServ)
            {
                // The server validates IDs but leaves cooldown handler execution to clients.
                Send(player, netIDsToRemove, ignoreClient: sender);
            }
        }
    }
}
