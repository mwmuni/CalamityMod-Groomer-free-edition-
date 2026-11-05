using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace CalamityMod
{
    public class CalamityKeybinds : ModSystem
    {
        public static ModKeybind AdrenalineHotKey { get; private set; }
        public static ModKeybind ArmorSetBonusHotKey { get; private set; }
        public static ModKeybind DashHotkey { get; private set; }
        public static ModKeybind ExoChairSlowdownHotkey { get; private set; }
        public static ModKeybind GodSlayerDashHotKey { get; private set; }
        public static ModKeybind NormalityRelocatorHotKey { get; private set; }
        public static ModKeybind RageHotKey { get; private set; }
        public static ModKeybind SwitchGravityHotkey { get; private set; }
        public static ModKeybind ExpandDebuffInfo { get; private set; }


        public static ModKeybind HeldItem { get; private set; }
        public static ModKeybind Accessory1 { get; private set; }
        public static ModKeybind Accessory2 { get; private set; }
        public static ModKeybind Accessory3 { get; private set; }
        public static ModKeybind Accessory4 { get; private set; }
        public static ModKeybind Accessory5 { get; private set; }
        public static ModKeybind Accessory6 { get; private set; }
        public static ModKeybind Accessory7 { get; private set; }

        public static ModKeybind[] AccessoryKeybinds = [];

        public static List<int> RentedKeybinds = [];
        /// <summary>
        /// We store the keybind then move it to the main list at the end of the frame
        /// This ensures that between frames has keybinds in the values needed for drawing
        /// This does means there's a 1 frame delay when switching slots for item tooltips to update, but that's never important
        /// </summary>
        public static List<int> KeybindRentalQueue = [];

        public override void Load()
        {
            // Register keybinds            
            AdrenalineHotKey = KeybindLoader.RegisterKeybind(Mod, "AdrenalineMode", "B");
            ArmorSetBonusHotKey = KeybindLoader.RegisterKeybind(Mod, "ArmorSetBonus", "Y");
            DashHotkey = KeybindLoader.RegisterKeybind(Mod, "DashDoubleTapOverride", "F");
            ExoChairSlowdownHotkey = KeybindLoader.RegisterKeybind(Mod, "ExoChairSlowDown", "RightShift");
            GodSlayerDashHotKey = KeybindLoader.RegisterKeybind(Mod, "GodSlayerDash", "H");
            NormalityRelocatorHotKey = KeybindLoader.RegisterKeybind(Mod, "NormalityRelocator", "Z");
            RageHotKey = KeybindLoader.RegisterKeybind(Mod, "RageMode", "V");
            SwitchGravityHotkey = KeybindLoader.RegisterKeybind(Mod, "GravitySwapOverride", "T");
            ExpandDebuffInfo = KeybindLoader.RegisterKeybind(Mod, "ExpandDebuffInfo", "LeftControl");

            HeldItem = KeybindLoader.RegisterKeybind(Mod, "HeldItem", "NumPad0");
            Accessory1 = KeybindLoader.RegisterKeybind(Mod, "Accessory1", "NumPad1");
            Accessory2 = KeybindLoader.RegisterKeybind(Mod, "Accessory2", "NumPad2");
            Accessory3 = KeybindLoader.RegisterKeybind(Mod, "Accessory3", "NumPad3");
            Accessory4 = KeybindLoader.RegisterKeybind(Mod, "Accessory4", "NumPad4");
            Accessory5 = KeybindLoader.RegisterKeybind(Mod, "Accessory5", "NumPad5");
            Accessory6 = KeybindLoader.RegisterKeybind(Mod, "Accessory6", "NumPad6");
            Accessory7 = KeybindLoader.RegisterKeybind(Mod, "Accessory7", "NumPad7");

            AccessoryKeybinds = [
                Accessory1,
                Accessory2,
                Accessory3,
                Accessory4,
                Accessory5,
                Accessory6,
                Accessory7
           ];
        }

        public override void Unload()
        {
            AdrenalineHotKey = null;
            ArmorSetBonusHotKey = null;
            DashHotkey = null;
            ExoChairSlowdownHotkey = null;
            GodSlayerDashHotKey = null;
            NormalityRelocatorHotKey = null;
            RageHotKey = null;
            ExpandDebuffInfo = null;

            Accessory1 = null;
            Accessory2 = null;
            Accessory3 = null;
            Accessory4 = null;
            Accessory5 = null;
            Accessory6 = null;
            Accessory1 = null;
            Accessory7 = null;

            AccessoryKeybinds = null;
        }
        public override void PostUpdateEverything()
        {
            // Reuse the rented list rather than allocating a copy every tick.
            RentedKeybinds.Clear();
            RentedKeybinds.AddRange(KeybindRentalQueue);
            KeybindRentalQueue.Clear();
        }
    }

    public class AccessoryKeybindManager : GlobalItem
    {
        public override bool InstancePerEntity => true;

        internal int KeybindId { get; set; }

        public override void UpdateEquip(Item item, Player player)
        {
            if (CalamityItemSets.HasAccessoryKeybind[item.type])
                CalamityKeybinds.KeybindRentalQueue.Add(item.type);
        }
    }

    public static class CalamityKeybindsExtensions
    {
        /// <summary>
        /// Gets if the Calamity keybind for the given item was just pressed. <br/>
        /// Automatically adjusts between Held Item and Accessory Ability keybinds based on if the item is held and equip order of accessories
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool JustPressedKeybind(this Item item)
        {
            if (item == Main.LocalPlayer.HeldItem)
                return CalamityKeybinds.HeldItem.JustPressed;

            if (!CalamityItemSets.HasAccessoryKeybind[item.type])
                Debug.Assert(false, "Item is not marked as having a Calamity keybind in the HasAccessoryKeybind item set, but is requesting an accessory keybind");

            if (CalamityKeybinds.RentedKeybinds.Contains(item.type))
            {
                int index = CalamityKeybinds.RentedKeybinds.IndexOf(item.type);
                if (CalamityKeybinds.AccessoryKeybinds.IndexInRange(index))
                    return CalamityKeybinds.AccessoryKeybinds[index].JustPressed;
            }

            return false;
        }

        /// <summary>
        /// Gets if the Calamity keybind for the given item was just released. <br/>
        /// Automatically adjusts between Held Item and Accessory Ability keybinds based on if the item is held and equip order of accessories
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool JustReleasedKeybind(this Item item)
        {

            if (item == Main.LocalPlayer.HeldItem)
                return CalamityKeybinds.HeldItem.JustReleased;

            if (!CalamityItemSets.HasAccessoryKeybind[item.type])
                Debug.Assert(false, "Item is not marked as having a Calamity keybind in the HasAccessoryKeybind item set, but is requesting an accessory keybind");

            if (CalamityKeybinds.RentedKeybinds.Contains(item.type))
            {
                int index = CalamityKeybinds.RentedKeybinds.IndexOf(item.type);
                if (CalamityKeybinds.AccessoryKeybinds.IndexInRange(index))
                    return CalamityKeybinds.AccessoryKeybinds[index].JustReleased;
            }

            return false;
        }


        /// <summary>
        /// Gets if the Calamity keybind for the given item is currently held down. <br/>
        /// Automatically adjusts between Held Item and Accessory Ability keybinds based on if the item is held and equip order of accessories
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool CurrentlyPressingKeybind(this Item item)
        {
            if (item == Main.LocalPlayer.HeldItem)
                return CalamityKeybinds.HeldItem.Current;

            if (!CalamityItemSets.HasAccessoryKeybind[item.type])
                Debug.Assert(false, "Item is not marked as having a Calamity keybind in the HasAccessoryKeybind item set, but is requesting an accessory keybind");

            if (CalamityKeybinds.RentedKeybinds.Contains(item.type))
            {
                int index = CalamityKeybinds.RentedKeybinds.IndexOf(item.type);
                if (CalamityKeybinds.AccessoryKeybinds.IndexInRange(index))
                    return CalamityKeybinds.AccessoryKeybinds[index].Current;
            }

            return false;
        }


        /// <summary>
        /// Gets the Calamity keybind for the given item. <br/>
        /// Automatically adjusts between Held Item and Accessory Ability keybinds based on if the item is held and equip order of accessories
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ModKeybind? GetDynamicModHotkey(this Item item)
        {

            if (!CalamityItemSets.HasAccessoryKeybind[item.type])
                return CalamityKeybinds.HeldItem;

            if (CalamityKeybinds.RentedKeybinds.Contains(item.type))
            {
                int index = CalamityKeybinds.RentedKeybinds.IndexOf(item.type);
                if (CalamityKeybinds.AccessoryKeybinds.IndexInRange(index))
                    return CalamityKeybinds.AccessoryKeybinds[index];
            }
            return null;
        }
       
        public static bool HasBoundDynamicHotkey(this Item item) => item.GetDynamicModHotkey().HasAssignedKeys(PlayerInput.CurrentInputMode);
    }
}
