using Terraria.ModLoader;

namespace CalamityMod
{
    public class AllClassDamageClass : CalamityDamageClass<AllClassDamageClass>
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            return StatInheritanceData.Full;
        }
        public override bool GetEffectInheritance(DamageClass damageClass) => true;
    }
}
