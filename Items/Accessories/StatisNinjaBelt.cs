using System.Collections.Generic;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.BaseItems;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StatisNinjaBelt : BaseToggleableItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }
        #region Toggleable Tiger Gear

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryDot(spriteBatch, position, new Vector2(16, 16) * Main.inventoryScale, toggleEnabled);
        }
        #endregion
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplace("[TOGGLE]", toggleEnabled ? this.GetLocalizedValue("ToggleEffect") : "");
            base.ModifyTooltips(tooltips);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.autoJump = true;
            player.jumpSpeedBoost += 1.6f;
            player.moveSpeed += 0.1f;
            player.noFallDmg = true;
            player.blackBelt = true;
            player.dashType = 0;
            player.Calamity().DashID = StatisNinjaBeltDash.ID;
            if (toggleEnabled)
                player.spikedBoots = 2;
            player.accFlipper = true;
            player.Calamity().statisNinjaBelt = true;

            player.MountedCenter.ToTileCoordinates();
        }

        public override void AddRecipes()
        {
            // 20FEB2024: Ozzatron: used to have one recipe which was MNG + Frog Gear. This requires 2 Tiger Climbing Gear.
            // There are now two recipes depending on whether you made Frog Gear or Master Ninja Gear.
            CreateRecipe().
                AddIngredient(ItemID.MasterNinjaGear).
                AddIngredient(ItemID.FrogFlipper).
                AddIngredient<PurifiedGel>(25).
                AddIngredient<Necroplasm>(5).
                AddTile(TileID.MythrilAnvil).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.Tabi).
                AddIngredient(ItemID.BlackBelt).
                AddIngredient(ItemID.FrogGear).
                AddIngredient<PurifiedGel>(25).
                AddIngredient<Necroplasm>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
