using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RougeSlashLarge : RougeSlashBase, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        protected override int ProjectileSize => 24;
    }
}
