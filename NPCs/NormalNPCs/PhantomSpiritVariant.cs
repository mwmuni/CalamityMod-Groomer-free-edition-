using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public abstract class PhantomSpiritVariant : ModNPC
    {
        protected abstract int VariantDamage { get; }
        protected abstract int VariantDefense { get; }
        protected abstract int VariantLifeMax { get; }
        protected abstract float VariantKnockBackResist { get; }
        protected abstract int ValueInSilver { get; }
        protected abstract float MovementSpeed { get; }
        protected abstract int MinimumNecroplasmDrop { get; }
        protected abstract int MaximumNecroplasmDrop { get; }
        protected virtual float? ScaleMultiplier => null;

        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.PhantomSpirit.DisplayName");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
        }

        public override void SetDefaults()
        {
            SetAIStyleAndDamage();
            NPC.width = 32;
            NPC.height = 80;
            if (ScaleMultiplier is float scaleMultiplier)
                NPC.scale *= scaleMultiplier;
            NPC.defense = VariantDefense;
            NPC.lifeMax = VariantLifeMax;
            NPC.knockBackResist = VariantKnockBackResist;
            AIType = -1;
            NPC.value = Item.buyPrice(silver: ValueInSilver);
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            Banner = ModContent.NPCType<PhantomSpirit>();
            BannerItem = ModContent.ItemType<PhantomSpiritBanner>();
            NPC.Calamity().VulnerableToSickness = false;
        }

        protected virtual void SetAIStyleAndDamage()
        {
            NPC.aiStyle = -1;
            NPC.damage = VariantDamage;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.PhantomSpirit")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityRegularEnemyAI.DungeonSpiritAI(NPC, Mod, MovementSpeed, -MathHelper.PiOver2);
            int polterDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Necroplasm, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[polterDust];
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Necroplasm, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int hitPolterDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Necroplasm, NPC.velocity.X, NPC.velocity.Y, 0, default, 1f);
                    Dust dust = Main.dust[hitPolterDust];
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.scale = 1.4f;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor) => new Color(200, 200, 200, 0);

        public override void ModifyNPCLoot(NPCLoot npcLoot) =>
            npcLoot.Add(ModContent.ItemType<Necroplasm>(), 1, MinimumNecroplasmDrop, MaximumNecroplasmDrop);
    }
}
