using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.TileEntities
{
    public abstract class TEHostileTargetingTurret : TEIndexedTargetingTurret
    {
        public int PlayerTargetIndex
        {
            get => TargetIndex;
            set => TargetIndex = value;
        }

        protected override Vector2 ChooseTarget(Vector2 targetingCenter)
        {
            // Store the value that will be set into PlayerTargetIndex here.
            // This local is used because changing the actual field will trigger a netcode auto-sync.
            int indexToSet = PlayerTargetIndex;

            // If a player is already targeted, make sure they still exist and aren't dead.
            if (indexToSet != -1)
            {
                Player p = Main.player[PlayerTargetIndex];

                // If they don't exist, are dead, or are using an invis pot then unmark them as a target and search for a new one.
                if (!p.active || p.dead || p.invis)
                    indexToSet = -1;
                else
                {
                    // If the existing target player is close enough, return their position. Otherwise unmark them as a target.
                    Vector2 playerPos = p.Center;
                    float distSQ = Vector2.DistanceSquared(targetingCenter, playerPos);
                    if (distSQ <= MaxRange * MaxRange)
                        return playerPos;
                    else
                        indexToSet = -1;
                }
            }

            // No valid target currently exists. Loop through players (efficiently!) to look for the closest new target.
            Vector2 ret = InvalidTarget;
            float distSQToBeat = MaxRange * MaxRange;

            foreach (Player p in Main.ActivePlayers)
            {
                if (p.dead || p.invis)
                    continue;

                float distSQ = p.DistanceSQ(targetingCenter);
                if (distSQ < distSQToBeat)
                {
                    distSQToBeat = distSQ;
                    ret = p.Center;
                    indexToSet = p.whoAmI;
                }
            }

            // Set PlayerTargetIndex now that we're sure what we're targeting. This triggers a netcode sync if the value is different.
            PlayerTargetIndex = indexToSet;
            return ret;
        }

        // Client-side tracking smooths visual rotation; firing and authoritative target choice remain unchanged.
        public override void UpdateClient()
        {
            if (PlayerTargetIndex != -1)
            {
                Player p = Main.player[PlayerTargetIndex];
                if (p.active && !p.dead)
                    TargetPos = p.Center;
            }
        }
    }
}
