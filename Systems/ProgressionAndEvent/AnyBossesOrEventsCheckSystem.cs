using CalamityMod.CalPlayer;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Skies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AnyBossesOrEventsCheckSystem : ModSystem
    {
        public override void PreUpdateEntities()
        {
            // Bool for if the SCal ritual effect is ongoing.
            // Only the client-side SCal sky reads this, so dedicated servers skip the projectile scan entirely.
            if (!Main.dedServ)
                SCalSky.RitualDramaProjectileIsPresent = CalamityUtils.AnyProjectiles(ModContent.ProjectileType<SCalRitualDrama>());

            // Bool for any existing bosses, true if any boss NPC is active.
            CalamityPlayer.areThereAnyDamnBosses = CalamityUtils.AnyBossNPCS();

            // Bool for any existing events, true if any event is active.
            // Player variable, always finds the closest player relative to the center of the map.
            int closestPlayer = Player.FindClosest(new Vector2(Main.maxTilesX / 2, (float)Main.worldSurface / 2f) * 16f, 0, 0);
            Player player = Main.player[closestPlayer];
            CalamityPlayer.areThereAnyDamnEvents = CalamityGlobalNPC.AnyEvents(player);
        }
    }
}
