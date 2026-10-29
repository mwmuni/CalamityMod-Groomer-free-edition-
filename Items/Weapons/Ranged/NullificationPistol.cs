using System;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("NullificationRifle")]
    public class NullificationPistol : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public bool shotType = true; // true = positive shot, false = negative shot
        public float mult = 0;
        public static SoundStyle HitSound = new("CalamityMod/Sounds/Item/NullHit");

        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 28;
            Item.damage = 190;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0.1f;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<NullShot>();
        }
        public override bool AltFunctionUse(Player player) => true;
        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.3f - (float)(1 - Math.Pow(Utils.GetLerpValue(0, shotType ? 0.175f : 0.25f, mult, true), 2.5f));

            if (Main.zenithWorld)
                return 3 * (shotType ? 0.7f : 1);

            if (!shotType)
                return 1.5f - ((mult > 0.25f) ? mult : 0);

            return 1f - ((mult > 0.175f) ? mult : 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/DudFire");
                SoundEngine.PlaySound(fire with { Volume = 0.7f, Pitch = -0.5f + (shotType ? 0.5f : 0) }, position);
                for (int i = 0; i < 18; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position, shotType ? ModContent.DustType<VoidDust>() : ModContent.DustType<LightDust>(), (velocity).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.9f, 1.25f);
                    dust.color = shotType ? Color.White : (Main.rand.NextBool() ? Color.Orchid : Color.Turquoise);
                }
                shotType = !shotType;
                mult = 0;
            }
            else if (shotType)
            {
                for (int i = 0; i < 3; i++)
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<NullShot>(), damage / 3, 0, player.whoAmI, 0, 0, i);
                if (mult < 0.175f)
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<NullFlash>(), damage, 0, player.whoAmI, 0, 0);
                if (mult < 0.35f)
                    mult += 0.013f;
            }
            else
            {
                Projectile aBeam = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<NullShot>(), (int)(damage * 0.7f), 0, player.whoAmI, 0, 5);
                aBeam.penetrate = 1;
                if (mult < 0.25f)
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<NullFlash>(), (int)(damage * 0.75f), 0, player.whoAmI, 0, 5);
                if (mult < 0.5f)
                    mult += 0.013f;
            }
            if (player.altFunctionUse != 2)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/NullShot");
                SoundEngine.PlaySound(fire with { Volume = 0.7f, Pitch = Main.rand.NextFloat(0f, 0.2f) + (!shotType ? 0.3f : 0) - (shotType ? ((mult > 0.25f) ? mult : 0) : ((mult > 0.175f) ? mult : 0)) }, position);

                if (mult >= (shotType ? 0.175f : 0.25f))
                {
                    for (int i = 0; i < 18; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(position + velocity * 8, ModContent.DustType<UnstableDust>(), (velocity).RotatedByRandom(0.5) * Main.rand.NextFloat(0.4f, 1f));
                        dust.noGravity = !Main.rand.NextBool(4);
                        dust.scale = Main.rand.NextFloat(1.2f, 1.8f);
                        dust.color = Main.rand.NextBool() ? Color.White : (Main.rand.NextBool() ? Color.Orchid : Color.Turquoise);
                        dust.fadeIn = 7.5f;
                    }
                }
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

            CalamityUtils.CleanHoldStyle(player, itemRotation, new Vector2(52, 28), new Vector2(-24, 4), pullback);

            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += (player.altFunctionUse == 2 ? -0.15f : 0) * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
    }
}
