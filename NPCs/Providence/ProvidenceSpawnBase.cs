using System;
using CalamityMod.Dusts;
using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Providence
{
    public abstract class ProvidenceSpawnBase : ModNPC
    {
        private bool start = true;

        protected abstract int Defense { get; }
        protected abstract float DamageReduction { get; }
        protected abstract int NormalLifeMax { get; }
        protected abstract int BossRushLifeMax { get; }
        protected abstract int Height { get; }
        protected abstract string GoreNamePrefix { get; }
        protected abstract int DeathDustCount { get; }

        protected abstract void RegisterWithGlobalNPC();

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[Type] = 10;
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            NPCID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.npcSlots = 1f;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.width = 228;
            NPC.height = Height;
            NPC.defense = Defense;
            NPC.DR_NERD(DamageReduction);
            NPC.lifeMax = NormalLifeMax;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = BossRushLifeMax;
            }
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            AIType = -1;
            NPC.HitSound = SoundID.NPCHit52;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void FindFrame(int frameHeight)
        {
            if (CalamityGlobalNPC.holyBoss < 0 || !Main.npc[CalamityGlobalNPC.holyBoss].active)
                NPC.frameCounter += 0.2f;
            else
                NPC.frameCounter += 0.12f + Main.npc[CalamityGlobalNPC.holyBoss].velocity.Length() / 120f;

            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            RegisterWithGlobalNPC();

            if (CalamityGlobalNPC.holyBoss < 0 || !Main.npc[CalamityGlobalNPC.holyBoss].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // Rotation
            NPC.rotation = Main.npc[CalamityGlobalNPC.holyBoss].velocity.X * 0.005f;

            NPC parent = Main.npc[CalamityGlobalNPC.holyBoss];
            if (start)
            {
                for (int d = 0; d < 30; d++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);

                NPC.ai[1] = NPC.ai[0];
                start = false;
            }

            float playerLocation = NPC.Center.X - Main.player[Main.npc[CalamityGlobalNPC.holyBoss].target].Center.X;
            NPC.direction = playerLocation < 0 ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            double deg = NPC.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = 450;
            NPC.position.X = parent.Center.X - (int)(Math.Cos(rad) * dist) - NPC.width / 2;
            NPC.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * dist) - NPC.height / 2;
            NPC.ai[1] += 1f;
        }

        public override void OnKill()
        {
            int heartAmt = Main.rand.Next(3) + 3;
            for (int i = 0; i < heartAmt; i++)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        string goreName = "ProfanedGuardianBoss" + GoreNamePrefix;
                        if (i == 2)
                            goreName += "2";
                        else if (i == 3)
                            goreName += "3";
                        else if (i == 4)
                            goreName += "4";
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(goreName).Type, 1f);
                    }
                }

                for (int k = 0; k < DeathDustCount; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);
            }
        }
    }
}
