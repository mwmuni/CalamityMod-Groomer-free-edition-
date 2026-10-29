using CalamityMod.Items.Placeables.Furniture;
﻿using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture
{
    public class ThaumaticChairTile : BaseChairTile<ThaumaticChair>
    {
        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.WoodFurniture, 0f, 0f, 1, default, 1f);
            return false;
        }
    }
}
