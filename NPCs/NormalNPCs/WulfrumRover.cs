using System;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumRover : WulfrumCombatNPCBase<WulfrumRoverBanner>
    {
        protected override int FrameCount => 16;
        protected override string BestiaryEntryKey => "Mods.CalamityMod.Bestiary.WulfrumRover";
        protected override int HitDustCount => 4;
        protected override int DeathDustCount => 20;
        protected override string GoreName => "WulfrumRoverGore1";
        protected override string AdditionalGoreName => "WulfrumRoverGore2";

        public float TimeSpentStuck
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public const float PlayerTargetingThreshold = 90f;
        public const float PlayerSearchDistance = 500f;
        public const float StuckJumpPromptTime = 90f;
        public const float MaxMovementSpeedX = 6f;
        public const float JumpSpeed = -4f;

        //GetFuckedBoi
        public int mineDelay = 0;

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.damage = 10;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 4;
            NPC.lifeMax = 40;
            NPC.knockBackResist = Main.zenithWorld ? 0f : 0.15f;
            NPC.value = Item.buyPrice(copper: 75);
            if (Main.zenithWorld)
                NPC.scale = 2f;
        }

        public override void AI()
        {
            NPC.TargetClosest(false);

            if (Supercharged)
            {
                SuperchargeTimer--;
                NPC.defense = Main.getGoodWorld ? 20 : 13;
            }
            else if (!Supercharged)
            {
                NPC.defense = Main.getGoodWorld ? 10 : 4;
            }

            Player player = Main.player[NPC.target];
            if (Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height) &&
                Math.Abs(player.Center.X - NPC.Center.X) < PlayerSearchDistance &&
                Math.Abs(player.Center.X - NPC.Center.X) > PlayerTargetingThreshold)
            {
                int direction = Math.Sign(player.Center.X - NPC.Center.X) * (NPC.confused ? -1 : 1);
                if (NPC.direction != direction)
                {
                    NPC.direction = direction;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.collideX)
            {
                NPC.direction *= -1;
                NPC.netUpdate = true;
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

            NPC.spriteDirection = -NPC.direction;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, MaxMovementSpeedX * NPC.direction, 0.0125f);
            NPC.StepUpBlocks();
        }

        protected override void AdditionalHitEffects(NPC.HitInfo hit)
        {
            if (!Main.dedServ)
            {
                if (Main.zenithWorld && mineDelay == 0)
                {
                    Vector2 roverBase = new Vector2(NPC.Center.X, NPC.Center.Y + 5f);
                    int mine = Projectile.NewProjectile(NPC.GetSource_FromAI(), roverBase, Vector2.Zero, ProjectileID.ProximityMineI, 50, 0f);
                    if (mine.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[mine].friendly = false;
                        Main.projectile[mine].hostile = true;
                        Main.projectile[mine].timeLeft = 60;
                    }
                    mineDelay = 3;
                }
                else if (Main.zenithWorld && mineDelay >= 1)
                    mineDelay--;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Supercharged)
            {
                //0.6 : The noise upscale
                //0.15 the scale its drawn at usually, 0.2 on Zenith
                float scale = (Main.zenithWorld ? 0.2f : 0.15f) + 0.03f * (0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f + NPC.whoAmI * 0.2f));
                float noiseScale = 0.6f;

                Effect shieldEffect = Terraria.Graphics.Effects.Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.24f);
                shieldEffect.Parameters["blowUpPower"].SetValue(2.5f);
                shieldEffect.Parameters["blowUpSize"].SetValue(0.5f);
                shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                float baseShieldOpacity = 0.9f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);
                shieldEffect.Parameters["shieldOpacity"].SetValue(baseShieldOpacity);
                shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                Color blueTint = new Color(51, 102, 255);
                Color cyanTint = new Color(71, 202, 255);
                Color wulfGreen = new Color(194, 255, 67) * 0.8f;
                Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, blueTint, cyanTint, wulfGreen);
                shieldEffect.Parameters["shieldColor"].SetValue(blueTint.ToVector3());
                shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

                if (RoverDrive.NoiseTex == null)
                    RoverDrive.NoiseTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise");

                Texture2D tex = RoverDrive.NoiseTex.Value;
                Vector2 pos = NPC.Center + NPC.gfxOffY * Vector2.UnitY - Main.screenPosition;
                Main.spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            AddCombatLoot(npcLoot, 1, 2, ModContent.ItemType<RoverDrive>());
        }
    }
}
