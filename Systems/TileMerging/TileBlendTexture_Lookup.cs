using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using static CalamityMod.Systems.BlendSideFlags;

namespace CalamityMod.Systems
{
    public abstract partial class TileBlendTexture : ModTexturedType
    {
        // Welcome to Hardcoded hell
        // Do NOT change anything in here unless you understand absolutely everything

        #region Shape Definitions
        private enum ShapeFamily
        {
            None,
            Corner,
            I,
            L,
            U
        }

        private readonly record struct ShapeDefinition(
            BlendSideFlags MergeSides,
            int SheetX,
            int SheetY,
            int Increment,
            bool Vertical,
            ShapeFamily Family);

        private static readonly ShapeDefinition[] _ShapeDefinitions =
        [
            new(Shape_AllSide, 1, 1, 1, false, ShapeFamily.None),
            new(ShapeCorner_UpLeft, 1, 4, 2, false, ShapeFamily.Corner),
            new(ShapeCorner_UpRight, 0, 4, 2, false, ShapeFamily.Corner),
            new(ShapeCorner_DownLeft, 1, 3, 2, false, ShapeFamily.Corner),
            new(ShapeCorner_DownRight, 0, 3, 2, false, ShapeFamily.Corner),

            new(ShapeI_Up, 1, 2, 1, false, ShapeFamily.I),
            new(ShapeI_Up_End, 8, 6, 1, false, ShapeFamily.I),
            new(ShapeI_Up_LeftEnd, 6, 6, 1, true, ShapeFamily.I),
            new(ShapeI_Up_RightEnd, 7, 6, 1, true, ShapeFamily.I),
            new(ShapeI_Down, 1, 0, 1, false, ShapeFamily.I),
            new(ShapeI_Down_End, 8, 3, 1, false, ShapeFamily.I),
            new(ShapeI_Down_LeftEnd, 6, 3, 1, true, ShapeFamily.I),
            new(ShapeI_Down_RightEnd, 7, 3, 1, true, ShapeFamily.I),
            new(ShapeI_Left, 4, 0, 1, true, ShapeFamily.I),
            new(ShapeI_Left_End, 8, 0, 1, true, ShapeFamily.I),
            new(ShapeI_Left_UpEnd, 3, 9, 1, false, ShapeFamily.I),
            new(ShapeI_Left_DownEnd, 3, 10, 1, false, ShapeFamily.I),
            new(ShapeI_Right, 0, 0, 1, true, ShapeFamily.I),
            new(ShapeI_Right_End, 5, 0, 1, true, ShapeFamily.I),
            new(ShapeI_Right_UpEnd, 0, 9, 1, false, ShapeFamily.I),
            new(ShapeI_Right_DownEnd, 0, 10, 1, false, ShapeFamily.I),

            new(ShapeL_UpLeft, 0, 5, 2, false, ShapeFamily.L),
            new(ShapeL_UpLeft_End, 0, 7, 2, false, ShapeFamily.L),
            new(ShapeL_UpLeft_RightEnd, 11, 3, 2, true, ShapeFamily.L),
            new(ShapeL_UpLeft_DownEnd, 13, 3, 2, true, ShapeFamily.L),
            new(ShapeL_UpRight, 1, 5, 2, false, ShapeFamily.L),
            new(ShapeL_UpRight_End, 1, 7, 2, false, ShapeFamily.L),
            new(ShapeL_UpRight_LeftEnd, 12, 3, 2, true, ShapeFamily.L),
            new(ShapeL_UpRight_DownEnd, 14, 3, 2, true, ShapeFamily.L),
            new(ShapeL_DownLeft, 0, 6, 2, false, ShapeFamily.L),
            new(ShapeL_DownLeft_End, 0, 8, 2, false, ShapeFamily.L),
            new(ShapeL_DownLeft_RightEnd, 11, 4, 2, true, ShapeFamily.L),
            new(ShapeL_DownLeft_UpEnd, 13, 4, 2, true, ShapeFamily.L),
            new(ShapeL_DownRight, 1, 6, 2, false, ShapeFamily.L),
            new(ShapeL_DownRight_End, 1, 8, 2, false, ShapeFamily.L),
            new(ShapeL_DownRight_LeftEnd, 12, 4, 2, true, ShapeFamily.L),
            new(ShapeL_DownRight_UpEnd, 14, 4, 2, true, ShapeFamily.L),

            new(ShapeU_UpEmpty, 8, 5, 1, false, ShapeFamily.U),
            new(ShapeU_UpEmpty_End, 8, 8, 1, false, ShapeFamily.U),
            new(ShapeU_UpEmpty_LeftEnd, 8, 10, 1, false, ShapeFamily.U),
            new(ShapeU_UpEmpty_RightEnd, 11, 10, 1, false, ShapeFamily.U),
            new(ShapeU_DownEmpty, 8, 4, 1, false, ShapeFamily.U),
            new(ShapeU_DownEmpty_End, 8, 7, 1, false, ShapeFamily.U),
            new(ShapeU_DownEmpty_LeftEnd, 8, 9, 1, false, ShapeFamily.U),
            new(ShapeU_DownEmpty_RightEnd, 11, 9, 1, false, ShapeFamily.U),
            new(ShapeU_LeftEmpty, 7, 0, 1, true, ShapeFamily.U),
            new(ShapeU_LeftEmpty_End, 10, 0, 1, true, ShapeFamily.U),
            new(ShapeU_LeftEmpty_UpEnd, 12, 0, 1, true, ShapeFamily.U),
            new(ShapeU_LeftEmpty_DownEnd, 14, 0, 1, true, ShapeFamily.U),
            new(ShapeU_RightEmpty, 6, 0, 1, true, ShapeFamily.U),
            new(ShapeU_RightEmpty_End, 9, 0, 1, true, ShapeFamily.U),
            new(ShapeU_RightEmpty_UpEnd, 11, 0, 1, true, ShapeFamily.U),
            new(ShapeU_RightEmpty_DownEnd, 13, 0, 1, true, ShapeFamily.U)
        ];

