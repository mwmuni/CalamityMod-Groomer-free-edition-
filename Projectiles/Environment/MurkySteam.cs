using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Environment
{
    public class MurkySteam : SteamBase, IAdditiveDrawer, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";

        protected override Color DrawColor => new Color(14, 138, 92);

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // CIT 23MAY2026: Vanilla's logic for granting "Burning immunity" is incredibly stupid because Player.fireWalk actually just prevents running hot tile touch code.
            // Thus we must add a manual check for Player.fireWalk here too.
            if (info.Damage <= 0 || target.fireWalk)
                return;

            target.AddBuff(BuffID.Burning, 180);
        }
    }
}
