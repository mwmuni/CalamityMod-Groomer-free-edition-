using CalamityMod.Items.Placeables.Abyss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.Abyss
{
    public class AbyssGiantKelp4Echo : BaseRubblemakerGlowEcho
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/AbyssGiantKelp4";
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(92, 93, 42));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;

            RegisterItemDrop(ModContent.ItemType<PlantyMush>());
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<PlantyMush>(), Type, 0);

            base.SetStaticDefaults();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameY <= 36)
            {
                r = 184f / 300f;
                g = 90f / 300f;
                b = 20f / 300f;
            }
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer && Main.rand.NextBool(100) && j > Main.worldSurface)
            {
                Dust dust;
                dust = Main.dust[Dust.NewDust(new Vector2(i * 16f, j * 16f), 280, 280, DustID.Firefly, 0.2f, 0f, 0, new Color(157, 175, 15), Main.rand.NextFloat(1f, 2f))];
                dust.noGravity = true;
                dust.noLight = true;
                dust.fadeIn = 2.5f;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }
}
