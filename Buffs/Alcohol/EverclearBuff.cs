using CalamityMod.DataStructures;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class EverclearBuff : AlcoholBuffBase
    {
        public static DebuffData debuffData = new DebuffData()
        {
            AlcoholLevel = 2
        };

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().everclear = true;
        }
    }
}
