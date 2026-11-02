using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class VoidEaterMarionetteProjectile : BaseWormProjectile, ILocalizedModType
    {
        #region Balancing
        public float ContactDamageMult => 0.6f + Projectile.minionSlots * 0.4f;
        public static int EffectiveContactIframes => 10;

        public float BlueFireballDamageMult => Projectile.minionSlots * 1.1f;
        public static int EffectiveBlueFireIframes => 10;

        public float PurpleFireballDamageMult => Projectile.minionSlots * 1.1f;
        public static int EffectivePurpleFireIframes => 10;

        #endregion
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Summon/VoidEaterMarionetteHead";

        public override string GlowTexture => "CalamityMod/Projectiles/Summon/VoidEaterMarionetteHeadGlow";
        public override List<string> SegmentTextures => new()
        {
            "CalamityMod/Projectiles/Summon/VoidEaterMarionetteBody",
            "CalamityMod/Projectiles/Summon/VoidEaterMarionetteTail"
        };

        public override int SegmentCount => (int)(Projectile.minionSlots);

        public override List<float> SegmentTypePositionOffsets => new()
        {
            54, //Head
            38, //Body 
            52 //Tail
        };

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.MaxUpdates = 2;
            Projectile.localNPCHitCooldown = EffectiveContactIframes * Projectile.MaxUpdates;
            Projectile.DamageType = DamageClass.Summon;
        }

        public enum AttackState
        {
            Idle,
            DivinityDevourer,
            FaithIncinerator,
            UltracosmicMaelstrom,
            PlayingFetch
        }
        /// <summary>
        /// Used to increase the length of / slots required by the minion
        /// </summary>
        public int MinionSlotsToAdd
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        public AttackState CurrentAttack
        {
            get { return (AttackState)Projectile.ai[1]; }
            set { Projectile.ai[1] = (float)value; }
        }

        public int AiTimer
        {
            get { return (int)Projectile.ai[2]; }
            set { Projectile.ai[2] = value; }
        }

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModOwner => Owner.Calamity();

        public Vector2 EntrancePortalLocation = Vector2.Zero;
        public Vector2 ExitPortalLocation = Vector2.Zero;
        bool TightHoming = false;
        public bool FocusOnFetching = false;
        public float JawOpeningAmount = 0;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(FocusOnFetching);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            FocusOnFetching = reader.ReadBoolean();
        }

        public override void AI()
        {
            Owner.AddBuff(ModContent.BuffType<VoidEaterMarionetteBuff>(), 3600);
            if (Owner.dead)
                ModOwner.hasVoidEaterMarionette = false;
            if (ModOwner.hasVoidEaterMarionette)
                Projectile.timeLeft = 2;

            #region Update Segment Count
            if (MinionSlotsToAdd > 0)
            {
                float minionSlotsAvaliable = Owner.maxMinions;
                foreach (var item in Main.ActiveProjectiles)
                {
                    if (item.owner == Projectile.owner)
                        minionSlotsAvaliable -= item.minionSlots;
                }
                while (minionSlotsAvaliable >= 1 && MinionSlotsToAdd > 0)
                {

                    Projectile.minionSlots++;
                    minionSlotsAvaliable--;
                    MinionSlotsToAdd--;
                }
                MinionSlotsToAdd = 0;
            }

            bool UpdatedSegmentCount = false;
            while (Segments.Count < SegmentCount)
            {
                UpdatedSegmentCount = true;
                Segments.Add(new BaseWormSegment(this, 0));
            }
            while (Segments.Count > SegmentCount)
            {
                UpdatedSegmentCount = true;
                Segments.RemoveAt(Segments.Count - 1);
            }
            if (UpdatedSegmentCount)
            {
                foreach (var item in Segments)
                {
                    item.segmentType = 0;
                }
                Segments[Segments.Count - 1].segmentType = 1;
            }
            #endregion
            if (Projectile.minionSlots < 1)
            {
                Projectile.Kill();
                return;
            }
            int targetID = -1;
            Projectile.Minion_FindTargetInRange(CalamityUtils.TilesToPixels(300), ref targetID, false);
            NPC target = Projectile.Center.MinionHoming(CurrentAttack != AttackState.Idle ? 999999f : 2800f, Owner);
            SegmentRigidity = 0.1f;
            Projectile.extraUpdates = 1;
            if (Owner.miscCounter % 10 == 0 && (CurrentAttack == AttackState.Idle || (FocusOnFetching && CurrentAttack != AttackState.UltracosmicMaelstrom)))
            {
                float SearchDistance = CalamityUtils.TilesToPixels(50);
                AiTimer = -1;
                foreach (var item in Main.ActiveItems)
                {
                    if (item.noGrabDelay == 0 && !item.beingGrabbed && Owner.CanPullItem(item, Owner.ItemSpace(item)) && !item.IsACoin)
                    {
                        var dis = item.Distance(Projectile.Center);
                        if (dis > SearchDistance)
                            continue;
                        AiTimer = item.whoAmI;
                        SearchDistance = dis;
                    }
                }
                if (AiTimer > -1)
                {
                    CurrentAttack = AttackState.PlayingFetch;
                }
                else if (CurrentAttack == AttackState.PlayingFetch)
                    CurrentAttack = AttackState.Idle;
            }

            switch (CurrentAttack)
            {
                case AttackState.Idle:
                    {
                        var playerDistance = Projectile.Distance(Owner.Center);
                        float speed = 0.06f;
                        AiTimer = -1;
                        if (playerDistance > CalamityUtils.TilesToPixels(150))
                        {
                            Projectile.Center = Owner.Center - Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 200;
                            SpawnRiftProjectileAt(Projectile.Center);
                            Projectile.velocity = Projectile.velocity.ClampMagnitude(1, 8);
                        }
                        else
                        {
                            if (playerDistance > 1000f)
                                speed = 0.3f;
                            if (playerDistance > 200f)
                                speed = 0.2f;
                            else if (playerDistance > 140f)
                                speed = 0.12f;
                            if (playerDistance > 100)
                                Projectile.velocity += Projectile.DirectionTo(Owner.Center) * speed;
                            else if (Projectile.velocity.Length() > 1)
                                Projectile.velocity *= 0.96f;

                            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                        }
                        if (target != null)
                        {
                            CurrentAttack = AttackState.DivinityDevourer;
                            AiTimer = 0;
                        }
                        JawOpeningAmount = MathHelper.Lerp(JawOpeningAmount,0, 0.1f);
                        break;

                    }

                case AttackState.DivinityDevourer:
                    {
                        if (target == null)
                        {
                            CurrentAttack = AttackState.Idle;
                            AiTimer = 0;
                            TightHoming = false;
                            break;
                        }
                        var targetDistance = Projectile.Distance(target.Center);
                        float turnspeed = 0.01f;

                        if (targetDistance > 500)
                            TightHoming = true;
                        if (TightHoming)
                        {
                            JawOpeningAmount = MathHelper.Lerp(JawOpeningAmount, 0.75f, 0.1f);
                            turnspeed = 0.2f;
                            var dot = Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), Projectile.DirectionTo(target.Center));
                            if (dot < 0f && targetDistance < 200)
                            {
                                TightHoming = false;
                                turnspeed = 0;
                            }
                        } else
                            JawOpeningAmount = MathHelper.Lerp(JawOpeningAmount, 0f, 0.25f);
                        if (targetDistance > 0)

                            Projectile.velocity = Projectile.velocity.ToRotation().AngleLerp(Projectile.DirectionTo(target.Center).ToRotation(), turnspeed).ToRotationVector2() * MathF.Min(Projectile.velocity.Length() + 1, 22f);

                        Projectile.velocity *= 0.975f;
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                        AiTimer++;
                        if (AiTimer > 300 * Projectile.MaxUpdates)
                        {
                            CurrentAttack = AttackState.FaithIncinerator;
                            AiTimer = 0;
                            TightHoming = false;
                        }
                        break;
                    }

                case AttackState.FaithIncinerator:
                    {
                        if (target == null)
                        {
                            CurrentAttack = AttackState.Idle;
                            AiTimer = 0;
                            break;
                        }

                        var targetDistance = Projectile.Distance(target.Center);
                        var targetDir = Projectile.DirectionTo(target.Center);
                        var targetDirToOwner = Owner.DirectionTo(target.Center);
                        var playerDistance = Projectile.Distance(Owner.Center);
                        Vector2 targetedPosition = target.Center;
                        float turnspeed = 0.05f;
                        if (TightHoming)
                            targetedPosition -= targetDir * 960f;
                        else
                            turnspeed = 0.15f;

                        Projectile.velocity = Projectile.velocity.ToRotation().AngleLerp(Projectile.DirectionTo(targetedPosition).ToRotation(), turnspeed).ToRotationVector2() * MathF.Min(Projectile.velocity.Length() + 1, 10f);
                        if (targetDistance > 800f)
                            TightHoming = false;
                        else if (target.Hitbox.Intersects(Projectile.Hitbox))
                            TightHoming = true;

                        JawOpeningAmount = MathHelper.Lerp(JawOpeningAmount, 0, 0.1f);
                        if (Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), targetDir) > 0.8f && AiTimer % 30 == 0)
                        {
                            JawOpeningAmount = 0.75f;
                            if (Main.myPlayer == Projectile.owner)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    Vector2 perturbedSpeed = targetDir.RotatedBy(MathHelper.Lerp(-0.15f, 0.15f, i / 3f)) * 24f;
                                    SpawnGuardianFireball(perturbedSpeed, blue: true);
                                }
                            }
                        }
                        AiTimer++;

                        if (AiTimer > 300 * Projectile.MaxUpdates)
                        {
                            CurrentAttack = AttackState.UltracosmicMaelstrom;
                            AiTimer = 0;
                            TightHoming = false;
                        }
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                        break;
                    }
                
                case AttackState.UltracosmicMaelstrom:
                    {
                        int strikes = 6;
                        var targetPos = Owner.Center;
                        if (target != null)
                        {
                            targetPos = target.Center;
                        }
                        if (AiTimer > 0)
                            JawOpeningAmount = 0.5f;
                        float lastSegmentOpacity = Segments[Segments.Count - 1].Opacity;
                        if (AiTimer == 0 && lastSegmentOpacity > 0 && EntrancePortalLocation == Vector2.Zero)
                        {
                            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 16;
                            EntrancePortalLocation = Projectile.Center + Projectile.velocity * 10;
                        }
                        if (Projectile.Distance(EntrancePortalLocation) <= 32 && AiTimer <= strikes && Projectile.Opacity > 0)
                        {
                            Projectile.Opacity = 0;
                            SpawnRiftProjectileAt(EntrancePortalLocation);
                        }
                        foreach (var item in Segments)
                        {
                            if (item.Center.Distance(EntrancePortalLocation) <= 32)
                            {
                                if (AiTimer <= strikes)
                                    item.Opacity = 0;
                                else if (item.segmentType > 0)
                                {
                                    CurrentAttack = AttackState.DivinityDevourer;
                                    AiTimer = 0;
                                    EntrancePortalLocation = Vector2.Zero;
                                    ExitPortalLocation = Vector2.Zero;
                                    break;
                                }
                            }

                            else if (item.Center.Distance(ExitPortalLocation) <= 32)
                            {
                                item.Opacity = 1;
                            }
                        }
                        if (lastSegmentOpacity == 0 && Projectile.Opacity == 0)
                        {
                            var dir = new Vector2(300).RotatedByRandom(MathHelper.TwoPi);
                            EntrancePortalLocation = targetPos + dir;
                            ExitPortalLocation = targetPos - dir;
                            Projectile.Center = ExitPortalLocation;
                            Projectile.velocity = dir.SafeNormalize(Vector2.Zero) * 16;
                            Projectile.Opacity = 1;
                            foreach (var item in Segments)
                            {
                                item.Center = Projectile.Center - dir * 10;
                            }
                            Segments[Segments.Count - 1].Opacity = 0.001f;
                            AiTimer++;
                            SpawnRiftProjectileAt(ExitPortalLocation);
                            if (Main.myPlayer == Projectile.owner)
                            {
                                for (int i = 0; i < 16; i++)
                                {
                                    Vector2 perturbedSpeed = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 16f) * (i % 2 == 0 ? 24f : 16f);
                                    SpawnGuardianFireball(perturbedSpeed, blue: false);
                                }
                            }
                        }
                        if (AiTimer > 0)
                            Projectile.extraUpdates = 2;

                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                        break;
                    }
                
                case AttackState.PlayingFetch:
                    {
                        var item = Main.item[AiTimer];
                        var dir = Projectile.Center.DirectionTo(item.Center);
                        var holdingSpot = Projectile.Center + (Projectile.rotation-MathHelper.PiOver2).ToRotationVector2() * 35;
                        if (!item.active || item.beingGrabbed || !Owner.CanPullItem(item, Owner.ItemSpace(item)))
                        {
                            AiTimer = 0;
                            CurrentAttack = AttackState.Idle;
                            break;
                        }
                        float dis = MathHelper.Min(item.Distance(holdingSpot), item.Distance(Projectile.Center));
                        if (dis < 36)
                        {

                            Projectile.velocity += Projectile.DirectionTo(Owner.Center) * 0.5f;
                            Projectile.velocity *= 0.95f;
                            item.Center = holdingSpot;
                            item.velocity = Vector2.Zero;
                            JawOpeningAmount = MathHelper.Lerp(JawOpeningAmount, 0, 0.25f);
                        }
                        else
                        {
                            Projectile.velocity += Projectile.DirectionTo(item.Center) * 0.5f;
                            Projectile.velocity *= 0.95f;
                            JawOpeningAmount = MathF.Max(0,MathHelper.Lerp(1,0,dis / 240f));
                        }
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                        break;
                    }
            }
            Projectile.position += Projectile.velocity;
            Projectile.position.X = MathHelper.Clamp(Projectile.position.X, 5, (Main.maxTilesX-5) * 16);
            Projectile.position.Y = MathHelper.Clamp(Projectile.position.Y, 5, (Main.maxTilesY-5) * 16);
            Projectile.position -= Projectile.velocity;
            UpdateSegments();
        }

        private void SpawnGuardianFireball(Vector2 perturbedSpeed, bool blue)
        {
            var p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<DoGFire>(), (int)(Projectile.damage * (blue ? BlueFireballDamageMult : PurpleFireballDamageMult)), Projectile.knockBack, Projectile.owner, blue ? 2 : 0);
            if (Main.projectile.IndexInRange(p))
            {
                Main.projectile[p].hostile = false;
                Main.projectile[p].friendly = true;
                Main.projectile[p].DamageType = DamageClass.Summon;
                Main.projectile[p].timeLeft = 120;
                Main.projectile[p].scale = 0.5f;
                Main.projectile[p].usesIDStaticNPCImmunity = true;
                Main.projectile[p].idStaticNPCHitCooldown = blue ? EffectiveBlueFireIframes : EffectivePurpleFireIframes;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= ContactDamageMult;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (var i = 0; i < Segments.Count; i++)
            {
                var prevSeg = new BaseWormSegment(this);
                if (i != 0)
                {
                    prevSeg = Segments[i - 1];
                }
                if (prevSeg.Opacity <= 0.001 || Segments[i].Opacity <= 0.001)
                    continue;
                float cpoint = 0;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), prevSeg.Center, Segments[i].Center, 16, ref cpoint))
                {
                    return true;
                }
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public void SpawnRiftProjectileAt(Vector2 position)
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, Vector2.Zero, ModContent.ProjectileType<DoGWeaponTeleportRift>(), 0, 0, Projectile.owner,0,0.375f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = Segments.Count - 1; i >= 0; i--)
            {
                DrawSegment(ref lightColor, Segments[i]);
            }
            var jawTex = CalamityUtils.GetTextureEfficient(ref Jaws, "CalamityMod/Projectiles/Summon/VoidEaterMarionetteJaw").Value;
            var jawGlowTex = CalamityUtils.GetTextureEfficient(ref JawGlow, "CalamityMod/Projectiles/Summon/VoidEaterMarionetteJawGlow").Value;
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 1);
            
            //This draws the jaws at the desired opening amount. The jaw glow doesn't draw during Divinity Devourer as that's the pink attack and this glow is blue
            Vector2 jawOffset = new Vector2(10, -18);
            Main.spriteBatch.Draw(jawTex, Projectile.Center - jawOffset.RotatedBy(Projectile.rotation) - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation - JawOpeningAmount, jawTex.Size() / 2 - jawOffset, Projectile.scale, SpriteEffects.None, 1);
            if (CurrentAttack != AttackState.DivinityDevourer)
                Main.spriteBatch.Draw(jawGlowTex, Projectile.Center - jawOffset.RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation - JawOpeningAmount, jawTex.Size() / 2 - jawOffset, Projectile.scale, SpriteEffects.None, 1);
            jawOffset.X *= -1;
            Main.spriteBatch.Draw(jawTex, Projectile.Center - jawOffset.RotatedBy(Projectile.rotation) - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + JawOpeningAmount, jawTex.Size() / 2 - jawOffset, Projectile.scale, SpriteEffects.FlipHorizontally, 1);
            if (CurrentAttack != AttackState.DivinityDevourer)
                Main.spriteBatch.Draw(jawGlowTex, Projectile.Center - jawOffset.RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation + JawOpeningAmount, jawTex.Size() / 2 - jawOffset, Projectile.scale, SpriteEffects.FlipHorizontally, 1);

            return false;
        }
        void DrawSegmentGlow(BaseWormSegment segment)
        {
            var color = Lighting.GetColor(segment.Center.ToTileCoordinates());
            //pink glows don't glow during Faith Incinerator while blue glows don't glow during divinity devourer
            if (!SegmentTextureAssetsGlow.IndexInRange(segment.segmentType) || (segment.segmentType == 0 && CurrentAttack == AttackState.FaithIncinerator) || (segment.segmentType == 1 && CurrentAttack == AttackState.DivinityDevourer))
            {
                return;
            }
            var tex = SegmentTextureAssetsGlow[segment.segmentType].Value;
            Main.spriteBatch.Draw(tex, segment.Center - Main.screenPosition, null, Color.White * segment.Opacity, segment.rotation, tex.Size() / 2 + (SegmentTypeDrawOffsets[segment.segmentType]), Projectile.scale, SpriteEffects.None, 1);
        }
        public List<Asset<Texture2D>> SegmentTextureAssetsGlow
        {
            get
            {
                if (internalTexAssetsGlow.Count == 0)
                    for (var i = 0; i < SegmentTextures.Count; i++)
                    {
                        internalTexAssetsGlow.Add(ModContent.Request<Texture2D>(SegmentTextures[i] + "Glow"));
                        if (SegmentTypeDrawOffsets.Count <= i)
                        {
                            SegmentTypeDrawOffsets.Add(Vector2.Zero);
                        }
                    }
                return internalTexAssetsGlow;
            }
        }
        private List<Asset<Texture2D>> internalTexAssetsGlow = new List<Asset<Texture2D>>();

        private Asset<Texture2D> GlowTexAsset;
        private Asset<Texture2D> Jaws;
        private Asset<Texture2D> JawGlow;
        private Asset<Texture2D> DoGJaws;
        public override void PostDraw(Color lightColor)
        {
            for (int i = Segments.Count - 1; i >= 0; i--)
            {
                DrawSegmentGlow(Segments[i]);
            }
            //Faith Incinerator doesn't use this glow as it's the blue attack and this glow is pink
            if (CurrentAttack != AttackState.FaithIncinerator)
                Main.EntitySpriteDraw(CalamityUtils.GetTextureEfficient(ref GlowTexAsset, GlowTexture).Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, CalamityUtils.GetTextureEfficient(ref GlowTexAsset, GlowTexture).Size() *0.5f, Projectile.scale, SpriteEffects.None, 0);

            if (Projectile.Opacity > 0 && CurrentAttack == AttackState.UltracosmicMaelstrom && AiTimer > 0)
            {
                var tex = CalamityUtils.GetTextureEfficient(ref DoGJaws, "CalamityMod/Particles/Jaws").Value;
                Main.spriteBatch.SetBlendState(BlendState.Additive);
                Main.spriteBatch.Draw(tex, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 12 - Main.screenPosition, null, Color.Fuchsia, Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() * 0.5f, 0.7f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 12 - Main.screenPosition, null, Color.Aqua, Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() * 0.5f, 0.6f, SpriteEffects.None, 0);
                Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            }
        }
    }
}
