using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class GungeonMusicSystem : ModSystem
    {
        // Copied straight from the ORDER easter egg

        private static readonly SoundStyle GungeonTrack = new("CalamityMod/Sounds/Custom/GungeonCreditMusic", SoundType.Music);
        private static SlotId gungeonSoundSlot;

        internal static float DefaultMusicTime = 100f;
        internal static float DefaultResetTime = 240f;

        internal static float remainingPlaytime = 0f;
        internal static float timeUntilReset = 0f;

        private static bool currentlyPlaying = false;

        public override void UpdateUI(GameTime gameTime)
        {
            // The next bullet hit will start the song from the beginning after the reset timer expires.
            SoundInstanceUpdateHelper.Update(ref gungeonSoundSlot, ref remainingPlaytime, ref timeUntilReset, ref currentlyPlaying, DefaultMusicTime, 0.8f);
        }

        // EEEEEEEEEEEEEEEEEEEEEEENTER THE GUNGEON, ENTER THE GUNGEON
        public static void GUN()
        {
            if (!currentlyPlaying)
                gungeonSoundSlot = SoundEngine.PlaySound(GungeonTrack);

            currentlyPlaying = true;
            remainingPlaytime = DefaultMusicTime + 30;
            timeUntilReset = DefaultResetTime;
        }
    }
}
