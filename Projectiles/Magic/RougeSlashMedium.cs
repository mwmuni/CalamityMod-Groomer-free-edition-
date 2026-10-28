using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RougeSlashMedium : RougeSlashBase, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        protected override int ProjectileSize => 18;
    }
}
