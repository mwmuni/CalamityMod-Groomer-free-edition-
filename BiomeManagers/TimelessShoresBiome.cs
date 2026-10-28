using CalamityMod.Systems;
using Terraria;

namespace CalamityMod.BiomeManagers
{
    public class TimelessShoresBiome : SunkenSeaSubBiome
    {
        public override bool IsBiomeActive(Player player)
        {
            bool MoreThanReefs = BiomeTileCounterSystem.SunkenSeaShoresTiles > BiomeTileCounterSystem.SunkenSeaReefsTiles;
            bool MoreThanBasalt = BiomeTileCounterSystem.SunkenSeaShoresTiles > BiomeTileCounterSystem.SunkenSeaBasaltTiles;
            bool MoreThanUnderground = BiomeTileCounterSystem.SunkenSeaShoresTiles > BiomeTileCounterSystem.UndergroundTiles;

            return (BiomeTileCounterSystem.SunkenSeaShoresTiles > 1200) && MoreThanReefs && MoreThanBasalt && MoreThanUnderground;
        }
    }
}
