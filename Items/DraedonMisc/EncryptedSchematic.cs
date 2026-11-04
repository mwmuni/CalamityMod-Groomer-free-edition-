using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    /// <summary>
    /// Shared behavior for Draedon's encrypted schematics: world discovery/unlock flags, the crafting recipe,
    /// unlocked-recipe tooltips and the schematic GUI.
    /// </summary>
    public abstract class EncryptedSchematic : ModItem, ILocalizedModType
    {
        protected readonly record struct SchematicDisplay(int ItemType, string TooltipName, Color Color);

        protected static readonly Color MachineColor = new(165, 118, 104);

        public new string LocalizationCategory => "Items.DraedonItems";

        /// <summary>
        /// Schematic name understood by <see cref="SchematicRecipe.ConstructRecipeCondition"/>.
        /// </summary>
        protected abstract string SchematicName { get; }

        protected abstract ref bool HasFoundSchematic { get; }

        protected abstract ref bool HasUnlockedRecipes { get; }

        protected abstract Type SchematicGUIType { get; }

        /// <summary>
        /// Unlocked items, listed in display order directly after <see cref="TooltipInsertionMarker"/>.
        /// </summary>
        protected abstract SchematicDisplay[] Displays { get; }

        protected virtual string TooltipInsertionMarker => "Tooltip0";

        protected virtual bool ChangesTooltipWhenUnlocked => true;

        /// <summary>
        /// Whether holding the schematic unlocks its recipes directly instead of requiring decryption in a Codebreaker.
        /// </summary>
        protected virtual bool UnlocksRecipesWhenFound => false;

        protected static SchematicDisplay Display<T>(string tooltipName, Color color) where T : ModItem =>
            new(ModContent.ItemType<T>(), $"CalamityMod:{tooltipName}", color);

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.maxStack = 1;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || (HasFoundSchematic && (HasUnlockedRecipes || !UnlocksRecipesWhenFound)))
                return;

            HasFoundSchematic = true;
            if (UnlocksRecipesWhenFound)
                HasUnlockedRecipes = true;

            CalamityNetcode.SyncWorld();
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (!HasUnlockedRecipes)
                return;

            if (ChangesTooltipWhenUnlocked)
            {
                TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
                if (line != null)
                    line.Text = CalamityUtils.GetTextValue($"{LocalizationCategory}.SchematicUnlocked");
            }

            int insertIndex = list.FindIndex(x => x.Name == TooltipInsertionMarker && x.Mod == "Terraria");
            if (insertIndex == -1)
                return;

            foreach (SchematicDisplay display in Displays)
            {
                TooltipLine displayLine = new TooltipLine(Mod, display.TooltipName, $"[i:{display.ItemType}] {CalamityUtils.GetItemName(display.ItemType)}");
                displayLine.OverrideColor = display.Color;
                list.Insert(++insertIndex, displayLine);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(10).
                AddIngredient(ItemID.Glass, 50).
                AddCondition(SchematicRecipe.ConstructRecipeCondition(SchematicName, out Func<bool> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && HasUnlockedRecipes)
                PopupGUIManager.FlipActivityOfGUIWithType(SchematicGUIType);

            return true;
        }
    }
}
