using System;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class ImpiousImmolator : HostileFlyingEnemyBase
    {
        public bool hasBeenHit = false;

        protected override bool HasBeenHit
        {
            get => hasBeenHit;
            set => hasBeenHit = value;
        }

        protected override float IdleHorizontalSpeedThreshold => 3f;
        protected override float FrameIncrementAfterHit => 0.25f;
        protected override float FrameIncrementBeforeHit => 0.125f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.NeedsExpertScaling[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.damage = 0; // 0 contact damage, projectile damage is handled separately
            NPC.width = 60;
            NPC.height = 60;
            NPC.defense = 30;
            NPC.lifeMax = 3000;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.knockBackResist = 0.2f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ImpiousImmolatorBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.ImpiousImmolator")
            });
        }

        protected override void UpdateAttackMovement()
        {
            NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.2f;
            NPC.velocity.Y = NPC.velocity.Y + (float)NPC.directionY * 0.2f;
            float velocityMax = CalamityWorld.death ? 16f : CalamityWorld.revenge ? 14f : 12f;
            if (NPC.velocity.X > velocityMax)
            {
                NPC.velocity.X = velocityMax;
            }
            if (NPC.velocity.X < -velocityMax)
            {
                NPC.velocity.X = -velocityMax;
            }
            if (NPC.velocity.Y > velocityMax)
            {
                NPC.velocity.Y = velocityMax;
            }
            if (NPC.velocity.Y < -velocityMax)
            {
                NPC.velocity.Y = -velocityMax;
            }

            if (NPC.justHit)
                NPC.localAI[0] = 0f;

            NPC.localAI[0] += 1f;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] >= (CalamityWorld.death ? 50f : CalamityWorld.revenge ? 70f : 90f))
            {
                NPC.localAI[0] = 0f;
                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    float speed = 12f;
                    Vector2 beamPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)(NPC.height / 2));
                    float targetXDist = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - beamPosition.X + (float)Main.rand.Next(-20, 21);
                    float targetYDist = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - beamPosition.Y + (float)Main.rand.Next(-20, 21);
                    float targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
                    targetDistance = speed / targetDistance;
                    targetXDist *= targetDistance;
                    targetYDist *= targetDistance;
                    int damage = Main.masterMode ? 35 : Main.expertMode ? 42 : 55;
                    int beam = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, targetXDist, targetYDist, ModContent.ProjectileType<FlameBurstHostile>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[beam].tileCollide = true;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !NPC.downedMoonlord || spawnInfo.Player.Calamity().ZoneCalamity || Main.pumpkinMoon || Main.snowMoon || Main.eclipse)
            {
                return 0f;
            }
            if (SpawnCondition.Underworld.Chance > 0f)
            {
                return SpawnCondition.Underworld.Chance / 4f;
            }
            return SpawnCondition.OverworldHallow.Chance / 4f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<UnholyEssence>(), 1, 2, 4);
            npcLoot.Add(ModContent.ItemType<SanctifiedSpark>(), 15);
            npcLoot.Add(ModContent.ItemType<BlasphemousDonut>(), 20);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ImpiousImmolator").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ImpiousImmolator2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ImpiousImmolator3").Type, 1f);
                }
            }
        }
    }
}
