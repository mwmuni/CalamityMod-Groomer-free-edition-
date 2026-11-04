using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Debugging;
using CalamityMod.Packets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    public class CalamityNetcode : ModSystem
    {
        private static readonly List<CalamityPacket> _PacketHandlers = [];
        private static bool _worldSyncPending;

        internal static ushort RegisterHandler(CalamityPacket handler)
        {
            if (_PacketHandlers.Count > ushort.MaxValue)
                throw new InvalidOperationException("Too many Calamity packet handlers registered.");

            var id = (ushort)_PacketHandlers.Count;
            _PacketHandlers.Add(handler);
            return id;
        }

        internal static string[] GetPacketHandlerNames()
        {
            string[] names = new string[_PacketHandlers.Count];
            for (int i = 0; i < names.Length; i++)
                names[i] = _PacketHandlers[i].GetType().FullName;
            return names;
        }

        internal static void WriteHandlerNetID(BinaryWriter packet, ushort netID)
        {
            if (_PacketHandlers.Count > 256)
                packet.Write(netID);
            else
                packet.Write((byte)netID);
        }

        internal static ushort ReadHandlerNetID(BinaryReader packet)
        {
            if (_PacketHandlers.Count > 256)
                return packet.ReadUInt16();
            else
                return packet.ReadByte();
        }

        public override void ClearWorld() => _worldSyncPending = false;

        public override void OnWorldUnload() => _worldSyncPending = false;

        public override void OnModUnload()
        {
            _PacketHandlers.Clear();
            _worldSyncPending = false;
        }

        public override void PostUpdateEverything()
        {
            if (!_worldSyncPending)
                return;

            _worldSyncPending = false;
            if (Main.dedServ)
                NetMessage.SendData(MessageID.WorldData);
        }

        public static void HandlePacket(Mod mod, BinaryReader reader, int whoAmI)
        {
            using var measurement = PerformanceRecorder.Measure(PerformanceMetric.PacketReceive);
            int handlerID = -1;
            bool threwException = false;
            try
            {
                var netID = ReadHandlerNetID(reader);
                handlerID = netID;
                if (netID >= _PacketHandlers.Count)
                    throw new InvalidDataException($"No Calamity packet exists with ID {netID}.");

                var packetHandler = _PacketHandlers[netID];
                if (packetHandler is not null)
                {
                    packetHandler.HandlePacket(reader, whoAmI);
                }
                else
                {
                    //
                    // Default case: with no idea how long the packet is, we can't safely read data.
                    // Throw an exception now instead of allowing the network stream to corrupt.
                    //

                    CalamityMod.Log.Error($"Failed to parse Calamity packet: No Calamity packet exists with ID {netID}.");
                    throw new Exception("Failed to parse Calamity packet: Invalid Calamity packet ID.");
                }
            }
            catch (Exception e)
            {
                threwException = true;
                if (e is InvalidDataException ide)
                    CalamityMod.Log.Error("Failed to parse Calamity packet: Invalid packet data.", ide);
                else if (e is EndOfStreamException eose)
                    CalamityMod.Log.Error("Failed to parse Calamity packet: Packet was too short, missing data, or otherwise corrupt.", eose);
                else if (e is ObjectDisposedException ode)
                    CalamityMod.Log.Error("Failed to parse Calamity packet: Packet reader disposed or destroyed.", ode);
                else if (e is IOException ioe)
                    CalamityMod.Log.Error("Failed to parse Calamity packet: An unknown I/O error occurred.", ioe);
                else
                    throw; // this either will crash the game or be caught by TML's packet policing
            }
            finally
            {
                measurement.RecordReceivedPacket(handlerID, threwException);
            }
        }

        public static void SyncWorld()
        {
            if (Main.dedServ)
                _worldSyncPending = true;
        }

        /// <summary>
        /// Shorthand Method for SyncNPC
        /// <code>
        /// This Equals to:
        /// 
        /// if (Main.dedServ and npc != null)
        ///     NetMessage.SendData(MessageID.SyncNPC, ...)
        /// </code>
        /// </summary>
        public static void SyncNPC(NPC npcToSync, int toClient = -1, int ignoreClient = -1)
        {
            if (!Main.dedServ)
                return;

            if (npcToSync is null)
                return;

            var npcWhoAmI = npcToSync.whoAmI;
            if (npcWhoAmI < 0 || npcWhoAmI >= Main.maxNPCs)
                return;

            NetMessage.SendData(MessageID.SyncNPC, toClient, ignoreClient, null, npcWhoAmI);
        }

        /// <summary>
        /// Shorthand Method for SyncNPC
        /// <code>
        /// This Equals to:
        /// 
        /// if (Main.dedServ and npcWhoAmI in valid range)
        ///     NetMessage.SendData(MessageID.SyncNPC, ...)
        /// </code>
        /// </summary>
        public static void SyncNPC(int npcWhoAmI, int toClient = -1, int ignoreClient = -1)
        {
            if (!Main.dedServ)
                return;

            if (npcWhoAmI < 0 || npcWhoAmI >= Main.maxNPCs)
                return;

            NetMessage.SendData(MessageID.SyncNPC, toClient, ignoreClient, null, npcWhoAmI);
        }

        public static void NewNPC_ClientSide(Vector2 spawnPosition, int npcType, Player player)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                NPC.NewNPC(new EntitySource_WorldEvent(), (int)spawnPosition.X, (int)spawnPosition.Y, npcType, Target: player.whoAmI);
                return;
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SpawnNPCOnPlayerPacket.Send(player, (int)spawnPosition.X, (int)spawnPosition.Y, npcType);
            }
        }
    }
}
