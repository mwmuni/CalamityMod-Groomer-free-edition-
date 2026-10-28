using CalamityMod.Items.Critters;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DraedonLabThings
{
    public class NanodroidPlagueRed : NanodroidPlagueBase
    {
        public static Asset<Texture2D> GlowTexture;

        protected override int CatchItemType => ModContent.ItemType<NanodroidPlagueRedItem>();
        protected override float LightRed => 0.3f;
        protected override float LightGreen => 0.03f;
        protected override float LightBlue => 0f;
        protected override Asset<Texture2D> GlowTextureAsset => GlowTexture;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.AsyncLoad);
            }
        }
    }
}
