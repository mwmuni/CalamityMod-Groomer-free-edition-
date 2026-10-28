using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrawlerSapphire : GemCrawlerBase
    {
        protected override int Damage => 18;
        protected override int Defense => 6;
        protected override int LifeMax => 120;
        protected override float KnockBackResist => 0.65f;
        protected override int NPCValue => Item.buyPrice(silver: 1);
        protected override int BannerItemType => ModContent.ItemType<SapphireCrawlerBanner>();
        protected override int GemItemID => ItemID.Sapphire;
        protected override int DustType => DustID.BlueCrystalShard;
        protected override string GoreName => "CrawlerSapphire";
        protected override float SpeedDetect => 5.5f;
        protected override float SpeedAdditive => 0.055f;
        protected override IBestiaryInfoElement BiomeBestiaryTag => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground;

        protected override float GetBiomeSpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Underground.Chance * 0.0325f;
    }
}
