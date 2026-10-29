using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SamsaraSlicerProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SamsaraSlicer";

        //These are the same number, but will be left as two seperate variables for future tuning purposes
        public float ReboundVelocity => 30;
        public float StealthReboundVelocity => 30;
        public float StealthPauseTime => 55;
        public int ReboundTime => 30;

        public int SmallDiskDamage => 30;
        public int SmallDiskStealthDamage => 19;

        public bool initialized = false;

        Vector2 oldVelocity;

        int? npcTaggedTo = null;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 46;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.aiStyle = -1;
            Projectile.ai[0] = -200;
            Projectile.ai[2] = -200;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 20;
            height = 20;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.5f, 0.03f));

            if (!initialized)
            {
                Projectile.ai[2] = -200;
                Projectile.ai[0] = -200;
                initialized = true;
            }

            Player player = Main.player[Projectile.owner];

            // Main movement

            if (Projectile.ai[0] < 0)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] > ReboundTime)
                {
                    Projectile.tileCollide = false;

                    float lerp = (float)(Projectile.ai[1] - ReboundTime) * 0.01f;

                    if (Projectile.Calamity().stealthStrike)
                        lerp = (float)(Projectile.ai[1] - ReboundTime) * 0.005f;

                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(player.Center) * (Projectile.Calamity().stealthStrike ? StealthReboundVelocity : ReboundVelocity), lerp);

                    if (Projectile.Distance(player.Center) < Projectile.velocity.Length() * 1.4f)
                    {
                        for (int i = 0; i < Main.projectile.Length; i++)
                        {
                            Projectile proj = Main.projectile[i];

                            if (proj.type == ModContent.ProjectileType<SamsaraSlicerSmallDisk>())
                            {
                                if ((proj.ModProjectile as SamsaraSlicerSmallDisk).Parent == Projectile)
                                {
                                    (Main.projectile[i].ModProjectile as SamsaraSlicerSmallDisk).Parent = null;
                                }
                            }
                        }

                        Projectile.Kill();
                    }
                }
            }
            else
            {
                if (npcTaggedTo != null)
                {
                    NPC npc = Main.npc[npcTaggedTo.GetValueOrDefault(0)];
                    if (npc.active)
                        Projectile.Center += npc.velocity;
                }
            }

            // Frame pause

            if (Projectile.ai[0] > -150) Projectile.ai[0]--;

            if (Projectile.ai[0] == 0)
            {
                npcTaggedTo = null;

                Projectile.velocity = oldVelocity;

                SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot.WithPitchOffset(1f));


                if (Projectile.Calamity().stealthStrike)
                {
                    SoundEngine.PlaySound(SoundID.Item122.WithPitchOffset(1f), Projectile.Center);

                    for (int i = 1; i <= 3; i++)
                    {
                        GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, oldVelocity * MathHelper.Lerp(2f, 1f, (float)i / 3f) / 4, Color.LimeGreen, "CalamityMod/Particles/SoftRoundExplosion", new Vector2(0.6f, 1f), oldVelocity.ToRotation(), 0.02f, 0.05f * i, 30));
                    }

                    for (int i = -10; i <= 20; i++)
                    {
                        GeneralParticleHandler.SpawnParticle(new CustomSpark(Projectile.Center, new Vector2(i * 2, 0).RotatedBy(Projectile.velocity.ToRotation()), "CalamityMod/Particles/ThinEndedLine", false, 10, Main.rand.NextFloat(0.3f, 1f), Main.rand.NextBool() ? new Color(1f, 0.8f, 0.1f) : Color.LimeGreen, new Vector2(Main.rand.NextFloat(0.4f, 1f), 1f)));
                    }
                }
                else
                {
                    for (int i = 1; i <= 2; i++)
                        GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, new Vector2(i == 1 ? 2 : 6, 0).RotatedBy(Projectile.velocity.ToRotation()), Color.LimeGreen, "CalamityMod/Particles/BloomRing", new Vector2(0.5f, 1f), Projectile.velocity.ToRotation(), 0.1f, 0.5f - (i * 0.1f), 20));
                }

                for (int i = 0; i <= 5; i++)
                    GeneralParticleHandler.SpawnParticle(new CustomSpark(Projectile.Center, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-10f, 10f)).RotatedBy(Projectile.velocity.ToRotation()), "CalamityMod/Particles/ThinEndedLine", false, 10, Main.rand.NextFloat(0.3f, 1f), Main.rand.NextBool() ? new Color(1f, 0.8f, 0.1f) : Color.LimeGreen, new Vector2(Main.rand.NextFloat(0.4f, 1f), 1f)));
            }

            if (Projectile.ai[0] <= 0 && Projectile.ai[0] > -4)
            {
                Projectile.extraUpdates = 1;
            }
            else
            {
                Projectile.extraUpdates = 0;
            }

            Vector2 vel = Projectile.velocity;
            if (Projectile.ai[0] > 0)
            {
                vel = new Vector2(18);
            }

            if (Projectile.ai[2] > -150)
            {
                Projectile.ai[2]--;
            }

            if (Projectile.ai[2] > 0)
            {
                if (Projectile.ai[2] % (Projectile.Calamity().stealthStrike ? 3 : 5) == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item23.WithPitchOffset(MathHelper.Lerp(1f, 0f, Projectile.ai[2] / 30)).WithVolumeScale(0.8f));
                }

                for (int i = 0; i <= 2; i++)
                    GeneralParticleHandler.SpawnParticle(new CustomSpark(Projectile.Center + new Vector2(25, 0).RotatedBy(oldVelocity.ToRotation()), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-10f, 10f)).RotatedBy(oldVelocity.ToRotation()), "CalamityMod/Particles/ThinEndedLine", false, 10, Main.rand.NextFloat(0.3f, 1f), Main.rand.NextBool() ? new Color(1f, 0.8f, 0.1f) : Color.LimeGreen, new Vector2(Main.rand.NextFloat(0.4f, 1f), 1f)));
            }
            
            if (Projectile.ai[2] > -150)
            {
                if (Projectile.Calamity().stealthStrike)
                {
                    float SpawnVel = 15;

                    float g = Main.rand.NextFloat(360f);

                    if (!Main.dedServ)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, new Vector2(SpawnVel, 0).RotatedBy(MathHelper.ToRadians(g)),
                        ModContent.ProjectileType<SamsaraSlicerSmallDisk>(), Projectile.Calamity().stealthStrike ? SmallDiskStealthDamage : SmallDiskDamage, 1f, Projectile.owner, Projectile.whoAmI);
                        (proj.ModProjectile as SamsaraSlicerSmallDisk).Parent = Projectile;
                    }
                }
            }

            if (Projectile.ai[2] == 0)
            {
                Projectile.localNPCHitCooldown = 30;
            }

            Projectile.rotation += MathHelper.ToRadians(vel.Length() * 1.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);

            float rand = 0;
            if (Projectile.ai[2] > 0)
            {
                rand = 4;
            }

            Vector2 randVec = new Vector2(Main.rand.NextFloat(-rand, rand), 0).RotatedBy(oldVelocity.ToRotation());

            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + randVec, tex.Frame(), Color.White, Projectile.rotation, tex.Frame().Center(), 1f, SpriteEffects.None);

            if (Projectile.ai[2] < 0)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], new Color(0f, 0.6f, 0f, 0f), 2, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/SamsaraSlicerGlow").Value);
            }
            else
            {
                Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/SamsaraSlicerGlow").Value, Projectile.Center - Main.screenPosition + randVec, tex.Frame(), new Color(0f, 1f, 0f, 0f), Projectile.rotation, tex.Frame().Center(), 1f, SpriteEffects.None);
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            npcTaggedTo = target.whoAmI;

            if (Projectile.velocity != Vector2.Zero)
            {
                if (Projectile.ai[0] <= -200)
                    oldVelocity = Projectile.velocity * 1.5f;
                else
                    oldVelocity = Projectile.velocity;
            }
            Projectile.ai[1] = ReboundTime - 10;
            Projectile.velocity = Vector2.Zero;
            Projectile.ai[0] = 5;

            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact);

            if (Projectile.ai[2] == -200)
            {
                float lag = 20;
                if (Projectile.Calamity().stealthStrike) lag = StealthPauseTime;

                Projectile.ai[0] = lag;
                Projectile.ai[2] = lag;

                Projectile.localNPCHitCooldown = (int)lag;

                float g = Main.rand.NextFloat(360f);

                g -= 15f;

                float SpawnVel = 15;
                if (Projectile.Calamity().stealthStrike)
                    SpawnVel = 20;

                if (!Main.dedServ)
                {
                    for (float i = g; i < g + 360f; i += Projectile.Calamity().stealthStrike ? 45f : 90f)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, new Vector2(SpawnVel, 0).RotatedBy(MathHelper.ToRadians(i)),
                            ModContent.ProjectileType<SamsaraSlicerSmallDisk>(), Projectile.Calamity().stealthStrike ? SmallDiskStealthDamage : SmallDiskDamage, 1f, Projectile.owner, Projectile.whoAmI);
                        (proj.ModProjectile as SamsaraSlicerSmallDisk).Parent = Projectile;
                        proj.Calamity().stealthStrike = Projectile.Calamity().stealthStrike;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        // Make it bounce on tiles.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Impacts the terrain even though it bounces off.
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            Projectile.BounceOnTileCollision(oldVelocity);

            return false;
        }
    }
}
