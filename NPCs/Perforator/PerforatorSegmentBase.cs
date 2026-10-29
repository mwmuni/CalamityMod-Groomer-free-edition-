using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
    public abstract class PerforatorSegmentBase<THead> : ModNPC where THead : ModNPC
    {
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(true);

            bool shouldDespawn = !NPC.AnyNPCs(ModContent.NPCType<THead>());
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            if (Main.player[NPC.target].dead)
                NPC.TargetClosest(false);

            Vector2 segmentPosition = NPC.Center;
            float targetX = Main.player[NPC.target].Center.X;
            float targetY = Main.player[NPC.target].Center.Y;
            targetX = (float)((int)(targetX / 16f) * 16);
            targetY = (float)((int)(targetY / 16f) * 16);
            segmentPosition.X = (float)((int)(segmentPosition.X / 16f) * 16);
            segmentPosition.Y = (float)((int)(segmentPosition.Y / 16f) * 16);
            targetX -= segmentPosition.X;
            targetY -= segmentPosition.Y;
            float targetDistance = (float)System.Math.Sqrt((double)(targetX * targetX + targetY * targetY));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    segmentPosition = NPC.Center;
                    targetX = Main.npc[(int)NPC.ai[1]].Center.X - segmentPosition.X;
                    targetY = Main.npc[(int)NPC.ai[1]].Center.Y - segmentPosition.Y;
                }
                catch
                {
                }
                NPC.rotation = (float)System.Math.Atan2((double)targetY, (double)targetX) + MathHelper.PiOver2;
                targetDistance = (float)System.Math.Sqrt((double)(targetX * targetX + targetY * targetY));
                int npcWidth = NPC.width;
                targetDistance = (targetDistance - (float)npcWidth) / targetDistance;
                targetX *= targetDistance;
                targetY *= targetDistance;
                NPC.velocity = Vector2.Zero;
                NPC.position.X += targetX;
                NPC.position.Y += targetY;

                if (targetX < 0f)
                    NPC.spriteDirection = 1;
                else if (targetX > 0f)
                    NPC.spriteDirection = -1;
            }

            if (Main.npc[(int)NPC.ai[1]].alpha >= 85)
            {
                if (NPC.alpha > 0 && NPC.life > 0)
                {
                    for (int dustIndex = 0; dustIndex < 2; dustIndex++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].noLight = true;
                    }
                }

                if ((NPC.position - NPC.oldPosition).Length() > 2f)
                {
                    NPC.alpha -= 42;
                    if (NPC.alpha < 0)
                        NPC.alpha = 0;
                }
            }
            else if (NPC.alpha > 0)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
