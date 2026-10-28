using CalamityMod.Systems;
using Terraria;

namespace CalamityMod.BiomeManagers
{
    public class GleamingBurrowsBiome : SunkenSeaSubBiome
    {
        public override bool IsBiomeActive(Player player)
        {
            bool MoreThanReefs = BiomeTileCounterSystem.SunkenSeaBurrowsTiles > BiomeTileCounterSystem.SunkenSeaReefsTiles;
            bool MoreThanPolyp = BiomeTileCounterSystem.SunkenSeaBurrowsTiles > BiomeTileCounterSystem.SunkenSeaPolypTiles;

            return BiomeTileCounterSystem.SunkenSeaBurrowsTiles > 3000 && MoreThanReefs && MoreThanPolyp;
        }
    }
}
