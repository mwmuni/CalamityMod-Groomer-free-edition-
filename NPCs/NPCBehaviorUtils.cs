using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.NPCs
{
    internal static class NPCBehaviorUtils
    {
        // Retargeting, motion changes and synchronization stay with the caller to preserve their order.
        internal static void SetAIState(NPC npc, float state, float ai1 = 0f, float? ai2 = null)
        {
            npc.ai[0] = state;
            npc.ai[1] = ai1;
            if (ai2.HasValue)
                npc.ai[2] = ai2.Value;
        }

        internal static void TargetClosestIfInvalidOrTooFar(NPC npc, bool updateNetwork)
        {
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
                if (updateNetwork)
                {
                    npc.netUpdate = true;
                }
            }

            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
            {
                npc.TargetClosest();
                if (updateNetwork)
                {
                    npc.netUpdate = true;
                }
            }
        }

        internal static void TargetClosestIfInvalidOrTooFarFrom(NPC npc, Vector2 center, bool updateNetwork)
        {
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
                if (updateNetwork)
                {
                    npc.netUpdate = true;
                }
            }

            if (Vector2.Distance(Main.player[npc.target].Center, center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
            {
                npc.TargetClosest();
                if (updateNetwork)
                {
                    npc.netUpdate = true;
                }
            }
        }
    }
}
