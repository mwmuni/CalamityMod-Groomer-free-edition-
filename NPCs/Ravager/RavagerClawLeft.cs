using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    [HasPierceResist]
    public class RavagerClawLeft : RavagerClawBase
    {
        protected override int ClawSide => -1;
        protected override int DeathGoreCount => 3;
        protected override string GetDeathGoreName(int index) => index == 1 ? "ScavengerClawLeft" : $"ScavengerClawLeft{index}";
    }
}
