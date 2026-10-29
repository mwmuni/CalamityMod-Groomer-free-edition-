using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public abstract class HostileFlyingEnemyBase : ModNPC
    {
        protected abstract bool HasBeenHit { get; set; }
        protected abstract float IdleHorizontalSpeedThreshold { get; }
        protected abstract float FrameIncrementAfterHit { get; }
        protected abstract float FrameIncrementBeforeHit { get; }

        protected abstract void UpdateAttackMovement();

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HasBeenHit);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HasBeenHit = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            NPC.noGravity = true;
            if (NPC.direction == 0)
            {
                NPC.TargetClosest(true);
            }
            if (NPC.justHit)
            {
                HasBeenHit = true;
            }
            NPC.chaseable = HasBeenHit;
            if (!NPC.wet)
            {
                bool canAttack = HasBeenHit;
                NPC.TargetClosest(false);
                if ((Main.player[NPC.target].wet || Main.player[NPC.target].dead) && canAttack)
                {
                    canAttack = false;
                }
                if (!canAttack)
                {
                    if (NPC.collideX)
                    {
                        NPC.velocity.X = NPC.velocity.X * -1f;
                        NPC.direction *= -1;
                        NPC.netUpdate = true;
                    }
                    if (NPC.collideY)
                    {
                        NPC.netUpdate = true;
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = System.Math.Abs(NPC.velocity.Y) * -1f;
                            NPC.directionY = -1;
                            NPC.ai[0] = -1f;
                        }
                        else if (NPC.velocity.Y < 0f)
                        {
                            NPC.velocity.Y = System.Math.Abs(NPC.velocity.Y);
                            NPC.directionY = 1;
                            NPC.ai[0] = 1f;
                        }
                    }
                }
                if (canAttack)
                {
                    NPC.TargetClosest(true);
                    UpdateAttackMovement();
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.1f;
                    if (NPC.velocity.X < -IdleHorizontalSpeedThreshold || NPC.velocity.X > IdleHorizontalSpeedThreshold)
                    {
                        NPC.velocity.X = NPC.velocity.X * 0.95f;
                    }
                    if (NPC.ai[0] == -1f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - 0.01f;
                        if ((double)NPC.velocity.Y < -0.3)
                        {
                            NPC.ai[0] = 1f;
                        }
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y + 0.01f;
                        if ((double)NPC.velocity.Y > 0.3)
                        {
                            NPC.ai[0] = -1f;
                        }
                    }
                }
                int tileCheckX = (int)(NPC.position.X + (float)(NPC.width / 2)) / 16;
                int tileCheckY = (int)(NPC.position.Y + (float)(NPC.height / 2)) / 16;
                if (Main.tile[tileCheckX, tileCheckY - 1].LiquidAmount < 128) //problem?
                {
                    if (Main.tile[tileCheckX, tileCheckY + 1].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                    else if (Main.tile[tileCheckX, tileCheckY + 2].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                if ((double)NPC.velocity.Y > 0.4 || (double)NPC.velocity.Y < -0.4)
                {
                    NPC.velocity.Y = NPC.velocity.Y * 0.95f;
                }
            }
            else
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.94f;
                    if ((double)NPC.velocity.X > -0.2 && (double)NPC.velocity.X < 0.2)
                    {
                        NPC.velocity.X = 0f;
                    }
                }
                NPC.velocity.Y = NPC.velocity.Y + 0.3f;
                if (NPC.velocity.Y > 5f)
                {
                    NPC.velocity.Y = 5f;
                }
                NPC.ai[0] = 1f;
            }
            NPC.rotation = NPC.velocity.Y * (float)NPC.direction * 0.1f;
            if ((double)NPC.rotation < -0.1)
            {
                NPC.rotation = -0.1f;
            }
            if ((double)NPC.rotation > 0.1)
            {
                NPC.rotation = 0.1f;
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return HasBeenHit;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += HasBeenHit ? FrameIncrementAfterHit : FrameIncrementBeforeHit;
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
    }
}
