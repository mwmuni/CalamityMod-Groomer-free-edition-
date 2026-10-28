using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Environment
{
    public class HotSteam : SteamBase, IAdditiveDrawer, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";

        protected override Color DrawColor => new Color(159, 207, 181);

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(BuffID.Poisoned, 180);
        }
    }
}
