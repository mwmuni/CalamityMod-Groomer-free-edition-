using CalamityMod.Items.Placeables.Banners;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class EbonianBlightSlime : BlightSlimeBase<EbonianBlightSlimeBanner>
    {
        protected override IBestiaryInfoElement BiomeTag => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption;
        protected override float SpawnConditionChance => SpawnCondition.Corruption.Chance;
        protected override int HitDustType => DustID.Demonite;
        protected override int HitDustCount => 3;
        protected override int PlayerDebuffType => BuffID.Weak;
        protected override int RareDropItemType => ItemID.Vitamins;
    }
}
