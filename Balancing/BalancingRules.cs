using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Balancing
{
    public interface IBalancingRule
    {
        bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile);

        void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers);
    }

    public abstract class SourceDamageBalancingRule
    {
        protected abstract float SourceDamageMultiplier { get; }

        protected void ApplySourceDamageBalancingChange(ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= SourceDamageMultiplier;
        }
    }

    public class ClassResistBalancingRule : SourceDamageBalancingRule, IBalancingRule
    {
        public float DamageMultiplier;
        public DamageClass ApplicableClass;
        public ClassResistBalancingRule(float damageMultiplier, DamageClass dc)
        {
            DamageMultiplier = damageMultiplier;
            ApplicableClass = dc;
        }

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile)
        {
            // E.g. so that a melee resist also applies to MeleeNoSpeed, TrueMelee, and TrueMeleeNoSpeed.
            return modifiers.DamageType.CountsAsClass(ApplicableClass);
        }

        protected override float SourceDamageMultiplier => DamageMultiplier;

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers) => ApplySourceDamageBalancingChange(ref modifiers);
    }

    public class NPCSpecificRequirementBalancingRule : IBalancingRule
    {
        public NPCApplicationRequirement Requirement;
        public delegate bool NPCApplicationRequirement(NPC npc);
        public NPCSpecificRequirementBalancingRule(NPCApplicationRequirement npcApplicationRequirement)
        {
            Requirement = npcApplicationRequirement;
        }

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile) => Requirement(npc);

        // This "balancing" rule doesn't actually perform any changes. It simply serves as a means of enforcing NPC-specific requirements, and should be used only as a filter.
        // As such, this method is empty.
        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers) { }
    }

    public class PierceResistBalancingRule : SourceDamageBalancingRule, IBalancingRule
    {
        public float DamageMultiplier;
        public PierceResistBalancingRule(float damageMultiplier) => DamageMultiplier = damageMultiplier;

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile)
        {
            return projectile is not null && (projectile.maxPenetrate > 1 || projectile.maxPenetrate == -1);
        }

        protected override float SourceDamageMultiplier => DamageMultiplier;

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers) => ApplySourceDamageBalancingChange(ref modifiers);
    }

    public class ProjectileResistBalancingRule : SourceDamageBalancingRule, IBalancingRule
    {
        public float DamageMultiplier;
        public int[] ApplicableProjectileTypes;
        public ProjectileResistBalancingRule(float damageMultiplier, params int[] projTypes)
        {
            DamageMultiplier = damageMultiplier;
            ApplicableProjectileTypes = projTypes;
        }

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile)
        {
            return projectile is not null && ApplicableProjectileTypes.Contains(projectile.type);
        }

        protected override float SourceDamageMultiplier => DamageMultiplier;

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers) => ApplySourceDamageBalancingChange(ref modifiers);
    }

    public class ProjectileSpecificRequirementBalancingRule : SourceDamageBalancingRule, IBalancingRule
    {
        public float DamageMultiplier;
        public ProjectileApplicationRequirement Requirement;
        public delegate bool ProjectileApplicationRequirement(Projectile proj);
        public ProjectileSpecificRequirementBalancingRule(float damageMultiplier, ProjectileApplicationRequirement projApplicationRequirement)
        {
            DamageMultiplier = damageMultiplier;
            Requirement = projApplicationRequirement;
        }

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile)
        {
            return projectile is not null && Requirement(projectile);
        }

        protected override float SourceDamageMultiplier => DamageMultiplier;

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers) => ApplySourceDamageBalancingChange(ref modifiers);
    }

    // TODO -- this rule type should be deleted once stealth is finished as its own class
    public class StealthStrikeBalancingRule : SourceDamageBalancingRule, IBalancingRule
    {
        public float DamageMultiplier;
        public int[] ApplicableProjectileTypes;
        public StealthStrikeBalancingRule(float damageMultiplier, params int[] projTypes)
        {
            DamageMultiplier = damageMultiplier;
            ApplicableProjectileTypes = projTypes;
        }

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile)
        {
            return projectile is not null && (modifiers.DamageType == StealthDamageClass.Instance || projectile.Calamity().stealthStrike);
        }

        protected override float SourceDamageMultiplier => DamageMultiplier;

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers) => ApplySourceDamageBalancingChange(ref modifiers);
    }
}
