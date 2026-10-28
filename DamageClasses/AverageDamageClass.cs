using Terraria.ModLoader;

namespace CalamityMod
{
    public class AverageDamageClass : CalamityDamageClass<AverageDamageClass>
    {
        internal static readonly StatInheritanceData TwentyPercentBoost = new(0.2f, 0.2f, 0.2f, 0.2f, 0.2f);

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Melee || damageClass == Ranged || damageClass == Magic || damageClass == Summon || damageClass == RogueDamageClass.Instance)
                return TwentyPercentBoost;
            if (damageClass == Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }
    }
}
