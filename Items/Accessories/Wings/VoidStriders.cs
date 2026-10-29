using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    [LegacyName("ElysianTracers", "TracersElysian")]
    public class VoidStriders : BaseWings
    {
        protected override bool IsToggleable => true;

        public override float BonusAscentWhileFalling => 0.85f;
        public override float BonusAscentWhileRising => 0.15f;
        public override float RisingSpeedThreshold => 1.1f;
        public override float MaxAscentSpeed => 2.7f;
        public override float BaseAscent => 0.135f;

        public static int wingSlot = 0;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(200, 10f, 2.7f);
            wingSlot = Item.wingSlot;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 36;
            Item.height = 32;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }
        #region Toggleable Wings
        protected override bool toggleEnabled
        {
            get { return Item.wingSlot != -1; }
            set
            {
                if (value)
                    Item.wingSlot = wingSlot;
                else
                    Item.wingSlot = -1;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!toggleEnabled)
                tooltips.RemoveAll(x => x.Name == "Tooltip0");
            base.ModifyTooltips(tooltips);
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Item.SetNameOverride(CalamityUtils.GetTextValue($"Items.Accessories.Wings.VoidStriders.{(toggleEnabled ? "DisplayName" : "TreadsName")}"));
            CalamityUtils.DrawInventoryDot(spriteBatch, position, new Vector2(16, 16) * Main.inventoryScale, toggleEnabled);
        }
        public override void UpdateInventory(Player player)
        {
        }
        #endregion
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.controlJump && player.wingTime > 0f && player.jump == 0 && player.velocity.Y != 0f && !hideVisual && toggleEnabled)
            {
                int dustXOffset = 4;
                if (player.direction == 1)
                {
                    dustXOffset = -40;
                }
                int flightDust = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)dustXOffset, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, Main.rand.NextBool() ? 206 : 173, 0f, 0f, 100, default, 2.4f);
                Main.dust[flightDust].noGravity = true;
                Main.dust[flightDust].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[flightDust].fadeIn = 2f;
                }
                Main.dust[flightDust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            CalamityPlayer modPlayer = player.Calamity();
            player.accRunSpeed = 8.5f;
            player.moveSpeed += 0.16f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaImmune = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.noFallDmg = true;
            player.socialShadowRocketBoots = true;
            if (!toggleEnabled) //Both these effects just boost flight time, but we don't want Tracers to boost it's own flight time when functioning as wings. All other Angel Tread effects are covered above.
            {
                player.rocketBoots = player.vanityRocketBoots = 1;
                modPlayer.angelTreads = true;
            }
            modPlayer.tracersDust = !hideVisual && toggleEnabled;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MoonWalkers>().
                AddIngredient<ArmoredShell>(3).
                AddIngredient<TwistingNether>(3).
                AddIngredient<DarkPlasma>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
