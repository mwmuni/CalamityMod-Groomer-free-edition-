using CalamityMod.Projectiles.BaseProjectiles;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class GleamingDaggerProj : BaseHomingCoinDaggerProjectile
    {
        protected override int CoinDustType => DustID.PlatinumCoin;
    }
}
