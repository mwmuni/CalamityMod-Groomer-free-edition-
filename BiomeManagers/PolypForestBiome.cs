using CalamityMod.Systems;
using Terraria;

namespace CalamityMod.BiomeManagers
{
    public class PolypForestBiome : SunkenSeaSubBiome
    {
        public override bool IsBiomeActive(Player player)
        {
            bool MoreThanReefs = BiomeTileCounterSystem.SunkenSeaPolypTiles > BiomeTileCounterSystem.SunkenSeaReefsTiles;
            bool MoreThanBurrows = BiomeTileCounterSystem.SunkenSeaPolypTiles > BiomeTileCounterSystem.SunkenSeaBurrowsTiles;

            return BiomeTileCounterSystem.SunkenSeaPolypTiles > 200 && MoreThanReefs && MoreThanBurrows;
        }
    }
}
