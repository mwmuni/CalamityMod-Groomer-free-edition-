using CalamityMod.World;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomSpiritM : PhantomSpiritVariant
    {
        protected override int VariantDamage => 90;
        protected override int VariantDefense => 50;
        protected override int VariantLifeMax => 2500;
        protected override float VariantKnockBackResist => 0.15f;
        protected override int ValueInSilver => 40;
        protected override float? ScaleMultiplier => 1.1f;
        protected override float MovementSpeed => CalamityWorld.death ? 18f : CalamityWorld.revenge ? 15.75f : 13.5f;
        protected override int MinimumNecroplasmDrop => 1;
        protected override int MaximumNecroplasmDrop => 3;
    }
}
