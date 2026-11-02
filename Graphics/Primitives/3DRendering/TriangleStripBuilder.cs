#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Graphics.Primitives;

public static class TriangleStripBuilder
{

    /// <summary>
    /// Builds a triangle strip mesh along the specified path with a uniform color, using pooled buffers.
    /// </summary>
    public static PooledPrimitiveMesh BuildStripPooled(
        IReadOnlyList<Vector3> path,
        float width,
        Color color,
        PrimitiveMeshCache? cache,
        Vector3? upHint = null,
        int smoothingSegments = 0,
        StripCapStyle startCap = StripCapStyle.None,
        StripCapStyle endCap = StripCapStyle.None,
        int capSegments = 8,
        StripJoinStyle joinStyle = StripJoinStyle.Perpendicular,
        bool textured = true,
        StripCurveType smoothingCurve = StripCurveType.CatmullRom,
        StripWidthAttenuation widthAttenuation = StripWidthAttenuation.None)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (width <= 0f)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");

        return BuildStripCorePooled(
            path,
            _ => color,
            _ => width,
            upHint ?? Vector3.UnitZ,
            smoothingSegments,
            startCap,
            endCap,
            capSegments,
            joinStyle,
            textured,
            smoothingCurve,
            widthAttenuation,
            cache);
    }

    /// <summary>
    /// Builds a triangle strip mesh along the specified path with varying colors, using pooled buffers.
    /// </summary>
    public static PooledPrimitiveMesh BuildStripPooled(
        IReadOnlyList<Vector3> path,
        float width,
        IReadOnlyList<Color> colors,
        PrimitiveMeshCache? cache,
        Vector3? upHint = null,
        int smoothingSegments = 0,
        StripCapStyle startCap = StripCapStyle.None,
        StripCapStyle endCap = StripCapStyle.None,
        int capSegments = 8,
        StripJoinStyle joinStyle = StripJoinStyle.Perpendicular,
        bool textured = true,
        StripCurveType smoothingCurve = StripCurveType.CatmullRom,
        StripWidthAttenuation widthAttenuation = StripWidthAttenuation.None)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (colors == null)
            throw new ArgumentNullException(nameof(colors));
        if (colors.Count != path.Count)
            throw new ArgumentException("Color count must match path length.", nameof(colors));
        if (width <= 0f)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");

        return BuildStripCorePooled(
            path,
            progress => SampleColor(colors, progress),
            _ => width,
            upHint ?? Vector3.UnitZ,
            smoothingSegments,
            startCap,
            endCap,
            capSegments,
            joinStyle,
            textured,
            smoothingCurve,
            widthAttenuation,
            cache);
    }

    /// <summary>
    /// Builds a triangle strip mesh along the specified path with varying width, using pooled buffers.
    /// </summary>
    public static PooledPrimitiveMesh BuildStripPooled(
        IReadOnlyList<Vector3> path,
        Func<float, float> widthFunc,
        Color color,
        PrimitiveMeshCache? cache,
        Func<float, float>? easing = null,
        Vector3? upHint = null,
        int smoothingSegments = 0,
        StripCapStyle startCap = StripCapStyle.None,
        StripCapStyle endCap = StripCapStyle.None,
        int capSegments = 8,
        StripJoinStyle joinStyle = StripJoinStyle.Perpendicular,
        bool textured = true,
        StripCurveType smoothingCurve = StripCurveType.CatmullRom,
        StripWidthAttenuation widthAttenuation = StripWidthAttenuation.None)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (widthFunc == null)
            throw new ArgumentNullException(nameof(widthFunc));

        return BuildStripCorePooled(
            path,
            _ => color,
            progress => EvaluateWidth(widthFunc, easing, progress),
            upHint ?? Vector3.UnitZ,
            smoothingSegments,
            startCap,
            endCap,
            capSegments,
            joinStyle,
            textured,
            smoothingCurve,
            widthAttenuation,
            cache);
    }

    /// <summary>
    /// Builds a triangle strip mesh along the specified path with varying width and gradient colors, using pooled buffers.
    /// </summary>
    public static PooledPrimitiveMesh BuildStripPooled(
        IReadOnlyList<Vector3> path,
        Func<float, float> widthFunc,
        IReadOnlyList<Color> colors,
        PrimitiveMeshCache? cache,
        Func<float, float>? easing = null,
        Vector3? upHint = null,
        int smoothingSegments = 0,
        StripCapStyle startCap = StripCapStyle.None,
        StripCapStyle endCap = StripCapStyle.None,
        int capSegments = 8,
        StripJoinStyle joinStyle = StripJoinStyle.Perpendicular,
        bool textured = true,
        StripCurveType smoothingCurve = StripCurveType.CatmullRom,
        StripWidthAttenuation widthAttenuation = StripWidthAttenuation.None)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (colors == null)
            throw new ArgumentNullException(nameof(colors));
        if (colors.Count != path.Count)
            throw new ArgumentException("Color count must match path length.", nameof(colors));
        if (widthFunc == null)
            throw new ArgumentNullException(nameof(widthFunc));

        return BuildStripCorePooled(
            path,
            progress => SampleColor(colors, progress),
            progress => EvaluateWidth(widthFunc, easing, progress),
            upHint ?? Vector3.UnitZ,
            smoothingSegments,
            startCap,
            endCap,
            capSegments,
            joinStyle,
            textured,
            smoothingCurve,
            widthAttenuation,
            cache);
    }

    /// <summary>
    /// Builds a triangle strip mesh along the specified path with varying width and color, using pooled buffers.
    /// </summary>
    public static PooledPrimitiveMesh BuildStripPooled(
        IReadOnlyList<Vector3> path,
        Func<float, float> widthFunc,
        Func<float, Color> colorFunc,
        PrimitiveMeshCache? cache,
        Func<float, float>? easing = null,
        Vector3? upHint = null,
        int smoothingSegments = 0,
        StripCapStyle startCap = StripCapStyle.None,
        StripCapStyle endCap = StripCapStyle.None,
        int capSegments = 8,
        StripJoinStyle joinStyle = StripJoinStyle.Perpendicular,
        bool textured = true,
        StripCurveType smoothingCurve = StripCurveType.CatmullRom,
        StripWidthAttenuation widthAttenuation = StripWidthAttenuation.None)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (widthFunc == null)
            throw new ArgumentNullException(nameof(widthFunc));
        if (colorFunc == null)
            throw new ArgumentNullException(nameof(colorFunc));

        return BuildStripCorePooled(
            path,
            progress => colorFunc(MathHelper.Clamp(progress, 0f, 1f)),
            progress => EvaluateWidth(widthFunc, easing, progress),
            upHint ?? Vector3.UnitZ,
            smoothingSegments,
            startCap,
            endCap,
            capSegments,
            joinStyle,
            textured,
            smoothingCurve,
            widthAttenuation,
            cache);
    }

    /// <summary>
    /// Builds a triangle strip mesh along the specified path with a uniform color. <para/>
    /// </summary>
    /// <param name="path">The path along which the strip is built.</param>
    /// <param name="width">The width of the strip.</param>
    /// <param name="color">The color of the strip.</param>
    /// <param name="upHint">An optional hint for the up vector.</param>
    /// <param name="smoothingSegments">The number of segments to use for smoothing.</param>
    /// <param name="startCap">The style of cap at the start of the strip.</param>
    /// <param name="endCap">The style of cap at the end of the strip.</param>
    /// <param name="capSegments">The number of segments to use for each cap.</param>
    /// <param name="joinStyle">The style of join between segments.</param>
    /// <param name="widthAttenuation">The style of width attenuation along the strip.</param>
    /// <param name="smoothingCurve">The curve type for smoothing.</param>
    /// <returns>A triangle strip mesh.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static PrimitiveMesh BuildStrip(IReadOnlyList<Vector3> path, float width, Color color, Vector3? upHint = null, int smoothingSegments = 0, StripCapStyle startCap = StripCapStyle.None, StripCapStyle endCap = StripCapStyle.None, int capSegments = 8, StripJoinStyle joinStyle = StripJoinStyle.Perpendicular, bool textured = true, StripCurveType smoothingCurve = StripCurveType.CatmullRom, StripWidthAttenuation widthAttenuation = StripWidthAttenuation.None)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (width <= 0f)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");

        return BuildStripCore(
            path,
            _ => color,
            _ => width,
            upHint ?? Vector3.UnitZ,
            smoothingSegments,
            startCap,
            endCap,
            capSegments,
            joinStyle,
            textured,
            smoothingCurve,
            widthAttenuation);
    }

    /// <summary>
    /// Builds a triangle strip mesh along the specified path with varying colors.
    /// </summary>
    /// <param name="path">The path along which the strip is built.</param>
    /// <param name="width">The width of the strip.</param>
    /// <param name="colors">The colors of the strip at each point.</param>
    /// <param name="upHint">An optional hint for the up vector.</param>
    /// <param name="smoothingSegments">The number of segments to use for smoothing.</param>
    /// <param name="startCap">The style of cap at the start of the strip.</param>
    /// <param name="endCap">The style of cap at the end of the strip.</param>
    /// <param name="capSegments">The number of segments to use for each cap.</param>
    /// <param name="joinStyle">The style of join between segments.</param>
    /// <param name="widthAttenuation">The style of width attenuation along the strip.</param>
    /// <param name="smoothingCurve">The curve type for smoothing.</param>
    /// <returns>A triangle strip mesh.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static PrimitiveMesh BuildStrip(IReadOnlyList<Vector3> path, float width, IReadOnlyList<Color> colors, Vector3? upHint = null, int smoothingSegments = 0, StripCapStyle startCap = StripCapStyle.None, StripCapStyle endCap = StripCapStyle.None, int capSegments = 8, StripJoinStyle joinStyle = StripJoinStyle.Perpendicular, bool textured = true, StripCurveType smoothingCurve = StripCurveType.CatmullRom, StripWidthAttenuation widthAttenuation = StripWidthAttenuation.None)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (colors == null)
            throw new ArgumentNullException(nameof(colors));
        if (colors.Count != path.Count)
            throw new ArgumentException("Color count must match path length.", nameof(colors));
        if (width <= 0f)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");

        return BuildStripCore(
            path,
            progress => SampleColor(colors, progress),
            _ => width,
            upHint ?? Vector3.UnitZ,
            smoothingSegments,
            startCap,
            endCap,
            capSegments,
            joinStyle,
            textured,
            smoothingCurve,
            widthAttenuation);
    }
    /// <summary>
    /// Builds a triangle strip mesh along the specified path. <para/>
    /// </summary>
    /// <param name="path">The path along which the strip is built.</param>
    /// <param name="widthFunc">A function that maps progress (0 to 1) to the width at that point.</param>
    /// <param name="color">The color of the strip.</param>
    /// <param name="easing">An optional easing function to apply to the progress.</param>
    /// <param name="upHint">An optional hint for the up vector.</param>
    /// <param name="smoothingSegments">The number of segments to use for smoothing.</param>
    /// <param name="startCap">The style of cap at the start of the strip.</param>
    /// <param name="endCap">The style of cap at the end of the strip.</param>
    /// <param name="capSegments">The number of segments to use for each cap.</param>
    /// <param name="joinStyle">The style of join between segments.</param>
    /// <param name="widthAttenuation">The style of width attenuation along the strip.</param>
    /// <param name="smoothingCurve">The curve type for smoothing.</param>
    /// <returns>A triangle strip mesh.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static PrimitiveMesh BuildStrip(
        IReadOnlyList<Vector3> path,
        Func<float, float> widthFunc,
        Color color,
        Func<float, float>? easing = null,
        Vector3? upHint = null,
        int smoothingSegments = 0,
        StripCapStyle startCap = StripCapStyle.None,
        StripCapStyle endCap = StripCapStyle.None,
        int capSegments = 8,
        StripJoinStyle joinStyle = StripJoinStyle.Perpendicular,
        bool textured = true,
        StripCurveType smoothingCurve = StripCurveType.CatmullRom,
        StripWidthAttenuation widthAttenuation = StripWidthAttenuation.None)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (widthFunc == null)
            throw new ArgumentNullException(nameof(widthFunc));

        return BuildStripCore(
            path,
            _ => color,
            progress => EvaluateWidth(widthFunc, easing, progress),
            upHint ?? Vector3.UnitZ,
            smoothingSegments,
            startCap,
            endCap,
            capSegments,
            joinStyle,
            textured,
            smoothingCurve,
            widthAttenuation);
    }
    /// <summary> 
    /// Builds a triangle strip mesh along the specified path with a gradient color. <para/>
    /// </summary>
    /// <param name="path">The path along which the strip is built.</param>
    /// <param name="widthFunc">A function that maps progress (0 to 1) to the width at that point.</param>
    /// <param name="colors">The colors of the strip.</param>
    /// <param name="easing">An optional easing function to apply to the progress.</param>
    /// <param name="upHint">An optional hint for the up vector.</param>
    /// <param name="smoothingSegments">The number of segments to use for smoothing.</param>
    /// <param name="startCap">The style of cap at the start of the strip.</param>
    /// <param name="endCap">The style of cap at the end of the strip.</param>
    /// <param name="capSegments">The number of segments to use for each cap.</param>
    /// <param name="joinStyle">The style of join between segments.</param>
    /// <returns>A triangle strip mesh.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static PrimitiveMesh BuildStrip(
        IReadOnlyList<Vector3> path,
        Func<float, float> widthFunc,
        IReadOnlyList<Color> colors,
        Func<float, float>? easing = null,
        Vector3? upHint = null,
        int smoothingSegments = 0,
        StripCapStyle startCap = StripCapStyle.None,
        StripCapStyle endCap = StripCapStyle.None,
        int capSegments = 8,
        StripJoinStyle joinStyle = StripJoinStyle.Perpendicular,
        bool textured = true,
        StripCurveType smoothingCurve = StripCurveType.CatmullRom,
        StripWidthAttenuation widthAttenuation = StripWidthAttenuation.None)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (colors == null)
            throw new ArgumentNullException(nameof(colors));
        if (colors.Count != path.Count)
            throw new ArgumentException("Color count must match path length.", nameof(colors));
        if (widthFunc == null)
            throw new ArgumentNullException(nameof(widthFunc));

        return BuildStripCore(
            path,
            progress => SampleColor(colors, progress),
            progress => EvaluateWidth(widthFunc, easing, progress),
            upHint ?? Vector3.UnitZ,
            smoothingSegments,
            startCap,
            endCap,
            capSegments,
            joinStyle,
            textured,
            smoothingCurve,
            widthAttenuation);
    }
    /// <summary>
    /// Builds a triangle strip mesh along the specified path with varying width and color. <para/>
    /// </summary>
    /// <param name="path">The path along which the strip is built.</param>
    /// <param name="widthFunc">A function that maps progress (0 to 1) to the width at that point.</param>
    /// <param name="colorFunc">A function that maps progress (0 to 1) to the color at that point.</param>
    /// <param name="easing">An optional easing function to apply to the progress.</param>
    /// <param name="upHint">An optional hint for the up vector.</param>
    /// <param name="smoothingSegments">The number of segments to use for smoothing.</param>
    /// <param name="startCap">The style of cap at the start of the strip.</param>
    /// <param name="endCap">The style of cap at the end of the strip.</param>
    /// <param name="capSegments">The number of segments to use for each cap.</param>
    /// <param name="joinStyle">The style of join between segments.</param>
    /// <param name="widthAttenuation">The style of width attenuation.</param>
    /// <param name="smoothingCurve">The curve type for smoothing.</param>
    /// <returns>A triangle strip mesh.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static PrimitiveMesh BuildStrip(
        IReadOnlyList<Vector3> path,
        Func<float, float> widthFunc,
        Func<float, Color> colorFunc,
        Func<float, float>? easing = null,
        Vector3? upHint = null,
        int smoothingSegments = 0,
        StripCapStyle startCap = StripCapStyle.None,
        StripCapStyle endCap = StripCapStyle.None,
        int capSegments = 8,
        StripJoinStyle joinStyle = StripJoinStyle.Perpendicular,
        bool textured = true,
        StripCurveType smoothingCurve = StripCurveType.CatmullRom,
        StripWidthAttenuation widthAttenuation = StripWidthAttenuation.None)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (widthFunc == null)
            throw new ArgumentNullException(nameof(widthFunc));
        if (colorFunc == null)
            throw new ArgumentNullException(nameof(colorFunc));

        return BuildStripCore(
            path,
            progress => colorFunc(MathHelper.Clamp(progress, 0f, 1f)),
            progress => EvaluateWidth(widthFunc, easing, progress),
            upHint ?? Vector3.UnitZ,
            smoothingSegments,
            startCap,
            endCap,
            capSegments,
            joinStyle,
            textured,
            smoothingCurve,
            widthAttenuation);
    }
    /// <summary>
    /// Core implementation for building a triangle strip mesh.
    /// </summary>
    /// <param name="path">The path along which the strip is built.</param>
    /// <param name="colorResolver">A function that resolves the color at a given progress.</param>
    /// <param name="widthResolver">A function that resolves the width at a given progress.</param>
    /// <param name="up">The up vector for the strip.</param>
    private static PrimitiveMesh BuildStripCore(
        IReadOnlyList<Vector3> path,
        Func<float, Color> colorResolver,
        Func<float, float> widthResolver,
        Vector3 up,
        int smoothingSegments,
        StripCapStyle startCap,
        StripCapStyle endCap,
        int capSegments,
        StripJoinStyle joinStyle,
        bool textured,
        StripCurveType smoothingCurve,
        StripWidthAttenuation widthAttenuation)
    {
        if (path.Count < 2)
            throw new ArgumentException("At least two points are required.", nameof(path));
        if (colorResolver == null)
            throw new ArgumentNullException(nameof(colorResolver));
        if (widthResolver == null)
            throw new ArgumentNullException(nameof(widthResolver));

        var workingPath = RemoveDegenerates(MaybeSmoothPath(path, smoothingSegments, smoothingCurve));
        var progress = ComputeProgress(workingPath, out _);

        var texturedVertices = textured ? new VertexPositionColorTexture[workingPath.Count * 2] : null;
        var colorVertices = textured ? null : new VertexPositionColor[workingPath.Count * 2];

        if ((textured ? texturedVertices!.Length : colorVertices!.Length) > short.MaxValue)
            throw new InvalidOperationException("Strip produced more vertices than supported by the index buffer.");

        var tangents = new Vector3[workingPath.Count];
        var rights = new Vector3[workingPath.Count];
        var centers = new Vector3[workingPath.Count];
        var sectionColors = new Color[workingPath.Count];
        var halfWidths = new float[workingPath.Count];

        var upNormalized = up.LengthSquared() < 1e-6f ? Vector3.UnitZ : Vector3.Normalize(up);

        int segmentCount = workingPath.Count - 1;
        ComputeSegmentFrames(workingPath, upNormalized, segmentCount, out var segmentDirs, out var segmentRights);

        Vector3 lastTangent = segmentDirs[0];

        for (int i = 0; i < workingPath.Count; i++)
        {
            var position = workingPath[i];
            float t = progress[i];
            float baseWidth = Math.Max(0f, widthResolver(MathHelper.Clamp(t, 0f, 1f)));

            Vector3 prevDir = segmentDirs[Math.Max(i - 1, 0)];
            Vector3 nextDir = segmentDirs[Math.Min(i, segmentCount - 1)];

            Vector3 tangent;
            if (i == 0)
                tangent = nextDir;
            else if (i == segmentCount)
                tangent = prevDir;
            else
            {
                tangent = prevDir + nextDir;
                if (tangent.LengthSquared() < 1e-6f)
                    tangent = nextDir;
                else
                    tangent.Normalize();
            }

            float width = baseWidth;


            if (i > 0 && widthAttenuation == StripWidthAttenuation.ContinuitySquared)
            {
                float continuity = MathHelper.Clamp((Vector3.Dot(lastTangent, tangent) + 1f) * 0.5f, 0f, 1f);
                width *= continuity * continuity;
            }

            float halfWidth = width * 0.5f;

            Vector3 prevRight = segmentRights[Math.Max(i - 1, 0)];
            Vector3 nextRight = segmentRights[Math.Min(i, segmentCount - 1)];

            ComputeJoinOffsets(prevRight, nextRight, i == 0, i == segmentCount, halfWidth, joinStyle, out Vector3 rightOffset, out Vector3 leftOffset);

            Vector3 leftPos = position + leftOffset;
            Vector3 rightPos = position + rightOffset;
            Vector3 chord = rightPos - leftPos;
            float chordLength = chord.Length();
            Vector3 lateralDir = chordLength > 1e-6f ? chord / chordLength : (nextRight.LengthSquared() > 1e-6f ? nextRight : Vector3.UnitX);
            Vector3 crossCenter = (leftPos + rightPos) * 0.5f;
            float effectiveHalfWidth = chordLength * 0.5f;

            float uCoord = progress[i];
            var color = colorResolver(t);

            if (textured)
            {
                texturedVertices![i * 2] = CreateEdgeVertex(leftPos, color, uCoord, isLeft: true);
                texturedVertices[i * 2 + 1] = CreateEdgeVertex(rightPos, color, uCoord, isLeft: false);
            }
            else
            {
                colorVertices![i * 2] = new VertexPositionColor(leftPos, color);
                colorVertices[i * 2 + 1] = new VertexPositionColor(rightPos, color);
            }

            Vector3 rightForCap = lateralDir.LengthSquared() > 1e-6f ? lateralDir : Vector3.UnitX;

            tangents[i] = tangent;
            rights[i] = Vector3.Normalize(rightForCap);
            centers[i] = crossCenter;
            sectionColors[i] = color;
            halfWidths[i] = effectiveHalfWidth;
            lastTangent = tangent;
        }

        bool needCaps = startCap != StripCapStyle.None || endCap != StripCapStyle.None;
        if (!needCaps)
        {
            var stripIndices = new short[textured ? texturedVertices!.Length : colorVertices!.Length];
            PrimitiveSimd.FillSequentialIndices(stripIndices.AsSpan());

            return textured
                ? new PrimitiveMesh(texturedVertices!, stripIndices, PrimitiveType.TriangleStrip)
                : new PrimitiveMesh(colorVertices!, stripIndices, PrimitiveType.TriangleStrip);
        }

        if (textured)
        {
            var vertexList = new List<VertexPositionColorTexture>(texturedVertices!);
            var indexList = new List<short>();
            AppendStripAsTriangles(indexList, vertexList.Count);

            int startLeftIndex = 0;
            int startRightIndex = 1;
            int endLeftIndex = texturedVertices!.Length - 2;
            int endRightIndex = texturedVertices!.Length - 1;
            int capSteps = Math.Max(2, capSegments);
            float startU = progress[0];
            float endU = progress[^1];

            if (startCap == StripCapStyle.Triangle)
                AddTriangleCap(vertexList, indexList, centers[0], tangents[0], rights[0], halfWidths[0], sectionColors[0], startU, startLeftIndex, startRightIndex, true);
            else if (startCap == StripCapStyle.HalfCircle)
                AddHalfCircleCap(vertexList, indexList, centers[0], tangents[0], rights[0], halfWidths[0], sectionColors[0], startU, startLeftIndex, startRightIndex, true, capSteps);

            if (endCap == StripCapStyle.Triangle)
                AddTriangleCap(vertexList, indexList, centers[^1], tangents[^1], rights[^1], halfWidths[^1], sectionColors[^1], endU, endLeftIndex, endRightIndex, false);
            else if (endCap == StripCapStyle.HalfCircle)
                AddHalfCircleCap(vertexList, indexList, centers[^1], tangents[^1], rights[^1], halfWidths[^1], sectionColors[^1], endU, endLeftIndex, endRightIndex, false, capSteps);

            return new PrimitiveMesh(vertexList.ToArray(), indexList.ToArray(), PrimitiveType.TriangleList);
        }
        else
        {
            var vertexList = new List<VertexPositionColor>(colorVertices!);
            var indexList = new List<short>();
            AppendStripAsTriangles(indexList, vertexList.Count);

            int startLeftIndex = 0;
            int startRightIndex = 1;
            int endLeftIndex = colorVertices!.Length - 2;
            int endRightIndex = colorVertices!.Length - 1;
            int capSteps = Math.Max(2, capSegments);

            if (startCap == StripCapStyle.Triangle)
                AddTriangleCap(vertexList, indexList, centers[0], tangents[0], rights[0], halfWidths[0], sectionColors[0], startLeftIndex, startRightIndex, true);
            else if (startCap == StripCapStyle.HalfCircle)
                AddHalfCircleCap(vertexList, indexList, centers[0], tangents[0], rights[0], halfWidths[0], sectionColors[0], startLeftIndex, startRightIndex, true, capSteps);

            if (endCap == StripCapStyle.Triangle)
                AddTriangleCap(vertexList, indexList, centers[^1], tangents[^1], rights[^1], halfWidths[^1], sectionColors[^1], endLeftIndex, endRightIndex, false);
            else if (endCap == StripCapStyle.HalfCircle)
                AddHalfCircleCap(vertexList, indexList, centers[^1], tangents[^1], rights[^1], halfWidths[^1], sectionColors[^1], endLeftIndex, endRightIndex, false, capSteps);

            return new PrimitiveMesh(vertexList.ToArray(), indexList.ToArray(), PrimitiveType.TriangleList);
        }
    }

    private static PooledPrimitiveMesh BuildStripCorePooled(
        IReadOnlyList<Vector3> path,
        Func<float, Color> colorResolver,
        Func<float, float> widthResolver,
        Vector3 up,
        int smoothingSegments,
        StripCapStyle startCap,
        StripCapStyle endCap,
        int capSegments,
        StripJoinStyle joinStyle,
        bool textured,
        StripCurveType smoothingCurve,
        StripWidthAttenuation widthAttenuation,
        PrimitiveMeshCache? cache)
    {
        if (path.Count < 2)
            throw new ArgumentException("At least two points are required.", nameof(path));
        if (colorResolver == null)
            throw new ArgumentNullException(nameof(colorResolver));
        if (widthResolver == null)
            throw new ArgumentNullException(nameof(widthResolver));

        cache ??= PrimitiveMeshCache.Shared;

        var workingPath = RemoveDegenerates(MaybeSmoothPath(path, smoothingSegments, smoothingCurve));
        var progress = ComputeProgress(workingPath, out _);

        int vertexCount = workingPath.Count * 2;
        if (vertexCount > short.MaxValue)
            throw new InvalidOperationException("Strip produced more vertices than supported by the index buffer.");

        bool needCaps = startCap != StripCapStyle.None || endCap != StripCapStyle.None;

        VertexPositionColorTexture[]? texturedVertices = null;
        VertexPositionColor[]? colorVertices = null;
        PooledPrimitiveMesh pooledLease = default;
        bool hasLease = false;
        List<VertexPositionColorTexture>? texturedList = null;
        List<VertexPositionColor>? colorList = null;

        var tangents = needCaps ? new Vector3[workingPath.Count] : null;
        var rights = needCaps ? new Vector3[workingPath.Count] : null;
        var centers = needCaps ? new Vector3[workingPath.Count] : null;
        var sectionColors = needCaps ? new Color[workingPath.Count] : null;
        var halfWidths = needCaps ? new float[workingPath.Count] : null;

        if (needCaps)
        {
            if (textured)
                texturedList = new List<VertexPositionColorTexture>(vertexCount);
            else
                colorList = new List<VertexPositionColor>(vertexCount);
        }
        else
        {
            if (textured)
            {
                pooledLease = cache.RentTextured(vertexCount, vertexCount, PrimitiveType.TriangleStrip);
                texturedVertices = pooledLease.TexturedVertices;
            }
            else
            {
                pooledLease = cache.RentColored(vertexCount, vertexCount, PrimitiveType.TriangleStrip);
                colorVertices = pooledLease.ColorVertices;
            }

            hasLease = true;
        }

        var upNormalized = up.LengthSquared() < 1e-6f ? Vector3.UnitZ : Vector3.Normalize(up);

        int segmentCount = workingPath.Count - 1;
        ComputeSegmentFrames(workingPath, upNormalized, segmentCount, out var segmentDirs, out var segmentRights);

        Vector3 lastTangent = segmentDirs[0];

        for (int i = 0; i < workingPath.Count; i++)
        {
            var position = workingPath[i];
            float t = progress[i];
            float baseWidth = Math.Max(0f, widthResolver(MathHelper.Clamp(t, 0f, 1f)));

            Vector3 prevDir = segmentDirs[Math.Max(i - 1, 0)];
            Vector3 nextDir = segmentDirs[Math.Min(i, segmentCount - 1)];

            Vector3 tangent;
            if (i == 0)
                tangent = nextDir;
            else if (i == segmentCount)
                tangent = prevDir;
            else
            {
                tangent = prevDir + nextDir;
                if (tangent.LengthSquared() < 1e-6f)
                    tangent = nextDir;
                else
                    tangent.Normalize();
            }

            float width = baseWidth;

            if (i > 0 && widthAttenuation == StripWidthAttenuation.ContinuitySquared)
            {
                float continuity = MathHelper.Clamp((Vector3.Dot(lastTangent, tangent) + 1f) * 0.5f, 0f, 1f);
                width *= continuity * continuity;
            }

            float halfWidth = width * 0.5f;

            Vector3 prevRight = segmentRights[Math.Max(i - 1, 0)];
            Vector3 nextRight = segmentRights[Math.Min(i, segmentCount - 1)];

            ComputeJoinOffsets(prevRight, nextRight, i == 0, i == segmentCount, halfWidth, joinStyle, out Vector3 rightOffset, out Vector3 leftOffset);

            Vector3 leftPos = position + leftOffset;
            Vector3 rightPos = position + rightOffset;
            Vector3 chord = rightPos - leftPos;
            float chordLength = chord.Length();
            Vector3 lateralDir = chordLength > 1e-6f ? chord / chordLength : (nextRight.LengthSquared() > 1e-6f ? nextRight : Vector3.UnitX);
            Vector3 crossCenter = (leftPos + rightPos) * 0.5f;
            float effectiveHalfWidth = chordLength * 0.5f;

            float uCoord = progress[i];
            var color = colorResolver(t);

            if (textured)
            {
                if (needCaps)
                {
                    texturedList!.Add(CreateEdgeVertex(leftPos, color, uCoord, isLeft: true));
                    texturedList.Add(CreateEdgeVertex(rightPos, color, uCoord, isLeft: false));
                }
                else
                {
                    texturedVertices![i * 2] = CreateEdgeVertex(leftPos, color, uCoord, isLeft: true);
                    texturedVertices[i * 2 + 1] = CreateEdgeVertex(rightPos, color, uCoord, isLeft: false);
                }
            }
            else
            {
                if (needCaps)
                {
                    colorList!.Add(new VertexPositionColor(leftPos, color));
                    colorList.Add(new VertexPositionColor(rightPos, color));
                }
                else
                {
                    colorVertices![i * 2] = new VertexPositionColor(leftPos, color);
                    colorVertices[i * 2 + 1] = new VertexPositionColor(rightPos, color);
                }
            }

            Vector3 rightForCap = lateralDir.LengthSquared() > 1e-6f ? lateralDir : Vector3.UnitX;

            if (needCaps)
            {
                tangents![i] = tangent;
                rights![i] = Vector3.Normalize(rightForCap);
                centers![i] = crossCenter;
                sectionColors![i] = color;
                halfWidths![i] = effectiveHalfWidth;
            }
            lastTangent = tangent;
        }
        if (!needCaps)
        {
            if (!hasLease)
                throw new InvalidOperationException("Pooled mesh lease was not initialized.");

            PrimitiveSimd.FillSequentialIndices(pooledLease.Indices.AsSpan(0, vertexCount));
            return pooledLease;
        }

        if (textured)
        {
            var vertexList = texturedList!;
            var indexList = new List<short>();
            AppendStripAsTriangles(indexList, vertexList.Count);

            int startLeftIndex = 0;
            int startRightIndex = 1;
            int endLeftIndex = texturedVertices!.Length - 2;
            int endRightIndex = texturedVertices!.Length - 1;
            int capSteps = Math.Max(2, capSegments);
            float startU = progress[0];
            float endU = progress[^1];

            if (startCap == StripCapStyle.Triangle)
                AddTriangleCap(vertexList, indexList, centers![0], tangents![0], rights![0], halfWidths![0], sectionColors![0], startU, startLeftIndex, startRightIndex, true);
            else if (startCap == StripCapStyle.HalfCircle)
                AddHalfCircleCap(vertexList, indexList, centers![0], tangents![0], rights![0], halfWidths![0], sectionColors![0], startU, startLeftIndex, startRightIndex, true, capSteps);

            if (endCap == StripCapStyle.Triangle)
                AddTriangleCap(vertexList, indexList, centers![^1], tangents![^1], rights![^1], halfWidths![^1], sectionColors![^1], endU, endLeftIndex, endRightIndex, false);
            else if (endCap == StripCapStyle.HalfCircle)
                AddHalfCircleCap(vertexList, indexList, centers![^1], tangents![^1], rights![^1], halfWidths![^1], sectionColors![^1], endU, endLeftIndex, endRightIndex, false, capSteps);

            var lease = cache.RentTextured(vertexList.Count, indexList.Count, PrimitiveType.TriangleList);
            vertexList.CopyTo(0, lease.TexturedVertices, 0, vertexList.Count);
            indexList.CopyTo(0, lease.Indices, 0, indexList.Count);
            return lease;
        }
        else
        {
            var vertexList = colorList!;
            var indexList = new List<short>();
            AppendStripAsTriangles(indexList, vertexList.Count);

            int startLeftIndex = 0;
            int startRightIndex = 1;
            int endLeftIndex = colorVertices!.Length - 2;
            int endRightIndex = colorVertices!.Length - 1;
            int capSteps = Math.Max(2, capSegments);

            if (startCap == StripCapStyle.Triangle)
                AddTriangleCap(vertexList, indexList, centers![0], tangents![0], rights![0], halfWidths![0], sectionColors![0], startLeftIndex, startRightIndex, true);
            else if (startCap == StripCapStyle.HalfCircle)
                AddHalfCircleCap(vertexList, indexList, centers![0], tangents![0], rights![0], halfWidths![0], sectionColors![0], startLeftIndex, startRightIndex, true, capSteps);

            if (endCap == StripCapStyle.Triangle)
                AddTriangleCap(vertexList, indexList, centers![^1], tangents![^1], rights![^1], halfWidths![^1], sectionColors![^1], endLeftIndex, endRightIndex, false);
            else if (endCap == StripCapStyle.HalfCircle)
                AddHalfCircleCap(vertexList, indexList, centers![^1], tangents![^1], rights![^1], halfWidths![^1], sectionColors![^1], endLeftIndex, endRightIndex, false, capSteps);

            var lease = cache.RentColored(vertexList.Count, indexList.Count, PrimitiveType.TriangleList);
            vertexList.CopyTo(0, lease.ColorVertices, 0, vertexList.Count);
            indexList.CopyTo(0, lease.Indices, 0, indexList.Count);
            return lease;
        }
    }

    private static void ComputeSegmentFrames(IReadOnlyList<Vector3> workingPath, Vector3 upNormalized, int segmentCount, out Vector3[] segmentDirs, out Vector3[] segmentRights)
    {
        segmentDirs = new Vector3[segmentCount];
        segmentRights = new Vector3[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 dir = workingPath[i + 1] - workingPath[i];
            if (dir.LengthSquared() < 1e-6f)
                dir = i > 0 ? segmentDirs[i - 1] : Vector3.UnitY;
            dir.Normalize();

            Vector3 right = Vector3.Cross(upNormalized, dir);
            if (right.LengthSquared() < 1e-6f)
                right = FindPerpendicular(dir);
            else
                right.Normalize();

            segmentDirs[i] = dir;
            segmentRights[i] = right;
        }
    }

    private static void ComputeJoinOffsets(Vector3 prevRight, Vector3 nextRight, bool isStart, bool isEnd, float halfWidth, StripJoinStyle joinStyle, out Vector3 rightOffset, out Vector3 leftOffset)
    {
        if (joinStyle == StripJoinStyle.Miter)
        {
            rightOffset = ComputeMiterOffset(prevRight, nextRight, isStart, isEnd, halfWidth);
            leftOffset = ComputeMiterOffset(-prevRight, -nextRight, isStart, isEnd, halfWidth);
        }
        else
        {
            Vector3 joinNormal;
            if (isStart)
                joinNormal = nextRight;
            else if (isEnd)
                joinNormal = prevRight;
            else
            {
                joinNormal = prevRight + nextRight;
                if (joinNormal.LengthSquared() < 1e-6f)
                    joinNormal = nextRight;
            }

            if (joinNormal.LengthSquared() < 1e-6f)
                joinNormal = Vector3.UnitY;

            joinNormal.Normalize();
            rightOffset = joinNormal * halfWidth;
            leftOffset = -joinNormal * halfWidth;
        }
    }

    private static IReadOnlyList<Vector3> MaybeSmoothPath(IReadOnlyList<Vector3> path, int subdivisions, StripCurveType curveType)
    {
        if (subdivisions <= 0 || path.Count < 2)
            return path;

        var result = new List<Vector3>((path.Count - 1) * (subdivisions + 1) + 1);

        for (int i = 0; i < path.Count - 1; i++)
        {
            var p0 = path[Math.Max(i - 1, 0)];
            var p1 = path[i];
            var p2 = path[i + 1];
            var p3 = path[Math.Min(i + 2, path.Count - 1)];

            if (i == 0)
                result.Add(p1);

            for (int s = 1; s <= subdivisions; s++)
            {
                float t = s / (float)(subdivisions + 1);
                var point = EvaluateCurve(curveType, p0, p1, p2, p3, t, path.Count);
                result.Add(point);
            }

            result.Add(p2);
        }

        return result;
    }

    private static Vector3 EvaluateCurve(StripCurveType curveType, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, int count)
    {
        switch (curveType)
        {
            case StripCurveType.Linear:
                return Vector3.Lerp(p1, p2, t);

            case StripCurveType.CatmullRom when count >= 4:
                return Vector3.CatmullRom(p0, p1, p2, p3, t);

            case StripCurveType.CubicBezier when count >= 4:
                {
                    Vector3 c1 = p1 + (p2 - p0) / 6f;
                    Vector3 c2 = p2 - (p3 - p1) / 6f;
                    float inv = 1f - t;
                    return inv * inv * inv * p1
                         + 3f * inv * inv * t * c1
                         + 3f * inv * t * t * c2
                         + t * t * t * p2;
                }

            case StripCurveType.Hermite when count >= 4:
                {
                    Vector3 tan1 = (p2 - p0) * 0.5f;
                    Vector3 tan2 = (p3 - p1) * 0.5f;
                    return Vector3.Hermite(p1, tan1, p2, tan2, t);
                }

            default:
                return Vector3.Lerp(p1, p2, t);
        }
    }

    private static Color SampleColor(IReadOnlyList<Color> colors, float progress)
    {
        if (colors.Count == 1)
            return colors[0];

        float scaled = MathHelper.Clamp(progress, 0f, 1f) * (colors.Count - 1);
        int index = Math.Min(colors.Count - 2, (int)MathF.Floor(scaled));
        float localT = scaled - index;
        return Color.Lerp(colors[index], colors[index + 1], localT);
    }

    private static float[] ComputeProgress(IReadOnlyList<Vector3> path, out float totalLength)
    {
        var progress = new float[path.Count];
        float cumulative = 0f;

        for (int i = 1; i < path.Count; i++)
        {
            cumulative += Vector3.Distance(path[i - 1], path[i]);
            progress[i] = cumulative;
        }

        totalLength = cumulative;

        if (cumulative > 1e-6f)
        {
            float inv = 1f / cumulative;
            for (int i = 1; i < progress.Length; i++)
                progress[i] *= inv;
        }

        return progress;
    }

    private static float EvaluateWidth(Func<float, float> widthFunc, Func<float, float>? easing, float progress)
    {
        float eased = easing?.Invoke(MathHelper.Clamp(progress, 0f, 1f)) ?? MathHelper.Clamp(progress, 0f, 1f);
        return Math.Max(0f, widthFunc(MathHelper.Clamp(eased, 0f, 1f)));
    }

    private static Vector3 FindPerpendicular(Vector3 vector)
    {
        Vector3 axis = Math.Abs(vector.Y) < Math.Abs(vector.X)
            ? Vector3.UnitY
            : Vector3.UnitX;

        var perpendicular = Vector3.Cross(vector, axis);
        if (perpendicular.LengthSquared() < 1e-6f)
        {
            perpendicular = Vector3.Cross(vector, Vector3.UnitZ);
        }

        perpendicular.Normalize();
        return perpendicular;
    }

    // maybe find a better name for this?
    private static IReadOnlyList<Vector3> RemoveDegenerates(IReadOnlyList<Vector3> path)
    {
        if (path.Count < 2)
            return path;

        var result = new List<Vector3>(path.Count);
        var last = path[0];
        result.Add(last);

        for (int i = 1; i < path.Count; i++)
        {
            if (Vector3.DistanceSquared(last, path[i]) <= 1e-6f)
                continue;

            last = path[i];
            result.Add(last);
        }

        if (result.Count == 1)
            result.Add(path[path.Count - 1]);

        return result;
    }

    private static void AppendStripAsTriangles(List<short> indices, int vertexCount)
    {
        for (int i = 0; i < vertexCount - 2; i++)
        {
            if ((i & 1) == 0)
            {
                indices.Add((short)i);
                indices.Add((short)(i + 1));
                indices.Add((short)(i + 2));
            }
            else
            {
                indices.Add((short)(i + 1));
                indices.Add((short)i);
                indices.Add((short)(i + 2));
            }
        }
    }

    private static void AddTriangleCap(
        List<VertexPositionColorTexture> vertices,
        List<short> indices,
        Vector3 center,
        Vector3 tangent,
        Vector3 rightDir,
        float halfWidth,
        Color color,
        float uCoord,
        int leftIndex,
        int rightIndex,
        bool isStart)
    {
        if (halfWidth <= 1e-6f)
            return;

        var outward = isStart ? -tangent : tangent;
        short apexIndex = AddVertex(vertices, CreateCapVertex(center + outward * halfWidth, color, uCoord, center, rightDir, halfWidth));

        if (isStart)
        {
            indices.Add(apexIndex);
            indices.Add((short)rightIndex);
            indices.Add((short)leftIndex);
        }
        else
        {
            indices.Add(apexIndex);
            indices.Add((short)leftIndex);
            indices.Add((short)rightIndex);
        }
    }

    private static void AddTriangleCap(
        List<VertexPositionColor> vertices,
        List<short> indices,
        Vector3 center,
        Vector3 tangent,
        Vector3 rightDir, // todo: remove
        float halfWidth,
        Color color,
        int leftIndex,
        int rightIndex,
        bool isStart)
    {
        if (halfWidth <= 1e-6f)
            return;

        var outward = isStart ? -tangent : tangent;
        short apexIndex = AddVertex(vertices, new VertexPositionColor(center + outward * halfWidth, color));

        if (isStart)
        {
            indices.Add(apexIndex);
            indices.Add((short)rightIndex);
            indices.Add((short)leftIndex);
        }
        else
        {
            indices.Add(apexIndex);
            indices.Add((short)leftIndex);
            indices.Add((short)rightIndex);
        }
    }

    private static void AddHalfCircleCap(
        List<VertexPositionColorTexture> vertices,
        List<short> indices,
        Vector3 center,
        Vector3 tangent,
        Vector3 rightDir,
        float halfWidth,
        Color color,
        float uCoord,
        int leftIndex,
        int rightIndex,
        bool isStart,
        int segments)
    {
        if (halfWidth <= 1e-6f)
            return;

        var outward = isStart ? -tangent : tangent;
        short centerIndex = AddVertex(vertices, CreateCapVertex(center, color, uCoord, center, rightDir, halfWidth));

        var arcVertices = new List<short>
        {
            isStart ? (short)rightIndex : (short)leftIndex
        };

        float step = MathF.PI / segments;
        for (int i = 1; i < segments; i++)
        {
            float angle = isStart ? step * i : MathF.PI - step * i;
            Vector3 offset = rightDir * MathF.Cos(angle) + outward * MathF.Sin(angle);
            short arcIndex = AddVertex(vertices, CreateCapVertex(center + offset * halfWidth, color, uCoord, center, rightDir, halfWidth));
            arcVertices.Add(arcIndex);
        }

        arcVertices.Add(isStart ? (short)leftIndex : (short)rightIndex);

        for (int i = 0; i < arcVertices.Count - 1; i++)
        {
            if (isStart)
            {
                indices.Add(centerIndex);
                indices.Add(arcVertices[i]);
                indices.Add(arcVertices[i + 1]);
            }
            else
            {
                indices.Add(centerIndex);
                indices.Add(arcVertices[i + 1]);
                indices.Add(arcVertices[i]);
            }
        }
    }

    private static void AddHalfCircleCap(
        List<VertexPositionColor> vertices,
        List<short> indices,
        Vector3 center,
        Vector3 tangent,
        Vector3 rightDir,
        float halfWidth,
        Color color,
        int leftIndex,
        int rightIndex,
        bool isStart,
        int segments)
    {
        if (halfWidth <= 1e-6f)
            return;

        var outward = isStart ? -tangent : tangent;
        short centerIndex = AddVertex(vertices, new VertexPositionColor(center, color));

        var arcVertices = new List<short>
        {
            isStart ? (short)rightIndex : (short)leftIndex
        };

        float step = MathF.PI / segments;
        for (int i = 1; i < segments; i++)
        {
            float angle = isStart ? step * i : MathF.PI - step * i;
            Vector3 offset = rightDir * MathF.Cos(angle) + outward * MathF.Sin(angle);
            short arcIndex = AddVertex(vertices, new VertexPositionColor(center + offset * halfWidth, color));
            arcVertices.Add(arcIndex);
        }

        arcVertices.Add(isStart ? (short)leftIndex : (short)rightIndex);

        for (int i = 0; i < arcVertices.Count - 1; i++)
        {
            if (isStart)
            {
                indices.Add(centerIndex);
                indices.Add(arcVertices[i]);
                indices.Add(arcVertices[i + 1]);
            }
            else
            {
                indices.Add(centerIndex);
                indices.Add(arcVertices[i + 1]);
                indices.Add(arcVertices[i]);
            }
        }
    }

    private static short AddVertex(List<VertexPositionColorTexture> vertices, VertexPositionColorTexture vertex)
    {
        if (vertices.Count >= short.MaxValue)
            throw new InvalidOperationException("Primitive mesh exceeded 16-bit index capacity.");
        vertices.Add(vertex);
        return (short)(vertices.Count - 1);
    }

    private static short AddVertex(List<VertexPositionColor> vertices, VertexPositionColor vertex)
    {
        if (vertices.Count >= short.MaxValue)
            throw new InvalidOperationException("Primitive mesh exceeded 16-bit index capacity.");
        vertices.Add(vertex);
        return (short)(vertices.Count - 1);
    }

    private static VertexPositionColorTexture CreateEdgeVertex(Vector3 position, Color color, float u, bool isLeft)
    {
        return new VertexPositionColorTexture(
            position,
            color,
            new Vector2(MathHelper.Clamp(u, 0f, 1f), isLeft ? 0f : 1f));
    }

    private static VertexPositionColorTexture CreateCapVertex(Vector3 position, Color color, float u, Vector3 center, Vector3 rightDir, float halfWidth)
    {
        Vector3 right = rightDir.LengthSquared() > 1e-6f ? Vector3.Normalize(rightDir) : Vector3.UnitX;
        float width = Math.Max(halfWidth, 1e-6f);
        float lateral = MathHelper.Clamp(Vector3.Dot(position - center, right) / width, -1f, 1f);
        float v = 0.5f + 0.5f * lateral;
        return new VertexPositionColorTexture(
            position,
            color,
            new Vector2(MathHelper.Clamp(u, 0f, 1f), MathHelper.Clamp(v, 0f, 1f)));
    }

    private static Vector3 ComputeMiterOffset(Vector3 prevNormal, Vector3 nextNormal, bool isStart, bool isEnd, float halfWidth)
    {
        if (halfWidth <= 1e-6f)
            return Vector3.Zero;

        if (isStart)
            return nextNormal * halfWidth;
        if (isEnd)
            return prevNormal * halfWidth;

        float prevLenSq = prevNormal.LengthSquared();
        float nextLenSq = nextNormal.LengthSquared();
        if (prevLenSq < 1e-6f || nextLenSq < 1e-6f)
            return (nextLenSq >= prevLenSq ? nextNormal : prevNormal) * halfWidth;

        Vector3 sum = prevNormal + nextNormal;
        float sumLenSq = sum.LengthSquared();
        if (sumLenSq < 1e-4f)
            return nextNormal * halfWidth;

        Vector3 miter = sum / MathF.Sqrt(sumLenSq);
        float denom = Vector3.Dot(miter, nextNormal);
        float absDenom = MathF.Abs(denom);
        if (absDenom <= 1e-3f)
            return nextNormal * halfWidth;

        float scale = halfWidth / denom;
        const float MiterLimit = 4f;
        float maxScale = halfWidth * MiterLimit;
        if (MathF.Abs(scale) > maxScale)
            scale = MathF.Sign(scale) * maxScale;

        return miter * scale;
    }
}
