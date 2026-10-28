using Terraria.UI.Chat;

namespace CalamityMod.ChatTags
{
    public sealed class BuffTagEnemyEffectHandler : BuffTagEffectHandler<BuffTagEnemyEffectHandler>
    {
        public sealed class Snippet : BuffTagSnippet
        {
            public Snippet(int buffId) : base(buffId)
            {
            }
        }

        protected override string[] TagNames { get; } = ["cbuff"];

        protected override TextSnippet CreateSnippet(int buffId) => new Snippet(buffId);
    }
}
