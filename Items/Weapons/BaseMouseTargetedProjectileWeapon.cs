using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public abstract class BaseMouseTargetedProjectileWeapon : ModItem
    {
        protected Vector2 GetRandomizedSpawnPosition(
            Player player,
            int projectileIndex,
            int directionalRandomMaxExclusive = 201,
            int horizontalSpread = 200,
            bool spawnBelowPlayer = false,
            int verticalStep = -100)
        {
            Vector2 spawnPosition = new Vector2(
                player.position.X + (float)player.width * 0.5f
                    + (float)(Main.rand.Next(directionalRandomMaxExclusive) * -(float)player.direction)
                    + ((float)Main.mouseX + Main.screenPosition.X - player.position.X),
                spawnBelowPlayer ? player.MountedCenter.Y + 600f : player.MountedCenter.Y - 600f);
            spawnPosition.X = (spawnPosition.X + player.Center.X) / 2f + (float)Main.rand.Next(-horizontalSpread, horizontalSpread + 1);
            if (projectileIndex != 0 && verticalStep != 0)
            {
                if (verticalStep < 0)
                {
                    spawnPosition.Y -= (float)(-verticalStep * projectileIndex);
                }
                else
                {
                    spawnPosition.Y += (float)(verticalStep * projectileIndex);
                }
            }

            return spawnPosition;
        }

        protected Vector2 GetMouseTargetedVelocity(
            Vector2 spawnPosition,
            float projectileSpeed,
            int randomizedAimXRange = 0,
            float randomizedAimXScale = 0f)
        {
            float mouseXDistance = (float)Main.mouseX + Main.screenPosition.X - spawnPosition.X;
            if (randomizedAimXRange > 0)
            {
                mouseXDistance += (float)Main.rand.Next(-randomizedAimXRange, randomizedAimXRange + 1) * randomizedAimXScale;
            }

            float mouseYDistance = (float)Main.mouseY + Main.screenPosition.Y - spawnPosition.Y;
            if (mouseYDistance < 0f)
            {
                mouseYDistance *= -1f;
            }
            if (mouseYDistance < 20f)
            {
                mouseYDistance = 20f;
            }

            float distance = (float)Math.Sqrt((double)(mouseXDistance * mouseXDistance + mouseYDistance * mouseYDistance));
            float speedMultiplier = projectileSpeed / distance;
            return new Vector2(mouseXDistance * speedMultiplier, mouseYDistance * speedMultiplier);
        }
    }
}
