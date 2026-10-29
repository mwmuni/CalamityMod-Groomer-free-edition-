using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureDriftwood
{
    public class DriftwoodBed : BaseBedTile
    {
        protected override int AssociatedItemType => ModContent.ItemType<Items.Placeables.FurnitureDriftwood.DriftwoodBed>();

        public override void SetStaticDefaults() => this.SetUpBed(ModContent.ItemType<Items.Placeables.FurnitureDriftwood.DriftwoodBed>(), true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.LifeDrain, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
    }
}
