using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using Terraria;
using Terraria.ID;

using CooldownInfoTuple = (ushort netID, int duration, int timeLeft);

namespace CalamityMod.Packets
{
    internal sealed class SyncCooldownDictionaryPacket : CalamityPacket
    {
        public static SyncCooldownDictionaryPacket Instance { get; private set; }

        public static void Send(CalamityPlayer player, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var cooldowns = player.cooldowns.Values.Select(cd => (cd.netID, cd.duration, cd.timeLeft));
            Send(player, cooldowns, toClient, ignoreClient);
        }

        public static void Send(CalamityPlayer player, IEnumerable<CooldownInfoTuple> cooldowns, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            if (cooldowns is null)
                return;

            var snapshot = cooldowns.ToArray();
            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write(snapshot.Length);
            foreach (CooldownInfoTuple cd in snapshot)
            {
                packet.Write(cd.netID);
                packet.Write(cd.duration);
                packet.Write(cd.timeLeft);
            }
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadCalamityPlayer();
            int count = packet.ReadInt32();
            if (count < 0 || count > ushort.MaxValue + 1 || count > (packet.BaseStream.Length - packet.BaseStream.Position) / 10)
                throw new InvalidDataException($"Invalid cooldown snapshot count: {count}.");

            Dictionary<ushort, CooldownInfoTuple> receivedCooldowns = new(count);
            for (int i = 0; i < count; ++i)
            {
                var netID = packet.ReadUInt16();
                var duration = packet.ReadInt32();
                var timeLeft = packet.ReadInt32();
                if (netID >= CooldownRegistry.registry.Length || CooldownRegistry.registry[netID] is null)
                    throw new InvalidDataException($"Unknown cooldown ID: {netID}.");

                receivedCooldowns[netID] = (netID, duration, timeLeft);
            }

            if (player is null)
                return;

            if (Main.netMode == NetmodeID.Server && player.Player.whoAmI != sender)
            {
                CalamityMod.Log.Warn($"Ignored cooldown snapshot sent by client {sender} for player {player.Player.whoAmI}.");
                return;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                List<string> removedIDs = [];
                foreach (var local in player.cooldowns)
                {
                    if (!receivedCooldowns.ContainsKey(local.Value.netID))
                        removedIDs.Add(local.Key);
                }
                foreach (string id in removedIDs)
                    player.cooldowns.Remove(id);

                foreach (var received in receivedCooldowns.Values)
                {
                    string id = CooldownRegistry.registry[received.netID].ID;
                    if (player.cooldowns.TryGetValue(id, out var local))
                    {
                        local.duration = received.duration;
                        local.timeLeft = received.timeLeft;
                    }
                    else
                    {
                        player.cooldowns[id] = new CooldownInstance(player.Player, received.netID, received.duration, received.timeLeft);
                    }
                }
            }
            else if (Main.dedServ)
            {
                // The server validates IDs but leaves cooldown handler execution to clients.
                Send(player, receivedCooldowns.Values, ignoreClient: sender);
            }
        }
    }
}
