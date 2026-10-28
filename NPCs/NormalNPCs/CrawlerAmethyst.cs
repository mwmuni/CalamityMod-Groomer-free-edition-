using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrawlerAmethyst : GemCrawlerBase
    {
        protected override int Damage => 15;
        protected override int Defense => 4;
        protected override int LifeMax => 75;
        protected override float KnockBackResist => 0.75f;
        protected override int NPCValue => Item.buyPrice(copper: 60);
        protected override int BannerItemType => ModContent.ItemType<AmethystCrawlerBanner>();
        protected override int GemItemID => ItemID.Amethyst;
        protected override int DustType => DustID.PurpleCrystalShard;
        protected override string GoreName => "CrawlerAmethyst";
        protected override float SpeedDetect => 4.5f;
        protected override float SpeedAdditive => 0.045f;
        protected override IBestiaryInfoElement BiomeBestiaryTag => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground;

        protected override float GetBiomeSpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Underground.Chance * 0.04f;
    }
}
