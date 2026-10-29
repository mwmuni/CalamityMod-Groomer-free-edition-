using System;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumGyrator : WulfrumCombatNPCBase<WulfrumGyratorBanner>
    {
        protected override int FrameCount => 10;
        protected override string BestiaryEntryKey => "Mods.CalamityMod.Bestiary.WulfrumGyrator";
        protected override int HitDustCount => 5;
        protected override int DeathDustCount => 20;
        protected override string GoreName => "WulfrumGyratorGore";
        protected override bool BonusFrameInZenith => true;

        public float TimeSpentStuck
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public const float PlayerTargetingThreshold = 90f;
        public const float PlayerSearchDistance = 500f;
        public const float StuckJumpPromptTime = 45f;
        public const float MaxMovementSpeedX = 6f;
        public const float JumpSpeed = -8f;
        public const float NPCGravity = 0.3f; // NPC.cs has this, but it's private, and there's no way in hell I'm using reflection.

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.damage = 15;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 5;
            NPC.lifeMax = 25;
            NPC.knockBackResist = 0.15f;
            NPC.value = Item.buyPrice(copper: 75);
        }

        public override void AI()
        {
            NPC.TargetClosest(Main.zenithWorld ? true : false);

            Player player = Main.player[NPC.target];

            if (Supercharged)
            {
                float chargeJumpSpeed = -12f;
                float maxHeight = chargeJumpSpeed * chargeJumpSpeed * (float)Math.Pow(Math.Sin(NPC.AngleTo(player.Center)), 2) / (4f * NPCGravity);
                bool jumpWouldHitPlayer = maxHeight > Math.Abs(player.Center.Y - NPC.Center.Y) && maxHeight < Math.Abs(player.Center.Y - NPC.Center.Y) + player.height;

                if (Main.netMode != NetmodeID.MultiplayerClient && jumpWouldHitPlayer && NPC.collideY && NPC.velocity.Y == 0f)
                {
                    NPC.velocity.Y = chargeJumpSpeed;
                    NPC.ForceNetUpdate();
                }

                SuperchargeTimer--;
            }

            // Jump if there's an obstacle ahead.
            if (Main.netMode != NetmodeID.MultiplayerClient && HoleAtPosition(NPC.Center.X + NPC.velocity.X * 4f) && NPC.collideY && NPC.velocity.Y == 0f)
            {
                NPC.velocity.Y = JumpSpeed;
                NPC.ForceNetUpdate();
            }

            if (Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height) &&
                Math.Abs(player.Center.X - NPC.Center.X) < PlayerSearchDistance &&
                Math.Abs(player.Center.X - NPC.Center.X) > PlayerTargetingThreshold)
            {
                int direction = Math.Sign(player.Center.X - NPC.Center.X) * (NPC.confused ? -1 : 1);
                if (NPC.direction != direction)
                {
                    NPC.direction = direction;
                    NPC.ForceNetUpdate();
                }
                //GOTTA GO FAST (FTW only)
                if (Main.getGoodWorld)
                {
                    NPC.velocity *= 1.01f;
                    //Overcharged
                    if (Supercharged && Main.rand.NextBool(3))
                    {
                        int spark = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitX * 6f * NPC.spriteDirection, Vector2.Zero, ModContent.ProjectileType<EGloveSpark>(), 10, 0f);
                        if (spark.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[spark].friendly = false;
                            Main.projectile[spark].hostile = true;
                            Main.projectile[spark].timeLeft = 60;
                        }
                    }
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient && NPC.collideX && NPC.collideY && NPC.velocity.Y == 0f)
            {
                NPC.velocity.Y = JumpSpeed;
                NPC.ForceNetUpdate();
            }

            if (NPC.oldPosition == NPC.position)
            {
                TimeSpentStuck++;
                if (Main.netMode != NetmodeID.MultiplayerClient && TimeSpentStuck > StuckJumpPromptTime)
                {
                    NPC.velocity.Y = JumpSpeed;
                    TimeSpentStuck = 0f;
                    NPC.netUpdate = true;
                }
            }
            else
                TimeSpentStuck = 0f;

            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, MaxMovementSpeedX * NPC.direction * (Supercharged ? 1.25f : 1f), Supercharged ? 0.02f : 0.0125f);
            Vector4 adjustedVectors = Collision.WalkDownSlope(NPC.position, NPC.velocity, NPC.width, NPC.height, NPCGravity);
            NPC.position = adjustedVectors.XY();
            NPC.velocity = adjustedVectors.ZW();
        }

        private bool HoleAtPosition(float xPosition)
        {
            int tileWidth = NPC.width / 16;
            xPosition = (int)(xPosition / 16f) - tileWidth;
            if (NPC.velocity.X > 0)
                xPosition += tileWidth;

            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = (int)xPosition; x < xPosition + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile)
                        return false;
                }
            }

            return true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            AddCombatLoot(npcLoot, 1, 2);
        }
    }
}
