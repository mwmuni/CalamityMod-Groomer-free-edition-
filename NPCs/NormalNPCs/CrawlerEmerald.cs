using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrawlerEmerald : GemCrawlerBase
    {
        protected override int Damage => 20;
        protected override int Defense => 6;
        protected override int LifeMax => 135;
        protected override float KnockBackResist => 0.55f;
        protected override int NPCValue => Item.buyPrice(silver: 1, copper: 20);
        protected override int BannerItemType => ModContent.ItemType<EmeraldCrawlerBanner>();
        protected override int GemItemID => ItemID.Emerald;
        protected override int DustType => DustID.GemEmerald;
        protected override string GoreName => "CrawlerEmerald";
        protected override float SpeedDetect => 5.5f;
        protected override float SpeedAdditive => 0.055f;
        protected override IBestiaryInfoElement BiomeBestiaryTag => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns;

        protected override float GetBiomeSpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Cavern.Chance * 0.0325f;
    }
}
