using CalamityMod.Systems;
using Terraria;

namespace CalamityMod.BiomeManagers
{
    public class RadiantReefsBiome : SunkenSeaSubBiome
    {
        public override bool IsBiomeActive(Player player)
        {
            bool MoreThanShores = BiomeTileCounterSystem.SunkenSeaReefsTiles > BiomeTileCounterSystem.SunkenSeaShoresTiles;
            bool MoreThanPolyp = BiomeTileCounterSystem.SunkenSeaReefsTiles > BiomeTileCounterSystem.SunkenSeaPolypTiles;
            bool MoreThanBurrows = BiomeTileCounterSystem.SunkenSeaReefsTiles > BiomeTileCounterSystem.SunkenSeaBurrowsTiles;
            bool MoreThanBasalt = BiomeTileCounterSystem.SunkenSeaReefsTiles > BiomeTileCounterSystem.SunkenSeaBasaltTiles;

            return ((BiomeTileCounterSystem.SunkenSeaReefsTiles > 2000) && MoreThanBasalt && MoreThanBurrows && MoreThanPolyp && MoreThanShores);
        }
    }
}
