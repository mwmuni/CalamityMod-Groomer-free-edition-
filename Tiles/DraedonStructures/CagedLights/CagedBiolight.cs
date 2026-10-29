using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public abstract class LightFixtureTile<TMapItem> : ModTile where TMapItem : ModItem
    {
        protected virtual bool IsMini => false;
        protected virtual int ItemDropType => -1;
        protected abstract int FixtureDustType { get; }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = false;
            if (ItemDropType >= 0)
                RegisterItemDrop(ItemDropType);

            HitSound = CommonCalamitySounds.PlatingMine;
            DustType = FixtureDustType;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(48, 201, 214), CalamityUtils.GetItemName<TMapItem>());

            TileObjectData.newTile.CopyFrom(IsMini ? TileObjectData.Style1x1 : TileObjectData.Style2x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 10;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.Origin = IsMini ? new Point16(0, 0) : new Point16(0, 1);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.Origin = IsMini ? new Point16(0, 0) : new Point16(1, 0);
            TileObjectData.addAlternate(2);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.addAlternate(4);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.addAlternate(6);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorWall = true;
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.Origin = IsMini ? new Point16(0, 0) : new Point16(1, 0);
            TileObjectData.addAlternate(8);
            TileObjectData.addTile(Type);
        }

    }

    public abstract class DustCountingLightFixtureTile<TMapItem> : LightFixtureTile<TMapItem> where TMapItem : ModItem
    {
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public abstract class CagedLightTile<TMapItem> : DustCountingLightFixtureTile<TMapItem> where TMapItem : ModItem
    {
        protected abstract Vector3 LightColor { get; }

        public override void HitWire(int i, int j)
        {
            int size = IsMini ? 1 : 2;
            FurnitureCommon.LightHitWire(Type, i, j, size, size);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                r = LightColor.X;
                g = LightColor.Y;
                b = LightColor.Z;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }
    }

    public abstract class MiniCagedLightTile<TMapItem> : CagedLightTile<TMapItem> where TMapItem : ModItem
    {
        protected override bool IsMini => true;
    }

    public abstract class AgedPulsingLightTile<TMapItem> : LightFixtureTile<TMapItem> where TMapItem : ModItem
    {
        protected abstract float Pulse { get; }
        protected abstract Vector3 LightColor { get; }
        protected virtual float AdjustBrightness(float brightness) => MathHelper.Clamp(brightness, 0.2f, 0.6f);
        protected virtual Vector2 LightPosition(int i, int j) => new Vector2(i * 16, j * 16);

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            float brightness = AdjustBrightness(Pulse);
            Lighting.AddLight(LightPosition(i, j), LightColor.X * brightness, LightColor.Y * brightness, LightColor.Z * brightness);
        }
    }

    public abstract class DustCountingAgedPulsingLightTile<TMapItem> : AgedPulsingLightTile<TMapItem> where TMapItem : ModItem
    {
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public abstract class MiniAgedPulsingLightTile<TMapItem> : DustCountingAgedPulsingLightTile<TMapItem> where TMapItem : ModItem
    {
        protected override bool IsMini => true;
    }

    public class CagedBiolight : CagedLightTile<CagedLablightItem>
    {
        protected override int ItemDropType => ModContent.ItemType<CagedBiolightItem>();
        protected override int FixtureDustType => DustID.GemEmerald;
        protected override Vector3 LightColor => new Vector3(9f / 255f, 195f / 255f, 0f);
    }
}
