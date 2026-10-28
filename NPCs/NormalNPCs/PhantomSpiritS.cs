using CalamityMod.World;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomSpiritS : PhantomSpiritVariant
    {
        protected override int VariantDamage => 80;
        protected override int VariantDefense => 45;
        protected override int VariantLifeMax => 2000;
        protected override float VariantKnockBackResist => 0.2f;
        protected override int ValueInSilver => 30;
        protected override float MovementSpeed => CalamityWorld.death ? 20f : CalamityWorld.revenge ? 17.5f : 15f;
        protected override int MinimumNecroplasmDrop => 1;
        protected override int MaximumNecroplasmDrop => 2;
    }
}
