using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public abstract class PermanentBooster : ModItem, ILocalizedModType
    {
        protected abstract int BoosterWidth { get; }
        protected abstract int BoosterHeight { get; }
        protected abstract int BoosterRarity { get; }
        protected abstract bool IsConsumed(Player player);
        protected abstract void MarkConsumed(Player player);

        public new string LocalizationCategory => "Items.Misc";

        public override void SetDefaults()
        {
            Item.width = BoosterWidth;
            Item.height = BoosterHeight;
            Item.consumable = true;
            Item.useAnimation = Item.useTime = 30;
            Item.UseSound = SoundID.Item122;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = BoosterRarity;
            Item.SetRevExclusive();
        }

        public override bool CanUseItem(Player player) => !IsConsumed(player);

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = Item.useTime;
                MarkConsumed(player);
            }

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (IsConsumed(Main.LocalPlayer))
                list.AddConsumedTooltip("Tooltip0");
        }
    }
}
