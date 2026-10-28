using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAbyss
{
    public class AbyssCandelabra : BaseBlueWhiteFrameLightTile
    {
        public override void SetStaticDefaults() => this.SetUpCandelabra(ModContent.ItemType<Items.Placeables.FurnitureAbyss.AbyssCandelabra>(), true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Stone, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void HitWire(int i, int j)
        {
            FurnitureCommon.LightHitWire(Type, i, j, 2, 2);
        }
    }
}
