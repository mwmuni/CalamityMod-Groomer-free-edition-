using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Crags
{
    public abstract class ScryllarBase : ModNPC
    {
        protected virtual float MaxHorizontalVelocity => 4f;

        public override void AI()
        {
            NPC.rotation = NPC.velocity.X * 0.04f;
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            bool hoverDownDistCheck = false;
            if (NPC.justHit)
            {
                NPC.ai[2] = 0f;
            }
            if (NPC.ai[2] >= 0f)
            {
                int hoverDistance = 16;
                bool changeDirectionX = false;
                bool changeDirectionY = false;
                if (NPC.position.X > NPC.ai[0] - (float)hoverDistance && NPC.position.X < NPC.ai[0] + (float)hoverDistance)
                {
                    changeDirectionX = true;
                }
                else if ((NPC.velocity.X < 0f && NPC.direction > 0) || (NPC.velocity.X > 0f && NPC.direction < 0))
                {
                    changeDirectionX = true;
                }
                hoverDistance += 24;
                if (NPC.position.Y > NPC.ai[1] - (float)hoverDistance && NPC.position.Y < NPC.ai[1] + (float)hoverDistance)
                {
                    changeDirectionY = true;
                }
                if (changeDirectionX && changeDirectionY)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 30f && hoverDistance == 16)
                    {
                        hoverDownDistCheck = true;
                    }
                    if (NPC.ai[2] >= 60f)
                    {
                        NPC.ai[2] = -200f;
                        NPC.direction *= -1;
                        NPC.velocity.X = NPC.velocity.X * -1f;
                        NPC.collideX = false;
                    }
                }
                else
                {
                    NPC.ai[0] = NPC.position.X;
                    NPC.ai[1] = NPC.position.Y;
                    NPC.ai[2] = 0f;
                }
                NPC.TargetClosest(true);
            }
            else
            {
                NPC.TargetClosest(true);
                NPC.ai[2] += 2f;
            }
            int npcTileX = (int)((NPC.position.X + (float)(NPC.width / 2)) / 16f) + NPC.direction * 2;
            int npcTileY = (int)((NPC.position.Y + (float)NPC.height) / 16f);
            bool hoverDownwards = true;
            int tileCheckLoopAmt = 3;
            for (int loopInc1 = npcTileY; loopInc1 < npcTileY + tileCheckLoopAmt; loopInc1++)
            {
                if ((Main.tile[npcTileX, loopInc1].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[npcTileX, loopInc1].TileType]) || Main.tile[npcTileX, loopInc1].LiquidAmount > 0)
                {
                    hoverDownwards = false;
                    break;
                }
            }
            if (Main.player[NPC.target].npcTypeNoAggro[Type])
            {
                bool inTileNoAggro = false;
                for (int loopInc2 = npcTileY; loopInc2 < npcTileY + tileCheckLoopAmt - 2; loopInc2++)
                {
                    if ((Main.tile[npcTileX, loopInc2].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[npcTileX, loopInc2].TileType]) || Main.tile[npcTileX, loopInc2].LiquidAmount > 0)
                    {
                        inTileNoAggro = true;
                        break;
                    }
                }
                NPC.directionY = (!inTileNoAggro).ToDirectionInt();
            }
            if (hoverDownDistCheck)
            {
                hoverDownwards = true;
            }
            if (hoverDownwards)
            {
                NPC.velocity.Y = NPC.velocity.Y + 0.1f;
                if (NPC.velocity.Y > 3f)
                {
                    NPC.velocity.Y = 3f;
                }
            }
            else
            {
                if (NPC.directionY < 0 && NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.1f;
                }
                if (NPC.velocity.Y < -4f)
                {
                    NPC.velocity.Y = -4f;
                }
            }
            if (NPC.collideX)
            {
                NPC.velocity.X = NPC.oldVelocity.X * -0.4f;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 1f)
                {
                    NPC.velocity.X = 1f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -1f)
                {
                    NPC.velocity.X = -1f;
                }
            }
            if (NPC.collideY)
            {
                NPC.velocity.Y = NPC.oldVelocity.Y * -0.25f;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1f)
                {
                    NPC.velocity.Y = 1f;
                }
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1f)
                {
                    NPC.velocity.Y = -1f;
                }
            }
            float maxXVelocity = MaxHorizontalVelocity;
            if (NPC.direction == -1 && NPC.velocity.X > -maxXVelocity)
            {
                NPC.velocity.X = NPC.velocity.X - 0.1f;
                if (NPC.velocity.X > maxXVelocity)
                {
                    NPC.velocity.X = NPC.velocity.X - 0.1f;
                }
                else if (NPC.velocity.X > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.05f;
                }
                if (NPC.velocity.X < -maxXVelocity)
                {
                    NPC.velocity.X = -maxXVelocity;
                }
            }
            else if (NPC.direction == 1 && NPC.velocity.X < maxXVelocity)
            {
                NPC.velocity.X = NPC.velocity.X + 0.1f;
                if (NPC.velocity.X < -maxXVelocity)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.1f;
                }
                else if (NPC.velocity.X < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - 0.05f;
                }
                if (NPC.velocity.X > maxXVelocity)
                {
                    NPC.velocity.X = maxXVelocity;
                }
            }
            maxXVelocity = 1.5f;
            if (NPC.directionY == -1 && NPC.velocity.Y > -maxXVelocity)
            {
                NPC.velocity.Y = NPC.velocity.Y - 0.04f;
                if (NPC.velocity.Y > maxXVelocity)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.05f;
                }
                else if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.03f;
                }
                if (NPC.velocity.Y < -maxXVelocity)
                {
                    NPC.velocity.Y = -maxXVelocity;
                }
            }
            else if (NPC.directionY == 1 && NPC.velocity.Y < maxXVelocity)
            {
                NPC.velocity.Y = NPC.velocity.Y + 0.04f;
                if (NPC.velocity.Y < -maxXVelocity)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.05f;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.03f;
                }
                if (NPC.velocity.Y > maxXVelocity)
                {
                    NPC.velocity.Y = maxXVelocity;
                }
            }
        }
    }
}
