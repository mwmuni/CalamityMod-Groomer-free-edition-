using CalamityMod.Systems;
using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Crags
{
    public class BrimstoneSlag : BaseRedTorchStoneDustTile
    {
        private const short subsheetWidth = 450;
        private const short subsheetHeight = 198;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithHell(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<InfernalSuevite>());

            HitSound = SoundID.Tink;
            MineResist = 2f;
            MinPick = 100;
            AddMapEntry(new Color(53, 33, 56));
            this.RegisterBlendMergeWith(TileID.Ash);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 2 * subsheetWidth;
            frameYOffset = j % 2 * subsheetHeight;
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return TileFramingSystem.BrimstoneFraming(i, j, resetFrame);
        }
    }
}
