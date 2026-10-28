using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    public abstract class DesertNuisanceBodyBase : ModNPC
    {
        private const int ClosedFinFrame = 5;

        protected abstract int SegmentWidth { get; }
        protected abstract int SegmentHeight { get; }
        protected abstract int SegmentDefense { get; }
        protected abstract int GoodWorldDefenseBonus { get; }
        protected abstract int NormalLifeMax { get; }
        protected abstract int RevengeanceLifeMax { get; }
        protected abstract int BossRushLifeMax { get; }
        protected abstract int SegmentOffset { get; }
        protected abstract int AlternateFinHitDistance { get; }
        protected abstract bool ScaleHitDistance { get; }
        protected abstract string GoreNamePrefix { get; }
        protected abstract Asset<Texture2D> AlternateBodyTexture2 { get; }
        protected abstract Asset<Texture2D> AlternateBodyTexture3 { get; }
        protected abstract Asset<Texture2D> AlternateBodyTexture4 { get; }

        protected abstract void RequestAlternateBodyTextures();
        protected abstract bool HasHead();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;

            this.HideFromBestiary();

            if (!Main.dedServ)
                RequestAlternateBodyTextures();

            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.damage = 12; // 24
            NPC.width = SegmentWidth;
            NPC.height = SegmentHeight;

            NPC.defense = SegmentDefense;
            if (Main.getGoodWorld)
                NPC.defense += GoodWorldDefenseBonus;

            NPC.LifeMaxNERB(NormalLifeMax, RevengeanceLifeMax, BossRushLifeMax);
            if (Main.getGoodWorld)
                NPC.lifeMax *= 2;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void AI()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            if (NPC.ai[3] > 0f)
            {
                switch ((int)NPC.ai[3])
                {
                    default:
                        break;

                    case 10:

                        NPC.ai[3] = 1f;

                        NPC.position = NPC.Center;
                        NPC.position -= NPC.Size * 0.5f;
                        NPC.frame = new Rectangle(0, 0, AlternateBodyTexture2 is null ? 0 : AlternateBodyTexture2.Width(), AlternateBodyTexture2 is null ? 0 : AlternateBodyTexture2.Height());
                        NPC.ForceNetUpdate();

                        break;

                    case 20:

                        NPC.ai[3] = 2f;

                        NPC.position = NPC.Center;
                        NPC.position -= NPC.Size * 0.5f;
                        NPC.frame = new Rectangle(0, 0, AlternateBodyTexture3 is null ? 0 : AlternateBodyTexture3.Width(), AlternateBodyTexture3 is null ? 0 : AlternateBodyTexture3.Height());
                        NPC.ForceNetUpdate();

                        break;

                    case 30:

                        NPC.ai[3] = 3f;

                        NPC.position = NPC.Center;
                        NPC.position -= NPC.Size * 0.5f;
                        NPC.frame = new Rectangle(0, 0, AlternateBodyTexture4 is null ? 0 : AlternateBodyTexture4.Width(), AlternateBodyTexture4 is null ? 0 : AlternateBodyTexture4.Height());
                        NPC.ForceNetUpdate();

                        break;
                }
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.life > Main.npc[(int)NPC.ai[1]].life)
                NPC.life = Main.npc[(int)NPC.ai[1]].life;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            bool shouldDespawn = !HasHead();
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

            if (Main.player[NPC.target].dead)
                NPC.TargetClosest(false);

            NPC aheadSegment = Main.npc[(int)NPC.ai[1]];
            Vector2 directionToNextSegment = aheadSegment.Center - NPC.Center;
            if (aheadSegment.rotation != NPC.rotation)
            {
                directionToNextSegment = directionToNextSegment.RotatedBy(MathHelper.WrapAngle(aheadSegment.rotation - NPC.rotation) * 0.08f);
                directionToNextSegment = directionToNextSegment.MoveTowards((aheadSegment.rotation - NPC.rotation).ToRotationVector2(), 1f);
            }

            // Decide segment offset stuff.
            int segmentOffset = SegmentOffset;
            NPC.rotation = directionToNextSegment.ToRotation() + MathHelper.PiOver2;
            NPC.Center = aheadSegment.Center - directionToNextSegment.SafeNormalize(Vector2.Zero) * NPC.scale * segmentOffset;
            NPC.spriteDirection = (directionToNextSegment.X > 0).ToDirectionInt();
        }

        public override bool CheckActive() => false;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            float hitDistance = 30f;
            switch ((int)NPC.ai[3])
            {
                default:
                case 1:
                case 10:
                    hitDistance = AlternateFinHitDistance;
                    break;

                case 2:
                case 20:
                    hitDistance = AlternateFinHitDistance;
                    break;

                case 3:
                case 30:
                    hitDistance = 30f;
                    break;
            }

            if (ScaleHitDistance)
                return minDist <= hitDistance * NPC.scale;
            return minDist <= hitDistance;
        }

        public override void FindFrame(int frameHeight)
        {
            // Fin animation segment.
            if (NPC.ai[3] == 0f)
            {
                // Close fins while head is in tiles.
                NPC head = Main.npc[(int)NPC.ai[2]];
                Point headTileCenter = head.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(headTileCenter);
                bool headInSolidTile = tileSafely.HasUnactuatedTile || tileSafely.LiquidAmount > 0;
                if (headInSolidTile)
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 10D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * ClosedFinFrame)
                        NPC.frame.Y = frameHeight * ClosedFinFrame;
                }

                // Open fins while head is outside tiles.
                else
                {
                    if (NPC.frame.Y > 0)
                    {
                        NPC.frameCounter += 1D;
                        if (NPC.frameCounter > 10D)
                        {
                            NPC.frame.Y += frameHeight;
                            NPC.frameCounter = 0D;
                        }
                        if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
                            NPC.frame.Y = 0;
                    }
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    switch ((int)NPC.ai[3])
                    {
                        default:
                        case 0:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(GoreNamePrefix + "Body").Type, NPC.scale);
                            break;

                        case 1:
                        case 10:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(GoreNamePrefix + "Body2").Type, NPC.scale);
                            break;

                        case 2:
                        case 20:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(GoreNamePrefix + "Body3").Type, NPC.scale);
                            break;

                        case 3:
                        case 30:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(GoreNamePrefix + "Body4").Type, NPC.scale);
                            break;
                    }
                }

                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * balance);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
            {
                Color lightColor = Color.Orange * drawColor.A;
                return lightColor * NPC.Opacity;
            }
            else return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[3] > 0f)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Texture2D texture = default;
                switch ((int)NPC.ai[3])
                {
                    default:
                    case 1:
                    case 10:
                        texture = AlternateBodyTexture2.Value;
                        break;

                    case 2:
                    case 20:
                        texture = AlternateBodyTexture3.Value;
                        break;

                    case 3:
                    case 30:
                        texture = AlternateBodyTexture4.Value;
                        break;
                }

                Vector2 halfSizeTexture = new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2));
                Vector2 drawLocation = NPC.Center - screenPos;
                drawLocation -= new Vector2((float)texture.Width, (float)(texture.Height)) * NPC.scale / 2f;
                drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                spriteBatch.Draw(texture, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

                return false;
            }

            return true;
        }
    }
}
