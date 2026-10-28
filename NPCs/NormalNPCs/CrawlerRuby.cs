using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrawlerRuby : GemCrawlerBase
    {
        protected override int Damage => 23;
        protected override int Defense => 8;
        protected override int LifeMax => 150;
        protected override float KnockBackResist => 0.45f;
        protected override int NPCValue => Item.buyPrice(silver: 1, copper: 60);
        protected override int BannerItemType => ModContent.ItemType<RubyCrawlerBanner>();
        protected override int GemItemID => ItemID.Ruby;
        protected override int DustType => DustID.HeartCrystal;
        protected override string GoreName => "CrawlerRuby";
        protected override float SpeedDetect => 6f;
        protected override float SpeedAdditive => 0.06f;
        protected override IBestiaryInfoElement BiomeBestiaryTag => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns;

        protected override float GetBiomeSpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Cavern.Chance * 0.025f;
    }
}
