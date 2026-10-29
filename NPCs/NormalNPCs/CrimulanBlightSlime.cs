using CalamityMod.Items.Placeables.Banners;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrimulanBlightSlime : BlightSlimeBase<CrimulanBlightSlimeBanner>
    {
        protected override IBestiaryInfoElement BiomeTag => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson;
        protected override float SpawnConditionChance => SpawnCondition.Crimson.Chance;
        protected override int HitDustType => DustID.Blood;
        protected override int HitDustCount => 5;
        protected override int PlayerDebuffType => BuffID.Darkness;
        protected override int RareDropItemType => ItemID.Blindfold;
    }
}
