using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    public abstract class DesertNuisanceTailBase : ModNPC
    {
        protected abstract int SegmentWidth { get; }
        protected abstract int SegmentHeight { get; }
        protected abstract int SegmentDefense { get; }
        protected abstract int GoodWorldDefenseBonus { get; }
        protected abstract int NormalLifeMax { get; }
        protected abstract int RevengeanceLifeMax { get; }
        protected abstract int BossRushLifeMax { get; }
        protected abstract int SegmentOffset { get; }
        protected abstract string DeathGoreName { get; }
        protected abstract bool HasHead();

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.damage = 10; // 20
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

            Vector2 segmentTilePos = NPC.Center;
            float playerXPos = Main.player[NPC.target].Center.X;
            float playerYPos = Main.player[NPC.target].Center.Y;
            playerXPos = (float)((int)(playerXPos / 16f) * 16);
            playerYPos = (float)((int)(playerYPos / 16f) * 16);
            segmentTilePos.X = (float)((int)(segmentTilePos.X / 16f) * 16);
            segmentTilePos.Y = (float)((int)(segmentTilePos.Y / 16f) * 16);
            playerXPos -= segmentTilePos.X;
            playerYPos -= segmentTilePos.Y;
            float playerDistance = (float)System.Math.Sqrt((double)(playerXPos * playerXPos + playerYPos * playerYPos));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    segmentTilePos = NPC.Center;
                    playerXPos = Main.npc[(int)NPC.ai[1]].Center.X - segmentTilePos.X;
                    playerYPos = Main.npc[(int)NPC.ai[1]].Center.Y - segmentTilePos.Y;
                }
                catch
                {
                }
                NPC.rotation = (float)System.Math.Atan2((double)playerYPos, (double)playerXPos) + MathHelper.PiOver2;
                playerDistance = (float)System.Math.Sqrt((double)(playerXPos * playerXPos + playerYPos * playerYPos));

                int segmentOffset = SegmentOffset;
                playerDistance = (playerDistance - segmentOffset) / playerDistance;
                playerXPos *= playerDistance;
                playerYPos *= playerDistance;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + playerXPos;
                NPC.position.Y = NPC.position.Y + playerYPos;

                if (playerXPos < 0f)
                    NPC.spriteDirection = 1;
                else if (playerXPos > 0f)
                    NPC.spriteDirection = -1;
            }
        }

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

            return minDist <= 30f * NPC.scale;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(DeathGoreName).Type, NPC.scale);

                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override bool CheckActive() => false;

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
    }
}
