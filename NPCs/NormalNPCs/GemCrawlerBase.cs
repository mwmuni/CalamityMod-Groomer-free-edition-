using System.IO;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public abstract class GemCrawlerBase : ModNPC
    {
        private bool detected = false;

        protected abstract int Damage { get; }
        protected abstract int Defense { get; }
        protected abstract int LifeMax { get; }
        protected abstract float KnockBackResist { get; }
        protected abstract int NPCValue { get; }
        protected abstract int BannerItemType { get; }
        protected abstract int GemItemID { get; }
        protected abstract int DustType { get; }
        protected abstract string GoreName { get; }
        protected abstract float SpeedDetect { get; }
        protected abstract float SpeedAdditive { get; }
        protected abstract IBestiaryInfoElement BiomeBestiaryTag { get; }
        protected abstract float GetBiomeSpawnChance(NPCSpawnInfo spawnInfo);

        protected virtual bool AITypeBeforeStats => false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 0.3f;
            NPC.aiStyle = -1;
            if (AITypeBeforeStats)
                AIType = -1;
            NPC.damage = Damage;
            NPC.width = 44;
            NPC.height = 34;
            NPC.defense = Defense;
            NPC.lifeMax = LifeMax;
            NPC.knockBackResist = KnockBackResist;
            if (!AITypeBeforeStats)
                AIType = -1;
            NPC.value = NPCValue;
            NPC.HitSound = SoundID.NPCHit33;
            NPC.DeathSound = SoundID.NPCDeath36;
            Banner = NPC.type;
            BannerItem = BannerItemType;
            SetVulnerabilities();
        }

        protected virtual void SetVulnerabilities()
        {
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BiomeBestiaryTag,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.GemCrawler")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(detected);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            detected = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            if (!detected)
            {
                NPC.frame.Y = frameHeight * 4;
                NPC.frameCounter = 0.0;
                return;
            }
            NPC.spriteDirection = -NPC.direction;
            NPC.frameCounter += (double)(NPC.velocity.Length() / 8f);
            if (NPC.frameCounter > 2.0)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0;
            }
        }

        public override void AI()
        {
            if (!detected)
                NPC.TargetClosest();
            if (((Main.player[NPC.target].Center - NPC.Center).Length() < 100f && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position,
                    Main.player[NPC.target].width, Main.player[NPC.target].height)) || NPC.justHit)
                detected = true;
            if (!detected)
                return;
            CalamityRegularEnemyAI.GemCrawlerAI(NPC, Mod, SpeedDetect, SpeedAdditive);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().InAnyCalamityBiome)
            {
                return 0f;
            }
            return GetBiomeSpawnChance(spawnInfo);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustType, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustType, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(GoreName).Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(GoreName + "2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Crawler").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Crawler2").Type, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(GemItemID, 1, 2, 4);
            npcLoot.Add(ModContent.ItemType<ScuttlersJewel>(), 6);
            npcLoot.Add(ModContent.ItemType<XyksBlessingBlue>(), 25);
        }
    }
}
