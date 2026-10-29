using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class Grax : BaseMeleeTool
    {
        private const int HammerPower = 110;
        private const int AxePower = 180 / 5;

        public static float DamageBoost = 0.15f;
        public static int DefenseBoost = 15;
        public static int GraxBoostDuration = CalamityUtils.SecondsToFrames(15);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), DefenseBoost, GraxBoostDuration.FramesToSeconds());
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            SetMeleeToolDefaults(
                62, 62, 472, 8f, 4, 16, CalamityGlobalItem.RarityTurquoiseBuyPrice, ModContent.RarityType<Turquoise>(),
                axePower: AxePower, hammerPower: HammerPower, tileBoost: 5);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            //TODO - Pressure the tmod devs into adding another method akin to ModifyToolStats that gets ran early enough to work.
            if (player.altFunctionUse == 2)
            {
                Item.axe = 0;
                Item.hammer = 0;
            }
            else
            {
                Item.axe = AxePower;
                Item.hammer = HammerPower;
            }
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<InfernaCutter>().
                AddRecipeGroup("LunarHamaxe").
                AddIngredient<UelibloomBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.AddBuff(ModContent.BuffType<GraxBoost>(), GraxBoostDuration);
        }
    }
}
