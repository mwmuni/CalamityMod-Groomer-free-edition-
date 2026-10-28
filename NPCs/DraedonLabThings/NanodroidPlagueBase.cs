using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DraedonLabThings
{
    public abstract class NanodroidPlagueBase : ModNPC
    {
        protected abstract int CatchItemType { get; }
        protected abstract float LightRed { get; }
        protected abstract float LightGreen { get; }
        protected abstract float LightBlue { get; }
        protected abstract Asset<Texture2D> GlowTextureAsset { get; }

        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.Nanodroid.DisplayName");

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[Type] = 8;
            NPCID.Sets.CountsAsCritter[Type] = true;
            Main.npcCatchable[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.LightningBug);
            NPC.width = 16;
            NPC.height = 12;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.catchItem = (short)CatchItemType;
        }

        public override void AI()
        {
            if (NPC.localAI[2] > 3f)
            {
                Lighting.AddLight(NPC.Center, LightRed, LightGreen, LightBlue);
            }
            foreach (Player player in Main.ActivePlayers)
            {
                if (NPC.Hitbox.Intersects(player.HitboxForBestiaryNearbyCheck))
                {
                    NPC nPC = new NPC();
                    nPC.SetDefaults(ModContent.NPCType<Nanodroid>());
                    Main.BestiaryTracker.Sights.RegisterWasNearby(nPC);
                    break;
                }
            }
            // The only purpose of this is to make them not take forever to appear when shimmering Plagued Containment Brick
            if (NPC.shimmerTransparency > 0f)
                NPC.shimmerTransparency -= 0.05f;
        }

        public override bool? CanBeHitByItem(Player player, Item item) => null;

        public override bool? CanBeHitByProjectile(Projectile projectile) => null;

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter += 0.3f;
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 6; i++)
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Electric);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D critterTexture = TextureAssets.Npc[Type].Value;
            Texture2D glowmask = GlowTextureAsset.Value;
            Vector2 drawPosition = NPC.Center - screenPos + Vector2.UnitY * NPC.gfxOffY;
            SpriteEffects direction = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(critterTexture, drawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            spriteBatch.Draw(glowmask, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            return false;
        }
    }
}
