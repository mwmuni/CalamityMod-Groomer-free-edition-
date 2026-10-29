using CalamityMod.Projectiles.BaseProjectiles;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class GildedDaggerProj : BaseHomingCoinDaggerProjectile
    {
        protected override int CoinDustType => DustID.GoldCoin;
    }
}
