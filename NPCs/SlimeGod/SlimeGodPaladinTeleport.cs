using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.NPCs.SlimeGod
{
    internal static class SlimeGodPaladinTeleport
    {
        internal static Vector2 FindDestination(NPC npc, Player player, float distanceAhead)
        {
            Vector2 randomDefault = Main.rand.NextBool() ? Vector2.UnitX : -Vector2.UnitX;
            Vector2 vectorAimedAheadOfTarget = player.Center + new Vector2((float)Math.Round(player.velocity.X), 0f).SafeNormalize(randomDefault) * distanceAhead;
            Point predictiveTeleportPoint = vectorAimedAheadOfTarget.ToTileCoordinates();
            int randomPredictiveTeleportOffset = 5;
            int teleportTries = 0;
            Vector2 teleportDestination = Vector2.Zero;
            while (teleportTries < 100)
            {
                teleportTries++;
                int teleportTileX = Main.rand.Next(predictiveTeleportPoint.X - randomPredictiveTeleportOffset, predictiveTeleportPoint.X + randomPredictiveTeleportOffset + 1);
                int teleportTileY = Main.rand.Next(predictiveTeleportPoint.Y - randomPredictiveTeleportOffset, predictiveTeleportPoint.Y);

                if (!Main.tile[teleportTileX, teleportTileY].HasUnactuatedTile)
                {
                    bool canTeleportToTile = true;
                    if (canTeleportToTile && Main.tile[teleportTileX, teleportTileY].LiquidType == LiquidID.Lava)
                        canTeleportToTile = false;
                    if (canTeleportToTile && !Collision.CanHitLine(npc.Center, 0, 0, predictiveTeleportPoint.ToVector2() * 16, 0, 0))
                        canTeleportToTile = false;

                    if (canTeleportToTile)
                    {
                        teleportDestination = new Vector2(teleportTileX * 16 + 8, teleportTileY * 16 + 16);
                        break;
                    }
                    else
                        predictiveTeleportPoint.X += predictiveTeleportPoint.X < 0f ? 1 : -1;
                }
                else
                    predictiveTeleportPoint.X += predictiveTeleportPoint.X < 0f ? 1 : -1;
            }

            // Preserve the existing timeout behavior, including when the final attempt finds a valid tile.
            if (teleportTries >= 100)
                teleportDestination = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].Bottom;

            return teleportDestination;
        }
    }
}
