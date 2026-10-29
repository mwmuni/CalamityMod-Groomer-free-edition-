using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    [HasPierceResist]
    public class RavagerClawRight : RavagerClawBase
    {
        protected override int ClawSide => 1;
        protected override int DeathGoreCount => 2;
        protected override string GetDeathGoreName(int index) => index == 1 ? "ScavengerClawRight" : $"ScavengerClawRight{index}";
    }
}
