using System;
using System.IO;
using System.Runtime.CompilerServices;
using CalamityMod.FluidSimulation;
using CalamityMod.Items;
using CalamityMod.MainMenu;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using log4net;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

[assembly: InternalsVisibleTo("CalTestHelpers")]
[assembly: InternalsVisibleTo("InfernumMode")]
namespace CalamityMod
{
    public class CalamityMod : Mod
    {
        #region External Flags
        // External flag to disable Defense Damage
        // This can be edited by other mods using reflection if desired
        // Note that this flag trumps Bloodflare Core and will stop that accessory from working properly.
        // There is also a means to disable defense damage on a per-player basis.
        public static bool ExternalFlag_DisableDefenseDamage = false;
        #endregion

        internal static CalamityMod Instance => _Instance ??= ModContent.GetInstance<CalamityMod>();
        private static CalamityMod _Instance;

        // This should not be named as 'Logger' as it hides Instance Property 'Logger'
        internal static ILog Log => Instance.Logger;

        #region Load
        public override void Load()
        {
            // Initialize the EnemyStats struct as early as it is safe to do so
            NPCStats.LoadDebuffs();

            // Initialize Calamity Balance, since it is tightly coupled with the remaining systems
            CalamityGlobalItem.LoadTweaks();
            CalamityGlobalProjectile.LoadTweaks();

            // All of Calamity's ILoadables (and therefore its equip textures) have loaded by the time Mod.Load runs.
            CalamityUtils.EquipSlotLookupsFinal = true;

            if (!Main.dedServ)
            {
                // This must be done separately from immediate loading, as loading is now multithreaded.
                // However, render targets and certain other graphical objects can only be created on the main thread.
                Main.QueueMainThreadAction(() => Main.OnPreDraw += PrepareRenderTargets);
            }
        }
        #endregion

        #region Unload
        public override void Unload()
        {
            NPCStats.UnloadDebuffs();
            CalamityGlobalItem.UnloadTweaks();
            CalamityGlobalProjectile.UnloadTweaks();
            CalamityUtils.UnloadEquipSlotCache();

            Main.QueueMainThreadAction(() =>
            {
                Main.OnPreDraw -= PrepareRenderTargets;
            });

            _Instance = null;
            base.Unload();
        }
        #endregion

        #region Render Target Management

        public static void PrepareRenderTargets(GameTime gameTime)
        {
            DeathAshParticle.PrepareRenderTargets();
            FluidFieldManager.Update();
        }
        #endregion Render Target Management

        #region Music

        // This function returns an available Calamity Music Mod track, or null if the Calamity Music Mod is not available.
        public int? GetMusicFromMusicMod(string songFilename) => ExternalMods.MusicAvailable ? MusicLoader.GetMusicSlot(ExternalMods.musicMod, "Sounds/Music/" + songFilename) : null;

        // This function returns an available VCMM track, or null if VCMM is not available.
        // Unlike the main Music Mod, VCMM is hierarchical.
        public int? GetMusicFromVCMM(string songPath) => ExternalMods.VCMMAvailable ? MusicLoader.GetMusicSlot(ExternalMods.vcmm, "Assets/" + songPath) : null;

        #endregion

        #region Mod Support
        public override object Call(params object[] args) => ModCalls.Call(args);
        #endregion

        #region Seasons
        public static Season CurrentSeason
        {
            get
            {
                DateTime date = DateTime.Now;
                int day = date.DayOfYear - Convert.ToInt32(DateTime.IsLeapYear(date.Year) && date.DayOfYear > 59);

                if (day < 80 || day >= 355)
                {
                    return Season.Winter;
                }

                else if (day >= 80 && day < 172)
                {
                    return Season.Spring;
                }

                else if (day >= 172 && day < 266)
                {
                    return Season.Summer;
                }

                else
                {
                    return Season.Fall;
                }
            }
        }
        #endregion

        #region Netcode
        public override void HandlePacket(BinaryReader reader, int whoAmI) => CalamityNetcode.HandlePacket(this, reader, whoAmI);
        #endregion
    }
}
