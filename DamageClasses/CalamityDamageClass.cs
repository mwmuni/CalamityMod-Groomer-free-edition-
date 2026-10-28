using Terraria.ModLoader;

namespace CalamityMod
{
    public abstract class CalamityDamageClass<TDamageClass> : DamageClass
        where TDamageClass : CalamityDamageClass<TDamageClass>
    {
        internal static TDamageClass Instance;

        public override void Load() => Instance = (TDamageClass)this;
        public override void Unload() => Instance = null;
    }
}
