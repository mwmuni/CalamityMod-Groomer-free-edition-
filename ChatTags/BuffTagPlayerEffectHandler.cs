using Terraria.UI.Chat;

namespace CalamityMod.ChatTags
{
    public sealed class BuffTagPlayerEffectHandler : BuffTagEffectHandler<BuffTagPlayerEffectHandler>
    {
        public sealed class Snippet : BuffTagSnippet
        {
            public Snippet(int buffId) : base(buffId)
            {
            }
        }

        protected override string[] TagNames { get; } = ["cbuffplr"];

        protected override TextSnippet CreateSnippet(int buffId) => new Snippet(buffId);
    }
}
