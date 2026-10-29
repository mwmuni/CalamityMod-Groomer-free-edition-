using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
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
    [LegacyName("CelestialTracers", "TracersSeraph")]
    public class SeraphTracers : BaseWings
    {
        protected override bool IsToggleable => true;

        public override float BonusAscentWhileFalling => 0.95f;
        public override float BonusAscentWhileRising => 0.16f;
        public override float RisingSpeedThreshold => 1.2f;
        public override float MaxAscentSpeed => 2.9f;
        public override float BaseAscent => 0.145f;

        public static int wingSlot = 0;

        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(250, 11f, 2.8f);
            wingSlot = Item.wingSlot;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 36;
            Item.height = 32;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
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
            Item.SetNameOverride(CalamityUtils.GetTextValue($"Items.Accessories.Wings.SeraphTracers.{(toggleEnabled ? "DisplayName" : "TreadsName")}"));
            CalamityUtils.DrawInventoryDot(spriteBatch, position, new Vector2(16, 16) * Main.inventoryScale, toggleEnabled);
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
                int flightDust = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)dustXOffset, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, DustID.GemDiamond, 0f, 0f, 100, default, 2.4f);
                Main.dust[flightDust].noGravity = true;
                Main.dust[flightDust].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[flightDust].fadeIn = 2f;
                }
                Main.dust[flightDust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            CalamityPlayer modPlayer = player.Calamity();
            player.accRunSpeed = 9f;
            player.moveSpeed += 0.18f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaImmune = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.noFallDmg = true;
            if (!toggleEnabled) //Both these effects just boost flight time, but we don't want Tracers to boost it's own flight time when functioning as wings. All other Angel Tread effects are covered above.
            {
                player.rocketBoots = player.vanityRocketBoots = 2;
                modPlayer.angelTreads = true;
            }
            modPlayer.tracersDust = !hideVisual && toggleEnabled;
            modPlayer.seraphTracers = true; // Grants immunity to Auric Rejection and other walk-on-block effects
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VoidStriders>().
                AddIngredient<AuricBar>(5).
                AddIngredient<AscendantSpiritEssence>(3).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
