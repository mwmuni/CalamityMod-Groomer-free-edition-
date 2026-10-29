using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Magic;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SepticSkewer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public bool pulled = false;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 24;
            Item.damage = 933;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shootSpeed = 18f;
            Item.shoot = ModContent.ProjectileType<SepticSkewerHarpoon>();

            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            pulled = false;
            if (player.altFunctionUse == 2)
            {
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active && projectile.type == type)
                    {
                        projectile.ai[2] = 5;
                        pulled = true;
                    }
                }

                SoundStyle fire = new("CalamityMod/Sounds/Item/GunShotMid");
                SoundEngine.PlaySound(fire with { Volume = 0.7f, Pitch = Main.rand.NextFloat(0.7f, 0.8f) }, position);
                if (pulled)
                    player.SetScreenshake(5.5f);
            }
            else
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/GunShotHeavy");
                SoundEngine.PlaySound(fire with { Volume = 0.45f, Pitch = Main.rand.NextFloat(0.4f, 0.65f) }, position);
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 0, 0);
            }
            return false;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            float pullback = 7f;

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                pullback -= (2.75f) * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2);

            CalamityUtils.CleanHoldStyle(player, itemRotation, new Vector2(46, 24), new Vector2(-24, 4), pullback);

            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += (player.altFunctionUse == 2 ? -0.25f : 0) * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
    }
}
