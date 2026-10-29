using System;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.DataStructures
{
    public enum AttunementID : byte
    {
        Default, Hot, Cold, Evil,  //Broken biome blade
        TrueDefault, TrueHot, TrueCold, TrueTropical, TrueEvil, Holy, //Biome blade
        Whirlwind, FlailBlade, SuperPogo, Shockwave, //True biome blade
        Phoenix, Aries, Polaris, Andromeda //Galaxia
    }

    // TODO -- Attunements should be managed by an Attunement ModSystem
    public abstract class Attunement
    {
        public static Attunement[] attunementArray;

        public static void Load()
        {
            attunementArray = new Attunement[] {
                  new DefaultAttunement(), new HotAttunement(), new ColdAttunement(), new EvilAttunement(),
                  new TrueDefaultAttunement(), new TrueHotAttunement(), new TrueColdAttunement(), new TrueTropicalAttunement(), new TrueEvilAttunement(), new HolyAttunement(),
                  new WhirlwindAttunement(), new FlailBladeAttunement(), new SuperPogoAttunement(), new ShockwaveAttunement(),
                  new PhoenixAttunement(), new AriesAttunement(), new PolarisAttunement(), new AndromedaAttunement(),
                  null
             };
        }

        public static void Unload()
        {
            attunementArray = null;
        }

        public AttunementID id;

        public virtual LocalizedText AttunementName => CalamityUtils.GetText($"Attunement.{GetType().Name}.Name");
        public virtual LocalizedText FunctionText => CalamityUtils.GetText($"Attunement.{GetType().Name}.Function");
        public virtual LocalizedText PassiveName => id >= AttunementID.Phoenix ? CalamityUtils.GetText($"Attunement.{GetType().Name}.PassiveName") : LocalizedText.Empty; //Used for galaxia
        public virtual LocalizedText PassiveDesc => id >= AttunementID.Whirlwind ? CalamityUtils.GetText($"Attunement.{GetType().Name}.PassiveDesc") : LocalizedText.Empty; //Used for True Biome blade and onwards
        public Color tooltipColor;
        public Color tooltipColor2;
        public Color tooltipPassiveColor;

        public Color energyParticleEdgeColor;
        public Color energyParticleCenterColor;

        public virtual void ApplyStats(Item item)
        {
        }

        public virtual float DamageMultiplier => 1f;

        /// <summary>
        /// What does the sword do when trying to shoot and having the current attunement. Return false to stop the sword from shooting its default projectile
        /// </summary>
        /// <param name="player"></param>
        /// <param name="position"></param>
        /// <param name="speedX"></param>
        /// <param name="speedY"></param>
        /// <param name="type"></param>
        /// <param name="damage"></param>
        /// <param name="knockBack"></param>
        /// <param name="Combo">Used by a different attunements to check the combo state of the weapon</param>
        /// <param name="CanLunge">>Used by evil attunements to check if a regular dash is avaialble</param>
        /// <param name="PowerLungeCounter">Used by evil attunements to check if a strong dash is avaialble</param>
        /// <returns></returns>
        public virtual bool Shoot(Player player, IEntitySource source, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack, ref int Combo, ref int CanLunge, ref int PowerLungeCounter)
        {
            Projectile blade = Projectile.NewProjectileDirect(source, position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            blade.originalDamage = (int)MathF.Round(blade.originalDamage * DamageMultiplier);
            return false;
        }

        public virtual void PassiveEffect(Player player, IEntitySource source, ref int UseTimer, ref bool Procced, Projectile projectile = null)
        {
        }
    }

    internal static class AttunementStats
    {
        internal static void ApplyProjectileStats<TProjectile>(Item item, float shootSpeed, bool channel = true, int useStyle = ItemUseStyleID.Shoot, int? reuseDelay = null)
            where TProjectile : ModProjectile
        {
            item.channel = channel;
            item.noUseGraphic = true;
            item.useStyle = useStyle;
            item.shoot = ProjectileType<TProjectile>();
            item.shootSpeed = shootSpeed;
            item.UseSound = null;
            item.noMelee = true;
            if (reuseDelay.HasValue)
                item.reuseDelay = reuseDelay.Value;
        }
    }


    #region Broken Biome Blade Attunements
    public class DefaultAttunement : Attunement
    {
        public DefaultAttunement()
        {
            id = AttunementID.Default;
            tooltipColor = new Color(171, 180, 73);

            energyParticleEdgeColor = new Color(117, 126, 72);
            energyParticleCenterColor = new Color(200, 184, 136);
        }

        public override float DamageMultiplier => BrokenBiomeBlade.DefaultAttunement_BaseDamage / (float)BrokenBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<PureClarity>(item, 0f);
    }
    public class HotAttunement : Attunement
    {
        public HotAttunement()
        {
            id = AttunementID.Hot;
            tooltipColor = new Color(238, 156, 73);

            energyParticleEdgeColor = new Color(137, 32, 0);
            energyParticleCenterColor = new Color(209, 154, 0);
        }

        public override float DamageMultiplier => BrokenBiomeBlade.HotAttunement_BaseDamage / (float)BrokenBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<AridGrandeur>(item, 12f);
    }
    public class ColdAttunement : Attunement
    {
        public ColdAttunement()
        {
            id = AttunementID.Cold;
            tooltipColor = new Color(165, 235, 235);

            energyParticleEdgeColor = new Color(165, 235, 235);
            energyParticleCenterColor = new Color(58, 110, 141);
        }

        public override float DamageMultiplier => BrokenBiomeBlade.ColdAttunement_BaseDamage / (float)BrokenBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<BitingEmbrace>(item, 12f);

        public override bool Shoot(Player player, IEntitySource source, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack, ref int Combo, ref int CanLunge, ref int PowerLungeCounter)
        {
            Projectile blade = Projectile.NewProjectileDirect(source, player.Center, new Vector2(speedX, speedY), ProjectileType<BitingEmbrace>(), damage, knockBack, player.whoAmI, 0, 15);
            blade.originalDamage = (int)MathF.Round(blade.originalDamage * DamageMultiplier);
            if (Combo > 0)
            {
                blade.ai[0] = Combo;
                blade.ai[1] = (Combo == 2 ? 50 : 20);
            }

            Combo++;
            if (Combo > 2)
                Combo = 0;
            return false;
        }
    }

    public class EvilAttunement : Attunement
    {
        public EvilAttunement()
        {
            id = AttunementID.Evil;
            tooltipColor = new Color(211, 64, 147);

            energyParticleEdgeColor = new Color(112, 4, 35);
            energyParticleCenterColor = new Color(195, 42, 200);
        }

        public override float DamageMultiplier => BrokenBiomeBlade.EvilAttunement_BaseDamage / (float)BrokenBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<DecaysRetort>(item, 12f, channel: false, useStyle: ItemUseStyleID.Thrust);

        public override bool Shoot(Player player, IEntitySource source, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack, ref int Combo, ref int CanLunge, ref int PowerLungeCounter)
        {
            Projectile blade = Projectile.NewProjectileDirect(source, player.Center, new Vector2(speedX, speedY), ProjectileType<DecaysRetort>(), damage, knockBack, player.whoAmI, 26, (float)CanLunge);
            blade.originalDamage = (int)MathF.Round(blade.originalDamage * DamageMultiplier);
            CanLunge = 0;
            return false;
        }
    }
    #endregion

    #region Biome Blade Attunements
    public class TrueDefaultAttunement : Attunement
    {
        public TrueDefaultAttunement()
        {
            id = AttunementID.TrueDefault;
            tooltipColor = new Color(201, 220, 93);

            energyParticleEdgeColor = new Color(117, 126, 72);
            energyParticleCenterColor = new Color(200, 184, 136);
        }

        public override float DamageMultiplier => TrueBiomeBlade.DefaultAttunement_BaseDamage / (float)TrueBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<TruePureClarity>(item, 0f);
    }
    public class TrueHotAttunement : Attunement
    {
        public TrueHotAttunement()
        {
            id = AttunementID.TrueHot;
            tooltipColor = new Color(238, 156, 73);

            energyParticleEdgeColor = new Color(137, 32, 0);
            energyParticleCenterColor = new Color(209, 154, 0);
        }

        public override float DamageMultiplier => TrueBiomeBlade.HotAttunement_BaseDamage / (float)TrueBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<TrueAridGrandeur>(item, 12f);
    }
    public class TrueColdAttunement : Attunement
    {
        public TrueColdAttunement()
        {
            id = AttunementID.TrueCold;
            tooltipColor = new Color(165, 235, 235);

            energyParticleEdgeColor = new Color(165, 235, 235);
            energyParticleCenterColor = new Color(58, 110, 141);
        }

        public override float DamageMultiplier => TrueBiomeBlade.ColdAttunement_BaseDamage / (float)TrueBiomeBlade.BaseDamage;
        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<TrueBitingEmbrace>(item, 12f);

        public override bool Shoot(Player player, IEntitySource source, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack, ref int Combo, ref int CanLunge, ref int PowerLungeCounter)
        {
            Projectile blade = Projectile.NewProjectileDirect(source, player.Center, new Vector2(speedX, speedY), ProjectileType<TrueBitingEmbrace>(), damage, knockBack, player.whoAmI, 0, 15);
            blade.originalDamage = (int)MathF.Round(blade.originalDamage * DamageMultiplier);
            if (Combo > 0)
            {
                blade.ai[0] = Combo;
                blade.ai[1] = (Combo == 2 ? 50 : 20);
            }

            Combo++;
            if (Combo > 2)
                Combo = 0;
            return false;
        }
    }

    public class TrueTropicalAttunement : Attunement
    {
        public TrueTropicalAttunement()
        {
            id = AttunementID.TrueTropical;
            tooltipColor = new Color(162, 200, 85);

            energyParticleEdgeColor = new Color(53, 112, 4);
            energyParticleCenterColor = new Color(131, 173, 39);
        }

        public override float DamageMultiplier => TrueBiomeBlade.TropicalAttunement_BaseDamage / (float)TrueBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<TrueGrovetendersTouch>(item, 30, channel: false, useStyle: ItemUseStyleID.Swing);

        public override bool Shoot(Player player, IEntitySource source, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack, ref int Combo, ref int CanLunge, ref int PowerLungeCounter)
        {
            Projectile whipProj = Projectile.NewProjectileDirect(source, player.Center, new Vector2(speedX, speedY), ProjectileType<TrueGrovetendersTouch>(), damage, knockBack, player.whoAmI, 0, 0);
            whipProj.originalDamage = (int)MathF.Round(whipProj.originalDamage * DamageMultiplier);
            if (whipProj.ModProjectile is TrueGrovetendersTouch whip)
                whip.flipped = Combo == 0 ? 1 : -1;
            Combo++;
            if (Combo > 1)
                Combo = 0;
            return false;
        }
    }

    public class TrueEvilAttunement : Attunement
    {
        public TrueEvilAttunement()
        {
            id = AttunementID.TrueEvil;
            tooltipColor = new Color(211, 64, 147);

            energyParticleEdgeColor = new Color(112, 4, 35);
            energyParticleCenterColor = new Color(195, 42, 200);
        }

        public override float DamageMultiplier => TrueBiomeBlade.EvilAttunement_BaseDamage / (float)TrueBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<TrueDecaysRetort>(item, 12f, channel: false, useStyle: ItemUseStyleID.Thrust);

        public override bool Shoot(Player player, IEntitySource source, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack, ref int Combo, ref int CanLunge, ref int PowerLungeCounter)
        {
            bool powerLungeAvailable = false;
            if (PowerLungeCounter == 3)
            {
                powerLungeAvailable = true;
                PowerLungeCounter = 0;
            }
            Projectile proj = Projectile.NewProjectileDirect(source, player.Center, new Vector2(speedX, speedY), ProjectileType<TrueDecaysRetort>(), damage, knockBack, player.whoAmI, 26f, CanLunge > 0 ? 1f : 0f);
            proj.originalDamage = (int)MathF.Round(proj.originalDamage * DamageMultiplier);
            if (proj.ModProjectile is TrueDecaysRetort rapier)
                rapier.ChargedUp = powerLungeAvailable;
            CanLunge--;
            if (CanLunge < 0)
                CanLunge = 0;
            return false;
        }
    }

    public class HolyAttunement : Attunement
    {
        public HolyAttunement()
        {
            id = AttunementID.Holy;
            tooltipColor = new Color(220, 143, 255);

            energyParticleEdgeColor = new Color(62, 55, 110);
            energyParticleCenterColor = new Color(255, 143, 255);
        }

        public override float DamageMultiplier => TrueBiomeBlade.HolyAttunement_BaseDamage / (float)TrueBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<HeavensMight>(item, 12f);
    }
    #endregion

    #region True Biome Blade Attunements
    public class WhirlwindAttunement : Attunement
    {
        public WhirlwindAttunement()
        {
            id = AttunementID.Whirlwind;
            tooltipColor = new Color(220, 105, 197);
            tooltipColor2 = new Color(132, 70, 232);
        }

        public override float DamageMultiplier => OmegaBiomeBlade.WhirlwindAttunement_BaseDamage / (float)OmegaBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<SwordsmithsPride>(item, 12f);

        public override void PassiveEffect(Player player, IEntitySource source, ref int UseTimer, ref bool Procced, Projectile projectile = null)
        {
            if (Procced)
            {
                int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(OmegaBiomeBlade.WhirlwindAttunement_PassiveBaseDamage);
                // 14NOV2024: Ozzatron: clamped mouse world unnecessary here, just used for direction
                Vector2 velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX) * 15f;
                Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<SwordsmithsPrideAstralBomber>(), damage, 0f, player.whoAmI);

                Procced = false;
            }
        }
    }

    public class FlailBladeAttunement : Attunement
    {
        public FlailBladeAttunement()
        {
            id = AttunementID.FlailBlade;
            tooltipColor = new Color(113, 239, 177);
            tooltipColor2 = new Color(169, 207, 255);
        }

        public override float DamageMultiplier => OmegaBiomeBlade.FlailBladeAttunement_BaseDamage / (float)OmegaBiomeBlade.BaseDamage;
        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<LamentationsOfTheChained>(item, 12f);

        public override void PassiveEffect(Player player, IEntitySource source, ref int UseTimer, ref bool Procced, Projectile projectile = null)
        {
            if (Procced)
            {
                if (projectile.ModProjectile is ChainedMeatHook hook && hook.Twirling == 0f)
                {
                    hook.Twirling = 1f;
                    hook.Projectile.timeLeft = (int)ChainedMeatHook.MaxTwirlTime;
                }
                Procced = false;
            }
        }
    }

    public class SuperPogoAttunement : Attunement
    {
        public SuperPogoAttunement()
        {
            id = AttunementID.SuperPogo;
            tooltipColor = new Color(216, 55, 22);
            tooltipColor2 = new Color(216, 131, 22);
        }

        public override float DamageMultiplier => OmegaBiomeBlade.SuperPogoAttunement_BaseDamage / (float)OmegaBiomeBlade.BaseDamage;
        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<SanguineFury>(item, 12f);

        public override void PassiveEffect(Player player, IEntitySource source, ref int UseTimer, ref bool Procced, Projectile projectile = null)
        {
            if (Procced)
            {
                player.DoLifestealDirect(null, OmegaBiomeBlade.SuperPogoAttunement_PassiveLifeSteal);
                Procced = false;
            }
        }
    }

    public class ShockwaveAttunement : Attunement
    {
        public ShockwaveAttunement()
        {
            id = AttunementID.Shockwave;
            tooltipColor = new Color(71, 191, 71);
            tooltipColor2 = new Color(122, 213, 233);
        }

        public override float DamageMultiplier => OmegaBiomeBlade.ShockwaveAttunement_BaseDamage / (float)OmegaBiomeBlade.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<EarthenTides>(item, 12f);

        public override void PassiveEffect(Player player, IEntitySource source, ref int UseTimer, ref bool Procced, Projectile projectile = null)
        {
            if (UseTimer % 120 == 119)
            {
                int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(OmegaBiomeBlade.ShockwaveAttunement_PassiveBaseDamage);
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<EarthenTidesShockwave>(), damage, 10f, player.whoAmI, 2f);
                UseTimer++;
            }
        }
    }

    #endregion

    #region Galaxia Attunements

    public class PhoenixAttunement : Attunement
    {
        public PhoenixAttunement()
        {
            id = AttunementID.Phoenix;
            tooltipColor = new Color(255, 87, 0);
            tooltipColor2 = new Color(255, 143, 0);

            tooltipPassiveColor = new Color(76, 137, 237);
        }

        public override float DamageMultiplier => FourSeasonsGalaxia.PhoenixAttunement_BaseDamage / (float)FourSeasonsGalaxia.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<PhoenixsPride>(item, 12f, reuseDelay: 12);
    }

    public class AriesAttunement : Attunement
    {
        public AriesAttunement()
        {
            id = AttunementID.Aries;
            tooltipColor = new Color(196, 89, 201);
            tooltipColor2 = new Color(255, 0, 0);

            tooltipPassiveColor = new Color(76, 137, 237);
        }

        public override float DamageMultiplier => FourSeasonsGalaxia.AriesAttunement_BaseDamage / (float)FourSeasonsGalaxia.BaseDamage;
        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<AriesWrath>(item, 12f, reuseDelay: 12);
    }

    public class PolarisAttunement : Attunement
    {
        public PolarisAttunement()
        {
            id = AttunementID.Polaris;
            tooltipColor = new Color(128, 189, 255);
            tooltipColor2 = new Color(255, 128, 140);

            tooltipPassiveColor = new Color(203, 25, 119);
        }

        public override float DamageMultiplier => FourSeasonsGalaxia.PolarisAttunement_BaseDamage / (float)FourSeasonsGalaxia.BaseDamage;
        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<PolarisGaze>(item, 12f, reuseDelay: 12);
    }

    public class AndromedaAttunement : Attunement
    {
        public AndromedaAttunement()
        {
            id = AttunementID.Andromeda;
            tooltipColor = new Color(132, 128, 255);
            tooltipColor2 = new Color(194, 166, 255);

            tooltipPassiveColor = new Color(203, 25, 119);
        }

        public override float DamageMultiplier => FourSeasonsGalaxia.AndromedaAttunement_BaseDamage / (float)FourSeasonsGalaxia.BaseDamage;

        public override void ApplyStats(Item item) => AttunementStats.ApplyProjectileStats<AndromedasStride>(item, 12f, reuseDelay: 12);
    }

    #endregion
}
