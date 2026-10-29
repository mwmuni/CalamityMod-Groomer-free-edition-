using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class ProfanedMoonlightDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/ProfanedMoonlightDye"), "DyePass");
        public override void SafeSetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ProfanedFlameDye>();
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }
    }
}
