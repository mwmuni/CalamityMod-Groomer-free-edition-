using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrawlerAmber : GemCrawlerBase
    {
        protected override int Damage => 25;
        protected override int Defense => 10;
        protected override int LifeMax => 150;
        protected override float KnockBackResist => 0.3f;
        protected override int NPCValue => Item.buyPrice(silver: 1);
        protected override int BannerItemType => ModContent.ItemType<AmberCrawlerBanner>();
        protected override int GemItemID => ItemID.Amber;
        protected override int DustType => DustID.Sand;
        protected override string GoreName => "CrawlerAmber";
        protected override float SpeedDetect => 5f;
        protected override float SpeedAdditive => 0.05f;
        protected override IBestiaryInfoElement BiomeBestiaryTag => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundDesert;
        protected override bool AITypeBeforeStats => true;

        protected override float GetBiomeSpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.DesertCave.Chance * 0.025f;

        protected override void SetVulnerabilities()
        {
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }
    }
}
