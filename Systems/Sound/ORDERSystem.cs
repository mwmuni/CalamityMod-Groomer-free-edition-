using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class ORDERSystem : ModSystem
    {
        private static readonly SoundStyle ORDERTrack = new("CalamityMod/Sounds/Custom/ORDER", SoundType.Music);
        private static SlotId orderSoundSlot;

        internal static float DefaultOrderTime = 100f;
        internal static float DefaultResetTime = 240f;

        internal static float remainingPlaytime = 0f;
        internal static float timeUntilReset = 0f;

        private static bool currentlyPlaying = false;

        public override void UpdateUI(GameTime gameTime)
        {
            // The next Ricoshot will start the song from the beginning after the reset timer expires.
            SoundInstanceUpdateHelper.Update(ref orderSoundSlot, ref remainingPlaytime, ref timeUntilReset, ref currentlyPlaying, DefaultOrderTime, 1f);
        }

        // DIE!
        public static void JUDGEMENT()
        {
            if (!currentlyPlaying)
                orderSoundSlot = SoundEngine.PlaySound(ORDERTrack);

            currentlyPlaying = true;
            remainingPlaytime = DefaultOrderTime;
            timeUntilReset = DefaultResetTime;
        }
    }
}
