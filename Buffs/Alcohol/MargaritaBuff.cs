using CalamityMod.DataStructures;
using CalamityMod.Items.Potions.Alcohol;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class MargaritaBuff : AlcoholBuffBase
    {
        public static DebuffData debuffData = new DebuffData()
        {
            AlcoholLevel = 1
        };

        public override void Update(Player player, ref int buffIndex)
        {
            var cplayer = player.Calamity();
            cplayer.margarita = true;
            cplayer.HeatDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.SicknessDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.ColdDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.WaterDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.ElectricDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.TypelessDebuffMultiplier -= Margarita.DebuffLoss;
        }
    }
}
