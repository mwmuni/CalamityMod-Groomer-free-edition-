using Terraria.ModLoader;

namespace CalamityMod
{
    public class MeleeRangedHybridDamageClass : CalamityDamageClass<MeleeRangedHybridDamageClass>
    {
        internal static readonly StatInheritanceData FiftyPercentBoost = new(0.5f, 0.5f, 0.5f, 0.5f, 0.5f);

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Melee || damageClass == Ranged)
                return FiftyPercentBoost;
            if (damageClass == Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        // Inherits from both melee and ranged
        public override bool GetEffectInheritance(DamageClass damageClass) => damageClass == Melee || damageClass == Ranged;
    }
}
