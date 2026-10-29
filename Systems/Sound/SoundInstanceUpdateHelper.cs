using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.Audio;

namespace CalamityMod.Systems
{
    internal static class SoundInstanceUpdateHelper
    {
        internal static void Update(ref SlotId soundSlot, ref float remainingPlaytime, ref float timeUntilReset, ref bool currentlyPlaying, float maxPlaytime, float maxVolume)
        {
            // Decrement timers
            if (remainingPlaytime > 0)
                --remainingPlaytime;
            if (timeUntilReset > 0)
                --timeUntilReset;

            if (currentlyPlaying)
            {
                // If the reset timer has run out, stop the active sound instance entirely.
                if (timeUntilReset <= 0f)
                {
                    currentlyPlaying = false;
                    if (SoundEngine.TryGetActiveSound(soundSlot, out var activeSound))
                        activeSound.Stop();
                    soundSlot = SlotId.Invalid;
                }

                // Otherwise, set the volume of the active sound instance appropriately.
                else
                {
                    bool foundSound = SoundEngine.TryGetActiveSound(soundSlot, out var activeSound);
                    if (!foundSound)
                    {
                        currentlyPlaying = false;
                        return;
                    }

                    float newVolume = MathHelper.Clamp(remainingPlaytime / maxPlaytime, 0f, maxVolume);
                    activeSound.Volume = newVolume;
                    activeSound.Update();
                }
            }
        }
    }
}
