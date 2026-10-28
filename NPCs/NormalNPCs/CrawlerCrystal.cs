using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrawlerCrystal : GemCrawlerBase
    {
        protected override int Damage => 35;
        protected override int Defense => 22;
        protected override int LifeMax => 400;
        protected override float KnockBackResist => 0.15f;
        protected override int NPCValue => Item.buyPrice(silver: 5);
        protected override int BannerItemType => ModContent.ItemType<CrystalCrawlerBanner>();
        protected override int GemItemID => ItemID.CrystalShard;
        protected override int DustType => DustID.BlueCrystalShard;
        protected override string GoreName => "CrawlerCrystal";
        protected override float SpeedDetect => 7f;
        protected override float SpeedAdditive => 0.07f;
        protected override IBestiaryInfoElement BiomeBestiaryTag => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundHallow;

        protected override float GetBiomeSpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.EnchantedSword.Chance;
    }
}
