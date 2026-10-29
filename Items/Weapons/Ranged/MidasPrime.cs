using System;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MidasPrime : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        // The Hold SHIFT tooltip is an easter egg and is not indicated to the player in any way.
        public bool ShowExtensionIndicator => false;
        
        // Easter egg has a special tooltip key and color.
        public string TooltipExtensionKey => "UltrakillEasterEgg";
        public Color? TooltipExtensionColor => Color.Red;

        internal static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Item/CrackshotColtShot") { Volume = 0.5f, PitchVariance = 0.1f };

        // Internal storage used to keep track between UseItem and Shoot hooks whether a gold coin was queued up
        private bool nextShotGoldCoin = false;

        public override void SetDefaults()
        {
            Item.width = 23;
            Item.height = 8;
            Item.damage = 81;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = ShootSound;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MarksmanShot>();
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 14f;
        }

        // This item has a right click.
        public override bool AltFunctionUse(Player player) => true;

        // This item enables the automatic syncing of player mouse coordinates while held.
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        // This item never uses ammo when right clicking.
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse != 2;

        public override bool CanUseItem(Player player)
        {
            // Two things are checked for right click:
            // 1) The player has at least 1 silver coin to toss
            // 2) The player doesn't have 4 ricoshot coins (of any type) in the air already
            if (player.altFunctionUse == 2)
            {
                // player.CanBuyItem() breaks if the player has more than 2,147 platinum coins and was never fixed
                // 20APR2024: Ozzatron: The method was instead replaced with player.CanAfford, which is immune to overflow
                // Additionally, CanAfford checks your Piggy Bank, Safe, etc., meaning Midas Prime can use coins from there
                bool hasAtLeastOneSilver = player.CanAfford(100);
                return hasAtLeastOneSilver && player.GetActiveRicoshotCoinCount() < 4;
            }
            return true;
        }

        public override bool? UseItem(Player player)
        {
            // Remove either 1 gold (if possible) or 1 silver (otherwise) when using right click
            if (player.altFunctionUse == 2)
            {
                // 20APR2024: Ozzatron: Use CanAfford (new function) to sum money across all the player's banks
                // If they have a sum total of at least 1 gold everywhere, they can toss a gold coin
                bool hasAtLeastOneGold = player.CanAfford(10000);

                // If the player has at least 1 gold in their inventory, spend it and use a gold coin
                if (hasAtLeastOneGold)
                {
                    player.BuyItem(10000);
                    nextShotGoldCoin = true;
                }

                // Otherwise, spend 1 silver and use a silver coin
                else
                {
                    player.BuyItem(100);
                    nextShotGoldCoin = false;
                }
            }

            return base.UseItem(player);
        }

        // This hook is a convenient location to change the use sound.
        public override void UseAnimation(Player player)
        {
            Item.UseSound = ShootSound;
            if (player.altFunctionUse == 2)
                Item.UseSound = RicoshotCoin.BlingSound;
        }

        // Coins can be tossed much faster than bullets can be fired.
        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 3f;
            return 1f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Move all fired projectiles 15 pixels upwards so they don't come out of the player's groin
            position -= Vector2.UnitY * 15f;

            // No matter what type of ammo is used, left click will fire a Marksman Round
            type = ModContent.ProjectileType<MarksmanShot>();

            // Right clicks toss coins instead
            if (player.altFunctionUse == 2)
            {
                damage = 0;
                type = ModContent.ProjectileType<RicoshotCoin>();
                velocity = player.GetCoinTossVelocity();
            }
        }

        // This shoot override is only needed to set ai[0] to be either 1f or 2f for silver and gold coins.
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                float coinAIVariable = nextShotGoldCoin ? 2f : 1f;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, coinAIVariable);
                return false;
            }

            // Otherwise use default behavior (which is just to return true).
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        // Make the gun have visible recoil when fired for extra cool factor.
        #region Firing Animation
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            CalamityUtils.CleanHoldStyle(player, new Vector2(50, 24), new Vector2(-17, 3), 7f);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += -0.45f * (float)Math.Pow((0.4f - animProgress) / 0.4f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
        #endregion
    }
}
