using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public sealed class CalamityNetImportantNPC : GlobalNPC
    {
        private static Dictionary<int, int> typesToUpdate;

        public override void Load()
        {
            typesToUpdate = new();
        }

        public override void Unload()
        {
            typesToUpdate?.Clear();
            typesToUpdate = null;
        }

        public override void SetStaticDefaults()
        {
            int uniqueNetOffsetID = 0;

            #region Vanilla Enemies
            MarkNPCToLongDistanceSync(NPCID.EaterofWorldsHead, uniqueNetOffsetID);
            MarkNPCToLongDistanceSync(NPCID.EaterofWorldsBody, uniqueNetOffsetID);
            MarkNPCToLongDistanceSync(NPCID.EaterofWorldsTail, uniqueNetOffsetID);
            uniqueNetOffsetID++;

            MarkNPCToLongDistanceSync(NPCID.TheDestroyer, uniqueNetOffsetID);
            MarkNPCToLongDistanceSync(NPCID.TheDestroyerBody, uniqueNetOffsetID);
            MarkNPCToLongDistanceSync(NPCID.TheDestroyerTail, uniqueNetOffsetID);
            uniqueNetOffsetID++;
            #endregion Vanilla Enemies

            var npcs = ModContent.GetContent<ModNPC>();
            var netOffsetTable = new Dictionary<Type, int>();
            foreach (var npc in npcs)
            {
                try
                {
                    var type = npc.GetType();
                    var longDistSync = type.GetCustomAttribute<LongDistanceNetSyncAttribute>();

                    if (longDistSync == null)
                        continue;

                    int npcType = npc.Type;
                    int netOffset = uniqueNetOffsetID;

                    Type typeToCheck = longDistSync.SyncWith ?? type;
                    if (netOffsetTable.TryGetValue(typeToCheck, out int savedUniqueID))
                    {
                        netOffset = savedUniqueID;
                    }
                    else
                    {
                        netOffsetTable[typeToCheck] = netOffset;
                        uniqueNetOffsetID++;
                    }

                    MarkNPCToLongDistanceSync(npcType, netOffset);
                }
                catch (Exception e)
                {
                    CalamityMod.Log.Error($"Exception thrown while evaluating type \"{npc.FullName}\": {e}");
                }
            }

            netOffsetTable?.Clear();
        }

        public override void PostAI(NPC npc)
        {
            // Only Server should update this!
            if (Main.netMode != NetmodeID.Server)
                return;

            // Obviously deactived npc is not on our interest (not sure if this is case though)
            if (!npc.active)
                return;

            if (!typesToUpdate.TryGetValue(npc.type, out var netUpdateTickOffset))
                return;

            if ((Main.GameUpdateCount + netUpdateTickOffset) % 45 != 0)
                return;

            foreach (var player in Main.ActivePlayers)
            {
                // distance between 1000~1500 update with 8 tick period
                // and distance over 1500 will never update
                // So we forcely update NPC distanced over 1500 with 45 tick period
                float distance = CalamityUtils.ManhattanDistance(player.position, npc.position);
                if (distance <= 1499.0f)
                    continue;

                Packets.SyncNPCPosAndRotOnlyPacket.Send(npc, toClient: player.whoAmI);
            }
        }

        private static void MarkNPCToLongDistanceSync<NPCType>(int netUpdateTickOffset = 0) where NPCType : ModNPC
        {
            MarkNPCToLongDistanceSync(ModContent.NPCType<NPCType>(), netUpdateTickOffset);
        }

        private static void MarkNPCToLongDistanceSync(int npcType, int netUpdateTickOffset = 0)
        {
            typesToUpdate[npcType] = netUpdateTickOffset;
        }
    }
}
