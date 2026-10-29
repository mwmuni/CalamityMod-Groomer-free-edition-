using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class DevilFishAlt : DevilFishBase
    {
        public static Asset<Texture2D> GlowTexture;

        protected override int BannerType => ModContent.NPCType<DevilFish>();
        protected override Asset<Texture2D> GlowTextureAsset => GlowTexture;
        protected override string GetMaskGoreName(int index) => "DevilFishMask" + index + (index == 3 ? "Alt" : "");
        protected override string GetDeathGoreName(int index) => index == 1 ? "DevilfishAlt" : "DevilfishAlt" + index;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            this.HideFromBestiary();
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/DevilFishGlowAlt", AssetRequestMode.AsyncLoad);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => DevilFish.DefineDevilFishLoot(npcLoot);
    }
}
