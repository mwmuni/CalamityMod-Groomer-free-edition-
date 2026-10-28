using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Waters;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public abstract class SunkenSeaSubBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => SunkenSeaBurrowsWater.Instance;
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("CalamityMod/SunkenSeaBGStyle");
        public override int BiomeTorchItemType => ModContent.ItemType<NavyPrismTorch>();
        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("SunkenSea") ?? MusicID.OceanNight;
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/SunkenSeaIcon";
        // Placeholder until we get a dedicated Sunken Sea background
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer1";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer1";
    }
}
