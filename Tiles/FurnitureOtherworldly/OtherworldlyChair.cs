using CalamityMod.Dusts.Furniture;
﻿using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureOtherworldly
{
    [LegacyName("OccultChair")]
    public class OtherworldlyChair : BaseChairTile<Items.Placeables.FurnitureOtherworldly.OtherworldlyChair>
    {
        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Stone, 0f, 0f, 1, new Color(125, 94, 128), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<OtherworldlyTileCloth>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
    }
}