        // TOTAL: 53
        private static readonly Dictionary<BlendSideFlags, Rectangle[]> _BasicShapeLookup = CreateBasicShapeLookup();

        private static Dictionary<BlendSideFlags, Rectangle[]> CreateBasicShapeLookup()
        {
            var lookup = new Dictionary<BlendSideFlags, Rectangle[]>(_ShapeDefinitions.Length);
            foreach (var shape in _ShapeDefinitions)
            {
                var rectangles = shape.Vertical
                    ? Create3RectsDirY(shape.SheetX, shape.SheetY, shape.Increment)
                    : Create3RectsDirX(shape.SheetX, shape.SheetY, shape.Increment);
                lookup.Add(shape.MergeSides, rectangles);
            }
            return lookup;
        }
        #endregion

        private static SheetPosition[] _SheetPositionLookup;

        private static void CalculateSheetPositionLookup()
        {
            if (_SheetPositionLookup != null)
                return;

            _SheetPositionLookup = new SheetPosition[256 * VariantCount];

            int bakeSheetIndex = 0;
            for (int i = 0; i < 256; i++)
            {
                bool hasAddedToSheet = false;

                for (byte randomFrame = 0; randomFrame < VariantCount; randomFrame++)
                {
                    var mergeSides = (BlendSideFlags)i;
                    var key = new SheetPositionKey(mergeSides, randomFrame);

                    if (_BasicShapeLookup.TryGetValue(mergeSides, out var rects))
                    {
                        var rect = rects[randomFrame];
                        _SheetPositionLookup[key] = new SheetPosition(rect.X, rect.Y, bakedSheetIndex: -1);
                    }
                    else
                    {
                        var y = Math.DivRem(bakeSheetIndex, BlendTextureXCount, out var x);
                        var heightOffset = BlendTextureHeight * randomFrame;
                        _SheetPositionLookup[key] = new SheetPosition(x * BlendTextureFrameWidth, (y * BlendTextureFrameHeight) + heightOffset, bakedSheetIndex: (sbyte)randomFrame);
                        hasAddedToSheet = true;
                    }
                }

                if (hasAddedToSheet)
                    bakeSheetIndex++;
            }
        }

        #region Shape Lookup

        /// <summary>
        /// An Descend Ordered Consume Lookup Table
        /// [U -> L -> I -> Corner] Order
        /// [Most flags -> Least flags] Order
        /// </summary>
        private static readonly IReadOnlyCollection<IReadOnlyCollection<BlendSideFlags>> _ShapeConsumeMap = [
            GetShapes(ShapeFamily.U, orderByFlagCount: true),
            GetShapes(ShapeFamily.L, orderByFlagCount: true),
            GetShapes(ShapeFamily.I, orderByFlagCount: true),
            // I shape includes up, down, left, right. So we don't need to consume them manually
            GetShapes(ShapeFamily.Corner, orderByFlagCount: false),
        ];

        private static IReadOnlyCollection<BlendSideFlags> GetShapes(ShapeFamily family, bool orderByFlagCount)
        {
            var shapes = _ShapeDefinitions
                .Where(shape => shape.Family == family)
                .Select(shape => shape.MergeSides);

            if (orderByFlagCount)
                return shapes.OrderByDescending(HotFlagCount).ToImmutableArray();

            return shapes.ToImmutableArray();
        }
        #endregion

        #region Utils
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Rectangle CreateRect(int sheetX, int sheetY)
        {
            return new Rectangle(sheetX * 18, sheetY * 18, 16, 16);
        }

        private static Rectangle[] Create3RectsDirX(int leftSheetX, int leftSheetY, int increment = 1)
        {
            return [
                CreateRect(leftSheetX, leftSheetY),
                CreateRect(leftSheetX + (1 * increment), leftSheetY),
                CreateRect(leftSheetX + (2 * increment), leftSheetY)
            ];
        }

        private static Rectangle[] Create3RectsDirY(int topSheetX, int topSheetY, int increment = 1)
        {
            return [
                CreateRect(topSheetX, topSheetY),
                CreateRect(topSheetX, topSheetY + (1 * increment)),
                CreateRect(topSheetX, topSheetY + (2 * increment))
            ];
        }

        private static int HotFlagCount(BlendSideFlags flags)
        {
            var count = 0;
            for (int i = 0; i < 8; i++)
            {
                var flag = (BlendSideFlags)(1 << i);
                if (flag == (flags & flag))
                {
                    count++;
                }
            }
            return count;
        }
        #endregion
    }
}
