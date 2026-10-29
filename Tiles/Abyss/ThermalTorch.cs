using System.Linq;
using CalamityMod.Graphics;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Tiles.Abyss
{
    public class ThermalTorch : BaseCustomTorchTile<Items.Placeables.Furniture.ThermalTorch>
    {
        public override void NearbyEffects(int i, int j, bool closer)
        {
            //This makes the placed torch cut through the abyss darkness.
            var pos = new Point(i, j).ToWorldCoordinates();
            if (!closer && Main.LocalPlayer.Calamity().ZoneAbyss && !Main.gamePaused)
            {
                if (EnhancedDarknessSystem.lights.Any(x => x.center == pos))
                {
                    var e = EnhancedDarknessSystem.lights.First(x => x.center == pos);
                    e.lifetime = 5;
                }
                else
                    EnhancedDarknessSystem.lights.Add(new EnhancedDarknessSystem.LightSource(pos, scale: 2f) { lifetime = 5 });
            }
        }

        protected override Color TorchDustColor => new(200, 0, 0);
        protected override Vector3 TorchLightColor => new(2f, 0.5f, 0.5f);
        protected override string FlameTexturePath => "CalamityMod/Tiles/Abyss/ThermalTorchFlame";
        protected override int FlameSparkDustType => DustID.RedTorch;
        protected override bool WaterImmune => true;
        protected override bool LavaImmune => true;
        protected override bool IsInTorchLuckBiome(Player player) => player.Calamity().ZoneAbyss;
    }
}
