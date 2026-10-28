using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrawlerTopaz : GemCrawlerBase
    {
        protected override int Damage => 12;
        protected override int Defense => 4;
        protected override int LifeMax => 90;
        protected override float KnockBackResist => 0.85f;
        protected override int NPCValue => Item.buyPrice(copper: 60);
        protected override int BannerItemType => ModContent.ItemType<TopazCrawlerBanner>();
        protected override int GemItemID => ItemID.Topaz;
        protected override int DustType => DustID.t_Honey;
        protected override string GoreName => "CrawlerTopaz";
        protected override float SpeedDetect => 4.5f;
        protected override float SpeedAdditive => 0.045f;
        protected override IBestiaryInfoElement BiomeBestiaryTag => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground;

        protected override float GetBiomeSpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Underground.Chance * 0.04f;
    }
}
