using CalamityMod.Projectiles.Turret;
using CalamityMod.Tiles.PlayerTurrets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public class TEPlayerPlagueTurret : TENPCTargetingTurret
    {
        // Tile which hosts this tile entity
        public override int TileType => ModContent.TileType<PlayerPlagueTurret>();
        public override int HostTileWidth => PlayerPlagueTurret.Width;
        public override int HostTileHeight => PlayerPlagueTurret.Height;

        // Projectile variables
        public override int ProjectileType => ModContent.ProjectileType<PlagueShotBuffer>();
        public override int ProjectileDamage => 100;
        public override float ProjectileKnockback => 6f;
        public override float ShootSpeed => 16f;
        public override int FiringStartupDelay => 50;
        public override int FiringUseTime => 50;

        // Projectile spawn location variables
        public override Vector2 TurretCenterOffset => new Vector2(22f + 4f * Direction, -2f);
        protected override float ShootForwardsOffset => 24f;

        // Targeting variables
        public override float MaxRange => 900f;
        protected override float MaxTargetAngleDeviance => MathHelper.ToRadians(50f);
        protected override float MaxDeltaAnglePerFrame => MathHelper.ToRadians(6f);
        protected override float CloseAimThreshold => MathHelper.ToRadians(12f);
        protected override float CloseAimLerpFactor => 0.08f;

        // Lab turrets specifically must be placed on the top left corner of their host tile.
        // This restriction is not shared by general turrets for flexibility reasons.
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == TileType && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        protected override Vector2 ChooseTarget(Vector2 targetingCenter)
        {
            if (CalamityUtils.AnyBossNPCS())
            {
                NPCTargetIndex = -1;
                return InvalidTarget;
            }

            int indexToSet = NPCTargetIndex;
            Vector2 ret = InvalidTarget;
            float distSQToBeat = MaxRange * MaxRange;


            //This turret can switch targets at any time
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

    }
}
