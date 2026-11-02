using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.NPCs
{
    internal static class NPCBehaviorUtils
    {
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
