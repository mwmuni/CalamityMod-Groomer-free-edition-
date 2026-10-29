using System;
using System.Collections.Generic;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    [LegacyName("Taser")]
    public class ShortCircuit : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.DraedonsArsenal";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(10);
        public static readonly SoundStyle Fire = new("CalamityMod/Sounds/Item/TaserLaunch") { Volume = 0.6f };
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
        }
        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 42;
            Item.height = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 11;
            Item.knockBack = 7f;
            Item.useTime = Item.useAnimation = 8;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;

            Item.shoot = ModContent.ProjectileType<ShortCircuitShot>();
            Item.shootSpeed = 6f;
        }
        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 0.3f;

            return 1f;
        }
        public override bool CanUseItem(Player player) => (player.altFunctionUse == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<ShortCircuitHook>()] <= 0) || player.altFunctionUse == 0;
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && player.whoAmI == Main.myPlayer && !Main.mapFullscreen && !Main.blockMouse && player.Calamity().arsenalCooldown <= 0)
            {
                SoundEngine.PlaySound(Fire with { Pitch = -0.1f }, position);
                Projectile hook = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<ShortCircuitHook>(), (int)(damage * 1.2f), 0, player.whoAmI);
                return false;
            }

            if (player.altFunctionUse == 0)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/GunShotSmall");
                SoundEngine.PlaySound(fire with { Pitch = Main.rand.NextFloat(0.5f, 0.65f), Volume = 0.2f, MaxInstances = -1 }, position);
                Projectile shot = Projectile.NewProjectileDirect(source, position + velocity - Vector2.UnitY * 5, (velocity * 1.5f).RotatedByRandom(0.15f), type, damage, 0, player.whoAmI);
                return false;
            }

            return false;
        }
        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.Calamity().rightClickListener = true;
            player.Calamity().mouseWorldListener = true;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.altFunctionUse == 0)
            {
                player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
                float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

                float pullback = 7f;

                float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
                float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
                if (animProgress < 0.4f)
                    pullback -= (2.75f) * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2);

                CalamityUtils.CleanHoldStyle(player, itemRotation, new Vector2(42, 24), new Vector2(-24, 4), pullback);

                base.UseStyle(player, heldItemFrame);
            }
        }

        public override void UseItemFrame(Player player)
        {
            if (player.altFunctionUse == 0)
            {
                player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

                float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
                float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
                if (animProgress < 0.4f)
                    rotation += (player.altFunctionUse == 2 ? -0.15f : 0) * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2) * player.direction;

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
            }
        }
        public override Vector2? HoldoutOffset() => new Vector2(0f, 0f);
        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(7).
                AddIngredient<DubiousPlating>(5).
                AddIngredient<AerialiteBar>(4).
                AddIngredient<SeaPrism>(7).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(1, out Func<bool> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
