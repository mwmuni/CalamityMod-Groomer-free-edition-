using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
    [HasPierceResist]
    [LongDistanceNetSync(SyncWith = typeof(PerforatorHeadLarge))]
    public class PerforatorBodyLarge : PerforatorSegmentBase<PerforatorHeadLarge>
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/PerfLargeHit", 3);
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/PerfLargeDeath");

        public static Asset<Texture2D> AltTexture;
        public static Asset<Texture2D> Texture_Glow;
        public static Asset<Texture2D> AltTexture_Glow;


        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.PerforatorHeadLarge.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            if (!Main.dedServ)
            {
                AltTexture = ModContent.Request<Texture2D>(Texture + "Alt", AssetRequestMode.AsyncLoad);
                AltTexture_Glow = ModContent.Request<Texture2D>(Texture + "AltGlow", AssetRequestMode.AsyncLoad);
                Texture_Glow = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.damage = 20; // 40
            NPC.width = 60;
            NPC.height = 60;
            NPC.defense = 8;

            NPC.LifeMaxNERB(2000, 2600, 80000);
            if (Main.zenithWorld)
                NPC.lifeMax *= 4;

            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.netAlways = true;
            NPC.dontCountMe = true;

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = NPC.localAI[3] == 1f ? AltTexture.Value : TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[Type].Value.Width / 2), (float)(TextureAssets.Npc[Type].Value.Height / 2));

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = NPC.localAI[3] == 1f ? AltTexture_Glow.Value : Texture_Glow.Value;
            Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("LargePerf2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("LargePerf3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("LargePerf4").Type, NPC.scale);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 180, true);
        }
    }
}
