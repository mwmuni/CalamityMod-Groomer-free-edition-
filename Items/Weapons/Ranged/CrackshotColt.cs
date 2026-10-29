using System;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class CrackshotColt : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        // Crackshot Colt uses the same sound as Midas Prime, just quieter.
        private static SoundStyle ShootSound => MidasPrime.ShootSound with { Volume = 0.4f };

        public override void SetDefaults()
        {
            Item.width = 23;
            Item.height = 8;
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
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
            // 1) The player has at least 1 copper coin to toss
            // 2) The player doesn't have 4 ricoshot coins (of any type) in the air already
            if (player.altFunctionUse == 2)
            {
                // player.CanBuyItem() breaks if the player has more than 2,147 platinum coins and was never fixed
                // 20APR2024: Ozzatron: The method was instead replaced with player.CanAfford, which is immune to overflow
                // Additionally, CanAfford checks your Piggy Bank, Safe, etc., meaning Crackshot Colt can use coins from there
                bool hasAtLeastOneCopper = player.CanAfford(1);
                return hasAtLeastOneCopper && player.GetActiveRicoshotCoinCount() < 4;
            }
            return true;
        }

        public override bool? UseItem(Player player)
        {
            // Remove 1 copper from the player's inventory when using right click
            if (player.altFunctionUse == 2)
                player.BuyItem(1);

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
            // Move all fired projectiles 12 pixels upwards so they don't come out of the player's groin
            position -= Vector2.UnitY * 12f;

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

        // 
        // Crackshot Colt has no Shoot override because it can only toss copper coins.
        // Copper coins have ai[0] = 0f, so no override is needed.
        //

        // Make the gun have visible recoil when fired for extra cool factor.
        #region Firing Animation
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            CalamityUtils.CleanHoldStyle(player, new Vector2(40, 20), new Vector2(-15, 1), 7f);
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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldBar", 8).
                AddIngredient<StormlionMandible>().
                AddIngredient<BloodOrb>().
                AddIngredient(ItemID.CopperCoin, 4).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
