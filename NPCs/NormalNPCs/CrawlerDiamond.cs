using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrawlerDiamond : GemCrawlerBase
    {
        protected override int Damage => 24;
        protected override int Defense => 10;
        protected override int LifeMax => 180;
        protected override float KnockBackResist => 0.35f;
        protected override int NPCValue => Item.buyPrice(silver: 2);
        protected override int BannerItemType => ModContent.ItemType<DiamondCrawlerBanner>();
        protected override int GemItemID => ItemID.Diamond;
        protected override int DustType => DustID.GemDiamond;
        protected override string GoreName => "CrawlerDiamond";
        protected override float SpeedDetect => 6f;
        protected override float SpeedAdditive => 0.06f;
        protected override IBestiaryInfoElement BiomeBestiaryTag => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns;

        protected override float GetBiomeSpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Cavern.Chance * 0.025f;
    }
}
