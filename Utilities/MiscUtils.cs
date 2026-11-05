using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        /// <summary>
        /// Broadcast a LocalizedText. This only should be run on Singleplayer or Server.
        /// Multiplayer Clients Do NOT ask Server to Broadcast nor the print message locally.
        /// </summary>
        /// <param name="key">LocalizedText key</param>
        /// <param name="textColor">Text Color to use</param>
        public static void BroadcastLocalizedText(string key, Color? textColor = null)
        {
            // An attempt to bypass the need for a separate method and runtime/compile-time parameter
            // constraints by using nulls for defaults.
            if (!textColor.HasValue)
                textColor = Color.White;

            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(key), textColor.Value);
            else if (Main.dedServ)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key), textColor.Value);
        }

        /// <summary>
        /// Broadcast a LocalizedText with formatting. This only should be run on Singleplayer or Server.
        /// Multiplayer Clients Do NOT ask Server to Broadcast nor the print message locally.
        /// </summary>
        /// <param name="key">LocalizedText key</param>
        /// <param name="textColor">Text Color to use</param>
        public static void BroadcastFormattedText(string key, Color textColor, params object[] args)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetOrRegister(key).Format(args), textColor);
            else if (Main.dedServ)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key, args), textColor);
        }

        public static int IngredientIndex(this Recipe r, int itemID)
        {
            for (int i = 0; i < r.requiredItem.Count; ++i)
                if (r.requiredItem[i].type == itemID)
                    return i;
            return -1;
        }

        public static bool ChangeIngredientStack(this Recipe r, int itemID, int stack)
        {
            int idx = r.IngredientIndex(itemID);
            if (idx == -1)
                return false;
            r.requiredItem[idx].stack = stack;
            return true;
        }

        // Yes, this method has a use that Utils.Swap does not; You cannot use refs on array indices.
        // The CLR will not allow that. As such, a custom method must be used to achieve this.

        /// <summary>
        /// Swaps two array indices based on a temporary variable.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="index1">The first index to swap.</param>
        /// <param name="index2">The second index to swap.</param>
        public static void SwapArrayIndices<T>(ref T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        public static T[] ShuffleArray<T>(T[] array, Random rand = null)
        {
            if (rand is null)
                rand = new Random();

            for (int i = array.Length; i > 0; --i)
            {
                int j = rand.Next(i);
                T tempElement = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tempElement;
            }
            return array;
        }

        public static T[,] ShaveOffEdge<T>(this T[,] array)
        {
            if (array.GetLength(0) <= 2 || array.GetLength(1) <= 2)
                return array;

            T[,] result = new T[array.GetLength(0) - 2, array.GetLength(1) - 2];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = array[i + 1, j + 1];
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves all the colors from a <see cref="Texture2D"/> and returns them as a 2D <see cref="Color"/> array.
        /// </summary>
        /// <param name="texture">The texture to load.</param>
        /// <returns></returns>
        public static Color[,] GetColorsFromTexture(this Texture2D texture)
        {
            Color[] alignedColors = new Color[texture.Width * texture.Height];
            texture.GetData(alignedColors); // Fills the color array with all the colors in the texture

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    colors2D[x, y] = alignedColors[x + y * texture.Width];
                }
            }
            return colors2D;
        }

        /// <summary>
        /// Determines if a list contains an entry of a specific type. Specifically intended to account for derived types.
        /// </summary>
        /// <typeparam name="T">The base type of the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="type">The type to search for.</param>
        public static bool ContainsType<T>(this IEnumerable<T> collection, Type type) => collection.Any(entry => entry.GetType() == type);

        /// <summary>
        /// Calculates the sound volume and panning for a sound which is played at the specified location in the game world.<br/>
        /// Note that sound does not play on dedicated servers or during world generation.
        /// </summary>
        /// <param name="soundPos">The position the sound is emitting from. If either X or Y is -1, the sound does not fade with distance.</param>
        /// <param name="ambient">Whether the sound is considered ambient, which makes it use the ambient sound slider in the options. Defaults to false.</param>
        /// <returns>Volume and pan, in that order. Volume is always between 0 and 1. Pan is always between -1 and 1.</returns>
        public static (float, float) CalculateSoundStats(Vector2 soundPos, bool ambient = false)
        {
            float volume = 0f;
            float pan = 0f;

            if (soundPos.X == -1f || soundPos.Y == -1f)
                volume = 1f;
            else if (WorldGen.gen || Main.dedServ)
                volume = 0f;
            else
            {
                float topLeftX = Main.screenPosition.X - Main.screenWidth * 2f;
                float topLeftY = Main.screenPosition.Y - Main.screenHeight * 2f;

                // Sounds cannot be heard from more than ~2.5 screens away.
                // This rectangle is 5x5 screens centered on the current screen center position.
                Rectangle audibleArea = new Rectangle((int)topLeftX, (int)topLeftY, Main.screenWidth * 5, Main.screenHeight * 5);
                Rectangle soundHitbox = new Rectangle((int)soundPos.X, (int)soundPos.Y, 1, 1);
                Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f);
                if (audibleArea.Intersects(soundHitbox))
                {
                    pan = (soundPos.X - screenCenter.X) / (Main.screenWidth * 0.5f);
                    float dist = Vector2.Distance(soundPos, screenCenter);
                    volume = 1f - (dist / (Main.screenWidth * 1.5f));
                }
            }

            pan = MathHelper.Clamp(pan, -1f, 1f);
            volume = MathHelper.Clamp(volume, 0f, 1f);
            if (ambient)
                volume = Main.gameInactive ? 0f : volume * Main.ambientVolume;
            else
                volume *= Main.soundVolume;

            // This is actually done by vanilla. I guess if the sound volume gets corrupted during gameplay, you can't blast your eardrums out.
            volume = MathHelper.Clamp(volume, 0f, 1f);
            return (volume, pan);
        }

        /// <summary>
        /// Convenience function to utilize CalculateSoundStats immediately on an existing sound effect.<br/>
        /// This allows updating a looping sound every single frame to have the correct volume and pan, even if the player drags the audio sliders around.
        /// </summary>
        /// <param name="sfx">The SoundEffectInstance which is having its values updated.</param>
        /// <param name="soundPos">The position the sound is emitting from. If either X or Y is -1, the sound does not fade with distance.</param>
        /// <param name="ambient">Whether the sound is considered ambient, which makes it use the ambient sound slider in the options. Defaults to false.</param>
        public static void ApplySoundStats(ref SoundEffectInstance sfx, Vector2 soundPos, bool ambient = false)
        {
            if (sfx is null || sfx.IsDisposed)
                return;
            (sfx.Volume, sfx.Pan) = CalculateSoundStats(soundPos, ambient);
        }

        /// <summary>
        /// Method to change the volume of a sound without having to manually do a nullcheck or having to clamp it down to between 0 and 1
        /// </summary>
        /// <param name="sfx">The SoundEffectInstance which is having its values updated.</param>
        /// <param name="volumeMultiplier">How much the sound's volume should get changed</param>
        public static void SafeVolumeChange(ref SoundEffectInstance sfx, float volumeMultiplier)
        {
            if (sfx is null || sfx.IsDisposed)
                return;
            sfx.Volume = MathHelper.Clamp(sfx.Volume * volumeMultiplier, 0f, 1f);
        }

        public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
        {
            if (condition)
                list.Add(type);
        }

        public static int ScaleWithDifficulty(this int value) => value * (Main.masterMode ? 3 : Main.expertMode ? 2 : 1);

        public static int SecondsToFrames(int seconds) => seconds * 60;
        public static int SecondsToFrames(float seconds) => (int)MathF.Round(seconds * 60);
        public static int MinutesToFrames(int minutes) => minutes * 3600;

        public static bool WithinBounds(this int index, int cap) => index >= 0 && index < cap;

        /// <summary>
        /// Clamps the distance between vectors via normalization.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="maxDistance">The maximum possible distance between the two vectors before they get clamped.</param>
        public static void DistanceClamp(ref Vector2 start, ref Vector2 end, float maxDistance)
        {
            if (Vector2.Distance(end, start) > maxDistance)
            {
                end = start + Vector2.Normalize(end - start) * maxDistance;
            }
        }

        public static void ChangeTime(bool changeToDay)
        {
            Main.time = 0D;
            Main.dayTime = changeToDay;
            CalamityNetcode.SyncWorld();
        }

        public static bool IntoMorseCode(string originalText, float completion)
        {
            int spaceLength = 13;
            int betweenLetterLength = 7;
            int betweenBlipLength = 4;
            int shortLength = 3;
            int longLength = 8;

            char[] TextKeys = { ' ', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
                's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            string[] MorseKeys = { " ", ".-|", "-...|", "-.-. |", "-..|", ".|", "..-.|"
                    , "--.|", "....|", "..|", ".---|","-.-|",".-..|","--|",
                      "-.|","---|",".--.|","--.-|",".-.|","...|","-|","..-|",
                      "...-|",".--|","-..-|","-.--|","--..|",".----|",
                      "..---|","...--|","....-|",".....|","-....|","--...|",
                      "---..|","----.|","-----|" };

            string morseText = "";
            originalText = originalText.ToLower();

            //Construct a string of text that replaces all the stuff with morse.
            for (int i = 0; i < originalText.Length; i++)
            {
                for (int j = 0; j < 37; j++)
            {
                    if (TextKeys[j] == originalText[i])
                    {
                        morseText += MorseKeys[j];
                        break;
                }
                }
            }

            List<bool> morseState = new List<bool>();

            for (int i = 0; i < morseText.Length; i++)
            {
                if (morseText[i] == " ".ToCharArray()[0])
                    morseState.AddRange(Enumerable.Repeat(false, spaceLength));

                if (morseText[i] == "|".ToCharArray()[0])
                    morseState.AddRange(Enumerable.Repeat(false, betweenLetterLength));

                if (morseText[i] == ".".ToCharArray()[0])
                    morseState.AddRange(Enumerable.Repeat(true, shortLength));

                if (morseText[i] == "-".ToCharArray()[0])
                    morseState.AddRange(Enumerable.Repeat(true, longLength));

                morseState.AddRange(Enumerable.Repeat(false, betweenBlipLength));
            }

            return morseState[(int)((morseState.Count - 1) * completion)];
        }

        public static List<string> GetAssignedKeysOrEmpty(this ModKeybind keybind, InputMode mode = InputMode.Keyboard)
        {
            if (keybind == null)
                return [];

            if (Main.dedServ) // Server does not have key assigned
                return [];

            try
            {
                if (mode == InputMode.Mouse)
                    mode = InputMode.Keyboard;
                return keybind.GetAssignedKeys(mode);
            }
            catch
            {
                return [];
            }
        }

        /// <summary>
        /// Returns whether <paramref name="keybind"/> has any keys assigned. Unlike <see cref="GetAssignedKeysOrEmpty"/>, this does not allocate on servers or for null keybinds.
        /// </summary>
        public static bool HasAssignedKeys(this ModKeybind keybind, InputMode mode = InputMode.Keyboard)
        {
            if (keybind == null || Main.dedServ)
                return false;

            return keybind.GetAssignedKeysOrEmpty(mode).Count > 0;
        }
    }
}
