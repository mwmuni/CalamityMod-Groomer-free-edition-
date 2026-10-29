using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.BossTrophies
{
    public abstract class BaseBossTrophy : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpTrophy();
    }
}
