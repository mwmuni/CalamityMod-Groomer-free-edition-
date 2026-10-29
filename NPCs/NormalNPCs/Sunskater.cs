using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.NPCs.ExoMechs.Ares;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class Sunskater : HostileFlyingEnemyBase
    {
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/Sunskater") { Volume = 0.9f };

        private bool hasBeenHit = false;

        protected override bool HasBeenHit
        {
            get => hasBeenHit;
            set => hasBeenHit = value;
        }

        protected override float IdleHorizontalSpeedThreshold => 2f;
        protected override float FrameIncrementAfterHit => 0.15f;
        protected override float FrameIncrementBeforeHit => 0.075f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.damage = 20;
            NPC.width = 58;
            NPC.height = 22;
            NPC.defense = 4;
            NPC.lifeMax = 160;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(silver: 3);
            NPC.HitSound = SoundID.NPCHit50;
            NPC.DeathSound = Main.zenithWorld ? AresGaussNuke.NukeExplosionSound : DeathSound;
            NPC.knockBackResist = 0.7f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SunskaterBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Sunskater")
            });
        }

        protected override void UpdateAttackMovement()
        {
            NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.15f;
            NPC.velocity.Y = NPC.velocity.Y + (float)NPC.directionY * 0.15f;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -10f, 10f);
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -10f, 10f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe)
            {
                return 0f;
            }
            return SpawnCondition.Sky.Chance * 0.15f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                if (Main.zenithWorld)
                {
                    target.AddBuff(ModContent.BuffType<HolyInferno>(), 150);

                    if (Main.rand.NextBool(3))
                        target.AddBuff(BuffID.OnFire, 180);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(BuffID.OnFire3, 120);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(BuffID.CursedInferno, 120);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(BuffID.Frostburn, 180);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(BuffID.Frostburn2, 120);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(BuffID.Burning, 60);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(ModContent.BuffType<Daybroken>(), 120);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 90);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(ModContent.BuffType<Dragonfire>(), 90);
                    if (Main.rand.NextBool(3))
                        target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 90);
                }
                else
                    target.AddBuff(BuffID.OnFire, 180);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddIf(() => Main.hardMode, ModContent.ItemType<EssenceofSunlight>(), 2);

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.YellowTorch, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.YellowTorch, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.zenithWorld)
                {
                    float screenShakePower = 16 * Utils.GetLerpValue(1300f, 0f, NPC.Distance(Main.LocalPlayer.Center), true);
                    Main.LocalPlayer.SetScreenshake(screenShakePower);
                }
            }
        }
    }
}
