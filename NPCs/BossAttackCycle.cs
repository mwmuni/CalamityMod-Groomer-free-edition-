using System;

namespace CalamityMod.NPCs
{
    internal static class BossAttackCycle
    {
        // Callers own wrapping and invalid-step recovery; negative remainders must not wrap implicitly.
        internal static TAttack Select<TAttack>(ReadOnlySpan<TAttack> attacks, int step, TAttack fallback) =>
            step >= 0 && step < attacks.Length ? attacks[step] : fallback;
    }
}
