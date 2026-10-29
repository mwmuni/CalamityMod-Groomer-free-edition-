using System.Collections.Generic;
using System.IO;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("StatisBeltOfCurses")]
    public class StatisVoidSash : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static SoundStyle VoidDash = new ("CalamityMod/Sounds/Item/VoidDash");

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 3));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }
        #region Toggleable Tiger Gear

        bool toggleEnabled = true;

        public override bool CanRightClick() => Main.keyState.PressingShift();
        public override void RightClick(Player player)
        {
            toggleEnabled = !toggleEnabled;
            Item.NetStateChanged();
        }
        public override bool ConsumeItem(Player player) => false;
        public override void SaveData(TagCompound tag)
        {
            tag.Add("toggleEffect", toggleEnabled);
        }

        public override void LoadData(TagCompound tag)
        {
            toggleEnabled = tag.GetBool("toggleEffect");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(toggleEnabled);
        }

        public override void NetReceive(BinaryReader reader)
        {
            toggleEnabled = reader.ReadBoolean();
        }

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
            CalamityPlayer modPlayer = player.Calamity();

            modPlayer.voidSashVisuals = !hideVisual;

            player.autoJump = true;
            player.jumpSpeedBoost += 1.6f;
            player.moveSpeed += 0.2f;
            player.noFallDmg = true;
            player.blackBelt = true;
            modPlayer.DashID = StatisVoidSashDash.ID;
            player.dashType = 0;
            if (toggleEnabled)
                player.spikedBoots = 2;
            player.accFlipper = true;
            player.Calamity().statisVoidSash = true;

            player.MountedCenter.ToTileCoordinates();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StatisNinjaBelt>().
                AddIngredient<TwistingNether>(10).
                AddIngredient<NightmareFuel>(20).
                AddTile<CosmicAnvil>().
                Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            AccessoryInventoryDrawing.DrawCustomScale(Type, spriteBatch, position, frame, drawColor, itemColor, origin, scale, 1f);
            return false;
        }
    }
}
