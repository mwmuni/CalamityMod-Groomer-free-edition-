using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.TileEntities
{
    public abstract class TENPCTargetingTurret : TEIndexedTargetingTurret
    {
        public int NPCTargetIndex
        {
            get => TargetIndex;
            set => TargetIndex = value;
        }

        protected override Vector2 ChooseTarget(Vector2 targetingCenter)
        {
            if (CalamityUtils.AnyBossNPCS())
            {
                NPCTargetIndex = -1;
                return InvalidTarget;
            }

            int indexToSet = NPCTargetIndex;
            // If an NPC is already targeted, make sure they still exist and aren't dead.
            if (indexToSet != -1)
            {
                NPC npc = Main.npc[NPCTargetIndex];

                // If they don't exist, or are dead, then unmark them as a target and search for a new one.
                if (!npc.active)
                    indexToSet = -1;
                else
                {
                    // If the existing target is close enough, return its position. Otherwise unmark it as a target.
                    Vector2 npcPos = npc.Center;
                    float distSQ = Vector2.DistanceSquared(targetingCenter, npcPos);
                    if (distSQ <= MaxRange * MaxRange)
                        return npcPos;
                    else
                        indexToSet = -1;
                }
            }

            // No valid target currently exists. Loop through NPCs (efficiently!) to look for the closest new target.
            Vector2 ret = InvalidTarget;
            float distSQToBeat = MaxRange * MaxRange;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.CountsAsACritter || NPCID.Sets.CountsAsCritter[npc.type])
                    continue;

                float distSQ = npc.DistanceSQ(targetingCenter);
                if (distSQ < distSQToBeat)
                {
                    distSQToBeat = distSQ;
                    ret = npc.Center;
                    indexToSet = npc.whoAmI;
                }
            }

            // Set NPCTargetIndex now that we're sure what we're targeting. This triggers a netcode sync if the value is different.
            NPCTargetIndex = indexToSet;
            return ret;
        }

        // Client-side tracking smooths visual rotation; firing and authoritative target choice remain unchanged.
        public override void UpdateClient()
        {
            if (NPCTargetIndex != -1)
            {
                NPC npc = Main.npc[NPCTargetIndex];
                if (npc.active)
                    TargetPos = npc.Center;
            }
        }
    }
}
