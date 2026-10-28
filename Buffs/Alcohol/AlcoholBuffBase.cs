using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public abstract class AlcoholBuffBase : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }
    }
}
