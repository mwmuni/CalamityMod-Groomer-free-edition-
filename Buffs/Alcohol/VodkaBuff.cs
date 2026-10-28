using CalamityMod.DataStructures;
using CalamityMod.Items.Potions.Alcohol;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class VodkaBuff : AlcoholBuffBase
    {
        public static DebuffData debuffData = new DebuffData()
        {
            AlcoholLevel = 1
        };

        public override void Update(Player player, ref int buffIndex)
        {
            var cplayer = player.Calamity();
            cplayer.vodka = true;
            cplayer.TypelessDebuffMultiplier += Vodka.DebuffBoost;
            cplayer.HeatDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.ColdDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.SicknessDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.WaterDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.ElectricDebuffMultiplier -= Vodka.DebuffLoss;
        }
    }
}
