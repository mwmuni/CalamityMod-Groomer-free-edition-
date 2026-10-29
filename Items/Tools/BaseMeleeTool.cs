using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public abstract class BaseMeleeTool : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";

        protected void SetMeleeToolDefaults(
            int width,
            int height,
            int damage,
            float knockBack,
            int useTime,
            int useAnimation,
            int sellValue,
            int rarity,
            int pickPower = 0,
            int axePower = 0,
            int hammerPower = 0,
            int tileBoost = 0,
            int crit = 0)
        {
            Item.width = width;
            Item.height = height;
            Item.damage = damage;
            Item.knockBack = knockBack;
            Item.useTime = useTime;
            Item.useAnimation = useAnimation;
            if (pickPower != 0)
                Item.pick = pickPower;
            if (axePower != 0)
                Item.axe = axePower;
            if (hammerPower != 0)
                Item.hammer = hammerPower;
            if (tileBoost != 0)
                Item.tileBoost += tileBoost;
            if (crit != 0)
                Item.crit = crit;

            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = sellValue;
            Item.rare = rarity;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }
    }
}
