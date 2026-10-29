using System;
using System.IO;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumHovercraft : WulfrumCombatNPCBase<WulfrumHovercraftBanner>
    {
        protected override int FrameCount => 12;
        protected override string BestiaryEntryKey => "Mods.CalamityMod.Bestiary.WulfrumHovercraft";
        protected override int HitDustCount => 5;
        protected override int DeathDustCount => 20;
        protected override string GoreName => "WulfrumHovercraftGore1";
        protected override string AdditionalGoreName => "WulfrumHovercraftGore2";

        internal enum HovercraftAIState
        {
            Searching = 0,
            Hover = 1,
            Slowdown = 2,
            SwoopDownward = 3
        }

        public float StunTime;

        internal HovercraftAIState AIState
        {
            get => (HovercraftAIState)(int)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public float SubphaseTime
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public float SearchDirection
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        public ref float FlyAwayTimer => ref NPC.localAI[0];

        public const float StunTimeMax = 45f;
        public const float SearchXOffset = 345f;
        public const float TotalSubphaseTime = 110f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.damage = 15;
            NPC.width = 40;
            NPC.height = 38;
            NPC.defense = 4;
            NPC.lifeMax = 25;
            NPC.value = Item.buyPrice(silver: 1);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StunTime);
            writer.Write(FlyAwayTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StunTime = reader.ReadSingle();
            FlyAwayTimer = reader.ReadSingle();
        }

        public override void AI()
        {
            NPC.knockBackResist = 0.1f;

            Player player = Main.player[NPC.target];

            bool farFromPlayer = NPC.Distance(player.Center) > 960f;

            if (NPC.target < 0 || NPC.target >= 255 || farFromPlayer|| player.dead || !player.active)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                farFromPlayer = NPC.Distance(player.Center) > 960f;
                // Fly away if there is no living target, or the closest target is too far away.
                if (player.dead || !player.active || farFromPlayer)
                {
                    if (FlyAwayTimer > 420)
                    {
                        // Don't go away from me >:(
                        if (Main.zenithWorld && player.active && !farFromPlayer)
                        {
                            AIState = HovercraftAIState.SwoopDownward;
                            SoundEngine.PlaySound(SoundID.DD2_KoboldFlyerHurt with { Pitch = SoundID.DD2_KoboldFlyerHurt.Pitch + 0.5f }, NPC.Center);
                            return;
                        }
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitY * -8f, 0.1f);
                        NPC.rotation = NPC.rotation.AngleTowards(0f, MathHelper.ToRadians(15f));
                        NPC.noTileCollide = true;
                    }
                    else
                    {
                        NPC.velocity *= 0.96f;
                        NPC.rotation = NPC.rotation.AngleTowards(0f, MathHelper.ToRadians(15f));
                        FlyAwayTimer++;
                    }
                    return;
                }
            }

            FlyAwayTimer = Utils.Clamp(FlyAwayTimer - 3, 0, 180);

            NPC.noTileCollide = !farFromPlayer;

            Lighting.AddLight(NPC.Center - Vector2.UnitY * 8f, Color.Lime.ToVector3() * 1.5f);

            if (StunTime > 0)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (!Main.dedServ && Main.rand.NextBool(4))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(8f, 8f), DustID.Electric).scale = 0.7f;
                    }
                }

                NPC.rotation = NPC.rotation.AngleTowards(0f, MathHelper.ToRadians(15f));
                if (StunTime > StunTimeMax - 15)
                    NPC.velocity *= 0.6f;
                else
                    NPC.knockBackResist = 2.4f;

                StunTime--;
                return;
            }

            if (SearchDirection == 0f)
            {
                if (Math.Abs(player.Center.X + SearchXOffset - NPC.Center.X) < Math.Abs(player.Center.X - SearchXOffset - NPC.Center.X))
                    SearchDirection = 1f;
                else
                    SearchDirection = -1f;

                NPC.netUpdate = true;
            }

            if (AIState == HovercraftAIState.Searching || AIState == HovercraftAIState.Hover)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                Vector2 destination = player.Center + new Vector2(SearchXOffset * SearchDirection, -160f);
                NPC.velocity = NPC.SafeDirectionTo(destination, Vector2.UnitY) * (Supercharged ? 7f : 5f);
                if (AIState == HovercraftAIState.Hover)
                {
                    destination = player.Center + new Vector2(SearchXOffset * -SearchDirection, -160f);
                    NPC.velocity = NPC.SafeDirectionTo(destination, Vector2.UnitY) * (Supercharged ? 5.5f : 4f);
                }

                NPC.rotation = NPC.velocity.X / 16f;
                if (NPC.Distance(destination) < 50f)
                {
                    if (AIState == HovercraftAIState.Searching)
                    {
                        AIState = HovercraftAIState.Slowdown;
                    }
                    else
                    {
                        AIState = HovercraftAIState.SwoopDownward;
                    }
                    NPC.netUpdate = true;
                }
            }

            if (AIState == HovercraftAIState.Slowdown)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                SubphaseTime++;
                if (SubphaseTime < 30f)
                {
                    NPC.velocity *= 0.96f;
                }
                else
                {
                    AIState = HovercraftAIState.Hover;
                    SubphaseTime = 0f;
                    NPC.netUpdate = true;
                }
            }

            if (AIState == HovercraftAIState.SwoopDownward)
            {
                // Set damage
                NPC.damage = NPC.defDamage;

                NPC.rotation = 0f;
                float swoopType = Supercharged ? TotalSubphaseTime - 40f : TotalSubphaseTime;
                float swoopSlowdownTime = Supercharged ? 10f : 45f;
                Vector2 swoopVelocity = Vector2.UnitY.RotatedBy(MathHelper.Pi * SubphaseTime / swoopType * -SearchDirection) * (Supercharged ? 11f : 8.5f);

                SubphaseTime++;
                if (SubphaseTime < swoopSlowdownTime)
                {
                    swoopVelocity *= MathHelper.Lerp(1f, 0.75f, Utils.GetLerpValue(45f, 0f, SubphaseTime));
                }
                if (SubphaseTime >= swoopType - swoopSlowdownTime)
                {
                    swoopVelocity *= MathHelper.Lerp(1f, 0.75f, Utils.GetLerpValue(swoopType - 45f, swoopType, SubphaseTime));
                }
                swoopVelocity.Y *= 0.5f;

                NPC.velocity = Vector2.Lerp(NPC.velocity, swoopVelocity, 0.2f);

                if (SubphaseTime >= swoopType)
                {
                    AIState = HovercraftAIState.Searching;
                    SearchDirection = 0f;
                    SubphaseTime = 0f;
                    NPC.netUpdate = true;
                }

                NPC.rotation = NPC.velocity.X / 12f;
            }

            NPC.spriteDirection = (NPC.velocity.X < 0).ToDirectionInt();
        }

        protected override void AdditionalHitEffects(NPC.HitInfo hit)
        {
            if (!Main.dedServ)
            {
                //Become a spark piñata in FTW
                if (Main.getGoodWorld && Supercharged)
                {
                    for (int Sparks = Main.rand.Next(2, 5); Sparks > 0; Sparks--)
                    {
                        Vector2 velocity = CalamityUtils.RandomVelocity(50f, 30f, 60f);
                        int spark = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitX * 6f * NPC.spriteDirection, velocity, ModContent.ProjectileType<EGloveSpark>(), 10, 0f);
                        if (spark.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[spark].friendly = false;
                            Main.projectile[spark].hostile = true;
                            Main.projectile[spark].timeLeft = 90;
                        }
                    }
                }
            }
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            StunTime = StunTimeMax;
            NPC.netUpdate = true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            AddCombatLoot(npcLoot, 2, 3);
        }
    }
}
