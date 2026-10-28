using CalamityMod.Systems;
using Terraria;

namespace CalamityMod.BiomeManagers
{
    public class BasaltGullyBiome : SunkenSeaSubBiome
    {
        // Placeholder until we get a dedicated Basalt Gully background
        public override string BackgroundPath => "Terraria/Images/MapBG3";
        public override string MapBackground => "Terraria/Images/MapBG3";

        public override bool IsBiomeActive(Player player)
        {
            bool MoreThanShores = BiomeTileCounterSystem.SunkenSeaBasaltTiles > BiomeTileCounterSystem.SunkenSeaShoresTiles;
            bool MoreThanReefs = BiomeTileCounterSystem.SunkenSeaBasaltTiles > BiomeTileCounterSystem.SunkenSeaReefsTiles;
            bool MoreThanPolyp = BiomeTileCounterSystem.SunkenSeaBasaltTiles > BiomeTileCounterSystem.SunkenSeaPolypTiles;
            bool MoreThanBurrows = BiomeTileCounterSystem.SunkenSeaBasaltTiles > BiomeTileCounterSystem.SunkenSeaBurrowsTiles;
            bool MoreThanUnderground = BiomeTileCounterSystem.SunkenSeaShoresTiles > BiomeTileCounterSystem.UndergroundTiles;

            return BiomeTileCounterSystem.SunkenSeaBasaltTiles > 2500 && (MoreThanShores || MoreThanReefs || MoreThanPolyp || MoreThanBurrows);
        }
    }
}
