using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public abstract class OarfishSegmentBase : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.OarfishHead.DisplayName");

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            // Avoid cheap bullshit
            NPC.damage = 0;

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Check if other segments are still alive, if not, die
            bool shouldDespawn = true;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type == ModContent.NPCType<OarfishHead>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
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

            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            Vector2 segmentPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float targetXDirection = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float targetYDirection = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            targetXDirection = (float)((int)(targetXDirection / 16f) * 16);
            targetYDirection = (float)((int)(targetYDirection / 16f) * 16);
            segmentPosition.X = (float)((int)(segmentPosition.X / 16f) * 16);
            segmentPosition.Y = (float)((int)(segmentPosition.Y / 16f) * 16);
            targetXDirection -= segmentPosition.X;
            targetYDirection -= segmentPosition.Y;
            float targetDistance = (float)System.Math.Sqrt((double)(targetXDirection * targetXDirection + targetYDirection * targetYDirection));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    segmentPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    targetXDirection = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - segmentPosition.X;
                    targetYDirection = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - segmentPosition.Y;
                }
                catch
                {
                }
                NPC.rotation = (float)System.Math.Atan2((double)targetYDirection, (double)targetXDirection) + 1.57f;
                targetDistance = (float)System.Math.Sqrt((double)(targetXDirection * targetXDirection + targetYDirection * targetYDirection));
                int segmentWidth = NPC.width;
                targetDistance = (targetDistance - (float)segmentWidth) / targetDistance;
                targetXDirection *= targetDistance;
                targetYDirection *= targetDistance;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + targetXDirection;
                NPC.position.Y = NPC.position.Y + targetYDirection;

                if (targetXDirection < 0f)
                    NPC.spriteDirection = -1;
                else if (targetXDirection > 0f)
                    NPC.spriteDirection = 1;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
