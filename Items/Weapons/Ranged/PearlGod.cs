using CalamityMod.Particles;
using CalamityMod.Projectiles;
using System;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Rarities;
using Terraria.Audio;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PearlGod : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public bool swapType = false;
        public bool healVisual = false;

        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 46;
            Item.damage = 160;
            Item.scale = 0.75f;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/GunShotSmall") with { Volume = 0.65f };
            Item.autoReuse = true;
            Item.shootSpeed = 14f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;
            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;

            if (Main.zenithWorld)
            {
                Projectile.NewProjectile(source, itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 1.35f, velocity, ModContent.ProjectileType<ShockblastRound>(), damage * 3, knockback * 5f, player.whoAmI, 0f, 10f);
                return false;
            }

            // Pearl bullets
            if (!swapType)
            {
                for (int k = 0; k < 2; k++)
                {
                    int randomColor = Main.rand.Next(1, 3 + 1);
                    Color color = randomColor == 1 ? Color.LightBlue : randomColor == 2 ? Color.LightPink : Color.Khaki;
                    SparkParticle spark = new SparkParticle(itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 1.35f, velocity.RotatedByRandom(0.25) * Main.rand.NextFloat(0.2f, 1.5f), false, Main.rand.Next(20, 25 + 1), Main.rand.NextFloat(0.4f, 0.65f), color);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                for (int k = 0; k < 6; k++)
                {
                    int randomColor = Main.rand.Next(1, 3 + 1);
                    Color color = randomColor == 1 ? Color.LightBlue : randomColor == 2 ? Color.LightPink : Color.Khaki;
                    PearlParticle pearl1 = new PearlParticle(itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 1.35f, velocity.RotatedByRandom(0.25) * Main.rand.NextFloat(0.2f, 1f), false, Main.rand.Next(40, 45 + 1), Main.rand.NextFloat(0.6f, 0.75f), color, 0.95f, Main.rand.NextFloat(1, -1), true);
                    GeneralParticleHandler.SpawnParticle(pearl1);
                }
                int shotColor = Main.rand.Next(1, 3 + 1);
                for (int k = 0; k < 3; k++)
                {
                    Projectile pearlShot = Projectile.NewProjectileDirect(source, itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 1.65f, velocity.RotatedBy(k == 0 ? 0 : k == 1 ? 0.025f : -0.025f), type, damage, knockback, player.whoAmI);
                    CalamityGlobalProjectile cgp = pearlShot.Calamity();
                    if (shotColor == 1)
                        cgp.pearlBullet1 = true; // blue
                    if (shotColor == 2)
                        cgp.pearlBullet2 = true; // pink
                    if (shotColor == 3)
                        cgp.pearlBullet3 = true; // yellow

                    if (shotColor < 3)
                        shotColor++;
                    else
                        shotColor = 1;

                }
            }
            // Life bullet
            if (swapType)
            {
                for (int k = 0; k < 8; k++)
                {
                    int randomColor = Main.rand.Next(1, 3 + 1);
                    Color color = randomColor == 1 ? Color.LightBlue : randomColor == 2 ? Color.LightPink : Color.Khaki;
                    Particle spark = new SparkParticle(itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 1.35f, velocity.RotatedByRandom(0.25) * Main.rand.NextFloat(0.2f, 1.5f), false, Main.rand.Next(20, 25 + 1), Main.rand.NextFloat(0.4f, 0.65f), color);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                for (int k = 0; (k < 13); k++)
                {
                    int randomColor = Main.rand.Next(1, 3 + 1);
                    Color color = randomColor == 1 ? Color.LightBlue : randomColor == 2 ? Color.LightPink : Color.Khaki;

                    Dust dust2 = Dust.NewDustPerfect(itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 1.35f, DustID.FireworksRGB, velocity.RotatedByRandom(0.25) * Main.rand.NextFloat(0.1f, 0.9f));
                    dust2.noGravity = true;
                    dust2.scale = Main.rand.NextFloat(0.3f, 0.45f);
                    dust2.color = color;
                }
                Projectile lifeShot = Projectile.NewProjectileDirect(source, itemPosition + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.65f, velocity, type, damage, knockback, player.whoAmI);
                if (!healVisual)
                {
                    CalamityGlobalProjectile cgp = lifeShot.Calamity();
                    cgp.betterLifeBullet1 = true;
                }
                if (healVisual)
                {
                    CalamityGlobalProjectile cgp = lifeShot.Calamity();
                    cgp.betterLifeBullet2 = true;
                }
                healVisual = !healVisual;
            }

            swapType = !swapType;
            return false;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            CalamityUtils.CleanHoldStyle(player, new Vector2(80, 46), new Vector2(-24, 4), 7f);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += -0.03f * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Arietes41>().
                AddIngredient<LifeAlloy>(5).
                AddIngredient<RuinousSoul>(2).
                AddIngredient(ItemID.WhitePearl).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
