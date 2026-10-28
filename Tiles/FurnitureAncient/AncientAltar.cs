using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureAncient
{
    public class AncientAltar : BaseRedTorchStoneDustTile
    {
        public Asset<Texture2D> FlameTexture;

        int animationFrame = 0;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(191, 142, 111), CalamityUtils.GetItemName<Items.Placeables.FurnitureAncient.AncientAltar>());
            AnimationFrameHeight = 54;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 6;
                animationFrame = frame;
                frameCounter = 0;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 0.5f;
            b = 0.5f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            FlameTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureAncient/AncientAltarFlame");
            CalamityUtils.DrawStaticFlameEffect(FlameTexture.Value, i, j, offsetY: animationFrame * AnimationFrameHeight);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX == 18 && tile.TileFrameY == 18)
            {
                CalamityUtils.DrawFlameSparks(60, 5, i, j);
            }
        }
    }
}
