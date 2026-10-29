#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace CalamityMod.Graphics.Primitives;

public static class SanePrimitiveRenderer
{
    private static GraphicsDevice _graphicsDevice => Main.graphics.GraphicsDevice;
    private static BasicEffect? _effect;

    public readonly struct ShaderScope : IDisposable
    {
        private readonly GraphicsDevice _device;
        private readonly Effect _effect;
        private readonly bool _useBasicEffect;
        private readonly Matrix _world;
        private readonly Matrix _view;
        private readonly Matrix _projection;
        private readonly string? _transformMatrixParam;
        private readonly RasterizerState _previousRasterizer;
        private readonly DepthStencilState _previousDepth;
        private readonly BlendState _previousBlend;
        private readonly SamplerState? _previousSampler;

        internal ShaderScope(
            GraphicsDevice device,
            Effect effect,
            bool useBasicEffect,
            in Matrix world,
            in Matrix view,
            in Matrix projection,
            string? transformMatrixParam,
            RasterizerState? rasterizerState,
            DepthStencilState? depthState,
            BlendState? blendState,
            SamplerState? samplerState)
        {
            _device = device;
            _effect = effect;
            _useBasicEffect = useBasicEffect;
            _world = world;
            _view = view;
            _projection = projection;
            _transformMatrixParam = transformMatrixParam;

            _previousRasterizer = device.RasterizerState;
            _previousDepth = device.DepthStencilState;
            _previousBlend = device.BlendState;
            _previousSampler = device.SamplerStates[0];

            device.RasterizerState = rasterizerState ?? RasterizerState.CullNone;
            device.DepthStencilState = depthState ?? DepthStencilState.Default;
            device.BlendState = blendState ?? BlendState.AlphaBlend;
            device.SamplerStates[0] = samplerState ?? SamplerState.PointWrap;

            if (_useBasicEffect)
            {
                var basic = (BasicEffect)_effect;
                basic.World = world;
                basic.View = view;
                basic.Projection = projection;
            }
            else if (transformMatrixParam != null)
            {
                var param = _effect.Parameters[transformMatrixParam];
                if (param != null)
                    param.SetValue(world * view * projection);
            }
        }

        public void Draw(in PrimitiveMesh mesh)
        {
            if (!mesh.IsValid)
                return;

            if (_useBasicEffect)
            {
                var basic = (BasicEffect)_effect;
                bool textured = mesh.UsesTexture;
                basic.TextureEnabled = textured;
                basic.Texture = textured ? TextureAssets.MagicPixel.Value : null;
            }

            int primitiveCount = GetPrimitiveCount(mesh.PrimitiveType, mesh.Indices.Length);

            DrawMeshCore(_device, _effect, mesh.AsView(), primitiveCount);
        }

        public void Draw(in PrimitiveMeshView mesh)
        {
            if (!mesh.IsValid)
                return;

            if (_useBasicEffect)
            {
                var basic = (BasicEffect)_effect;
                bool textured = mesh.UsesTexture;
                basic.TextureEnabled = textured;
                basic.Texture = textured ? TextureAssets.MagicPixel.Value : null;
            }

            int primitiveCount = GetPrimitiveCount(mesh.PrimitiveType, mesh.IndexCount);

            DrawMeshCore(_device, _effect, mesh, primitiveCount);
        }

        public void Dispose()
        {
            _device.RasterizerState = _previousRasterizer;
            _device.DepthStencilState = _previousDepth;
            _device.BlendState = _previousBlend;
            if (_previousSampler != null)
                _device.SamplerStates[0] = _previousSampler;
        }
    }

    public static bool IsReady =>
        _graphicsDevice != null &&
        !_graphicsDevice.IsDisposed &&
        _effect != null &&
        !_effect.IsDisposed;

    public static void Initialize()
    {
        Main.RunOnMainThread(() =>
        {
            if (_graphicsDevice == null)
                throw new ArgumentNullException(nameof(_graphicsDevice));
            
            _effect?.Dispose();
            _effect = new BasicEffect(_graphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true,
                LightingEnabled = false,
                FogEnabled = false,
                Texture = TextureAssets.Logo.Value
            };
        });
    }

    public static void Dispose()
    {
        Main.RunOnMainThread(() =>
        _effect?.Dispose()
        );
    }

    /// <summary>
    /// Draws the specified mesh using the provided world, view, and projection matrices.
    /// </summary>
    /// <param name="world">World matrix.</param>
    /// <param name="view">View matrix.</param>
    /// <param name="projection">Projection matrix.</param>
    /// <param name="mesh">The mesh to draw.</param>
    /// <param name="rasterizerState">The rasterizer state to use. If null, defaults to <see cref="RasterizerState.CullNone"/>.</param>
    /// <param name="depthState">The depth stencil state to use. If null, defaults to <see cref="DepthStencilState.Default"/>.</param>
    /// <param name="blendState">The blend state to use. If null, defaults to <see cref="BlendState.AlphaBlend"/>.</param>
    /// <exception cref="InvalidOperationException">The renderer is not initialized.</exception>
    /// <exception cref="NotSupportedException">The primitive type is not supported. Supported types are: TriangleList, TriangleStrip, LineList, LineStrip, and PointListEXT.</exception>
    public static void DrawMesh(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        in PrimitiveMesh mesh,
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null)
    {
        if (!IsReady)
            throw new InvalidOperationException("SanePrimitiveRenderer is not initialized.");
        if (!mesh.IsValid)
            return;

        Debug.Assert(_effect is not null);

        bool textured = mesh.UsesTexture;

        _effect.TextureEnabled = textured;
        _effect.Texture = textured ? TextureAssets.MagicPixel.Value : null;
        _graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

        _effect.World = world;
        _effect.View = view;
        _effect.Projection = projection;

        var device = _graphicsDevice;

        var previousRasterizer = device.RasterizerState;
        var previousDepth = device.DepthStencilState;
        var previousBlend = device.BlendState;

        device.RasterizerState = rasterizerState ?? RasterizerState.CullNone;
        device.DepthStencilState = depthState ?? DepthStencilState.Default;
        device.BlendState = blendState ?? BlendState.AlphaBlend;

        int primitiveCount = GetPrimitiveCount(mesh.PrimitiveType, mesh.Indices.Length);

        DrawMeshCore(device, _effect, mesh.AsView(), primitiveCount);

        device.RasterizerState = previousRasterizer;
        device.DepthStencilState = previousDepth;
        device.BlendState = previousBlend;
    }

    /// <summary>
    /// Draws the specified mesh using an arbitrary <see cref="Effect"/> instead of the built-in <see cref="BasicEffect"/>.<br/>
    /// The caller is responsible for setting all shader parameters (including any transform matrices) before calling this method.<br/>
    /// A convenience parameter, <paramref name="transformMatrixParam"/>, can be used to automatically set a <c>uTransformMatrix</c>
    /// parameter on the effect to <c>world * view * projection</c>.
    /// </summary>
    /// <param name="world">World matrix.</param>
    /// <param name="view">View matrix.</param>
    /// <param name="projection">Projection matrix.</param>
    /// <param name="mesh">The mesh to draw.</param>
    /// <param name="effect">The custom effect to use for rendering.</param>
    /// <param name="transformMatrixParam">
    /// If non-null, the name of an <see cref="EffectParameter"/> on <paramref name="effect"/> that will be set to <c>world * view * projection</c>.
    /// Defaults to <c>"uTransformMatrix"</c>. Pass <c>null</c> to skip automatic matrix assignment.
    /// </param>
    /// <param name="rasterizerState">The rasterizer state to use. If null, defaults to <see cref="RasterizerState.CullNone"/>.</param>
    /// <param name="depthState">The depth stencil state to use. If null, defaults to <see cref="DepthStencilState.Default"/>.</param>
    /// <param name="blendState">The blend state to use. If null, defaults to <see cref="BlendState.AlphaBlend"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="effect"/> is null.</exception>
    /// <exception cref="NotSupportedException">The primitive type is not supported.</exception>
    public static void DrawMesh(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        in PrimitiveMesh mesh,
        Effect effect,
        string? transformMatrixParam = "uTransformMatrix",
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));
        if (!mesh.IsValid)
            return;

        if (transformMatrixParam != null)
        {
            var param = effect.Parameters[transformMatrixParam];
            if (param != null)
                param.SetValue(world * view * projection);
        }

        var device = _graphicsDevice;

        var previousRasterizer = device.RasterizerState;
        var previousDepth = device.DepthStencilState;
        var previousBlend = device.BlendState;

        device.RasterizerState = rasterizerState ?? RasterizerState.CullNone;
        device.DepthStencilState = depthState ?? DepthStencilState.Default;
        device.BlendState = blendState ?? BlendState.AlphaBlend;

        int primitiveCount = GetPrimitiveCount(mesh.PrimitiveType, mesh.Indices.Length);

        DrawMeshCore(device, effect, mesh.AsView(), primitiveCount);

        device.RasterizerState = previousRasterizer;
        device.DepthStencilState = previousDepth;
        device.BlendState = previousBlend;
    }

    /** <summary>
    Draws a triangle strip using the specified parameters. <para/>
    Alias for <see cref="DrawMesh"/>. Assumes you want a 'triangle strip' primitive type.
    </summary>
    <param name="world">The world matrix.</param>
    <param name="view">The view matrix.</param>
    <param name="projection">The projection matrix.</param>
    <param name="vertices">The vertex positions and colors.</param>
    **/
    public static void DrawTriangleStrip(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        VertexPositionColorTexture[] vertices,
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null)
    {
        if (vertices == null)
            throw new ArgumentNullException(nameof(vertices));
        if (vertices.Length < 3)
            return;
        if (vertices.Length > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(vertices), "Vertex count exceeds index buffer range.");

        DrawMesh(
            world,
            view,
            projection,
            PrimitiveMesh.FromSequential(vertices, PrimitiveType.TriangleStrip),
            rasterizerState,
            depthState,
            blendState);
    }

    /** <summary>
    Draws a triangle strip using the specified parameters. <para/>
    Alias for <see cref="DrawMesh"/>. Assumes you want a 'triangle strip' primitive type.
    </summary>
    <param name="world">The world matrix.</param>
    <param name="view">The view matrix.</param>
    <param name="projection">The projection matrix.</param>
    <param name="vertices">The vertex positions and colors.</param>
    **/
    public static void DrawTriangleStrip(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        VertexPositionColor[] vertices,
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null)
    {
        if (vertices == null)
            throw new ArgumentNullException(nameof(vertices));
        if (vertices.Length < 3)
            return;
        if (vertices.Length > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(vertices), "Vertex count exceeds index buffer range.");

        DrawMesh(
            world,
            view,
            projection,
            PrimitiveMesh.FromSequential(vertices, PrimitiveType.TriangleStrip),
            rasterizerState,
            depthState,
            blendState);
    }

    /** <summary>
    Draws a triangle list using the specified parameters. <para/>
    Alias for <see cref="DrawMesh"/>. Assumes you want a 'triangle list' primitive type.
    </summary>
    <param name="world">The world matrix.</param>
    <param name="view">The view matrix.</param>
    <param name="projection">The projection matrix.</param>
    <param name="vertices">The vertex positions and colors.</param>
    **/
    public static void DrawTriangleList(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        VertexPositionColorTexture[] vertices,
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null)
    {
        if (vertices == null)
            throw new ArgumentNullException(nameof(vertices));
        if (vertices.Length < 3 || vertices.Length % 3 != 0)
            return;
        if (vertices.Length > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(vertices), "Vertex count exceeds index buffer range.");

        DrawMesh(
            world,
            view,
            projection,
            PrimitiveMesh.FromSequential(vertices, PrimitiveType.TriangleList),
            rasterizerState,
            depthState,
            blendState);
    }

    /** <summary>
    Draws a triangle list using the specified parameters. <para/>
    Alias for <see cref="DrawMesh"/>. Assumes you want a 'triangle list' primitive type.
    </summary>
    <param name="world">The world matrix.</param>
    <param name="view">The view matrix.</param>
    <param name="projection">The projection matrix.</param>
    <param name="vertices">The vertex positions and colors.</param>
    **/
    public static void DrawTriangleList(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        VertexPositionColor[] vertices,
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null)
    {
        if (vertices == null)
            throw new ArgumentNullException(nameof(vertices));
        if (vertices.Length < 3 || vertices.Length % 3 != 0)
            return;
        if (vertices.Length > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(vertices), "Vertex count exceeds index buffer range.");

        DrawMesh(
            world,
            view,
            projection,
            PrimitiveMesh.FromSequential(vertices, PrimitiveType.TriangleList),
            rasterizerState,
            depthState,
            blendState);
    }

    public static void DrawMesh(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        in PrimitiveMeshView mesh,
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null)
    {
        if (!IsReady)
            throw new InvalidOperationException("SanePrimitiveRenderer is not initialized.");
        if (!mesh.IsValid)
            return;

        Debug.Assert(_effect is not null);

        bool textured = mesh.UsesTexture;

        _effect.TextureEnabled = textured;
        _effect.Texture = textured ? TextureAssets.MagicPixel.Value : null;
        _graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

        _effect.World = world;
        _effect.View = view;
        _effect.Projection = projection;

        var device = _graphicsDevice;

        var previousRasterizer = device.RasterizerState;
        var previousDepth = device.DepthStencilState;
        var previousBlend = device.BlendState;

        device.RasterizerState = rasterizerState ?? RasterizerState.CullNone;
        device.DepthStencilState = depthState ?? DepthStencilState.Default;
        device.BlendState = blendState ?? BlendState.AlphaBlend;

        int primitiveCount = GetPrimitiveCount(mesh.PrimitiveType, mesh.IndexCount);

        DrawMeshCore(device, _effect, mesh, primitiveCount);

        device.RasterizerState = previousRasterizer;
        device.DepthStencilState = previousDepth;
        device.BlendState = previousBlend;
    }

    public static void DrawMesh(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        in PrimitiveMeshView mesh,
        Effect effect,
        string? transformMatrixParam = "uTransformMatrix",
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));
        if (!mesh.IsValid)
            return;

        if (transformMatrixParam != null)
        {
            var param = effect.Parameters[transformMatrixParam];
            if (param != null)
                param.SetValue(world * view * projection);
        }

        var device = _graphicsDevice;

        var previousRasterizer = device.RasterizerState;
        var previousDepth = device.DepthStencilState;
        var previousBlend = device.BlendState;

        device.RasterizerState = rasterizerState ?? RasterizerState.CullNone;
        device.DepthStencilState = depthState ?? DepthStencilState.Default;
        device.BlendState = blendState ?? BlendState.AlphaBlend;

        int primitiveCount = GetPrimitiveCount(mesh.PrimitiveType, mesh.IndexCount);

        DrawMeshCore(device, effect, mesh, primitiveCount);

        device.RasterizerState = previousRasterizer;
        device.DepthStencilState = previousDepth;
        device.BlendState = previousBlend;
    }

    public static ShaderScope BeginShaderScope(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null,
        SamplerState? samplerState = null)
    {
        if (!IsReady)
            throw new InvalidOperationException("SanePrimitiveRenderer is not initialized.");

        Debug.Assert(_effect is not null);

        return new ShaderScope(
            _graphicsDevice,
            _effect,
            true,
            world,
            view,
            projection,
            null,
            rasterizerState,
            depthState,
            blendState,
            samplerState);
    }

    public static ShaderScope BeginShaderScope(
        Effect effect,
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        string? transformMatrixParam = "uTransformMatrix",
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null,
        SamplerState? samplerState = null)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));

        return new ShaderScope(
            _graphicsDevice,
            effect,
            false,
            world,
            view,
            projection,
            transformMatrixParam,
            rasterizerState,
            depthState,
            blendState,
            samplerState);
    }

    public static void DrawMeshes(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        IReadOnlyList<PrimitiveMesh> meshes,
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null,
        SamplerState? samplerState = null)
    {
        if (meshes == null)
            throw new ArgumentNullException(nameof(meshes));

        using var scope = BeginShaderScope(world, view, projection, rasterizerState, depthState, blendState, samplerState);
        for (int i = 0; i < meshes.Count; i++)
            scope.Draw(meshes[i]);
    }

    public static void DrawMeshes(
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        IReadOnlyList<PrimitiveMeshView> meshes,
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null,
        SamplerState? samplerState = null)
    {
        if (meshes == null)
            throw new ArgumentNullException(nameof(meshes));

        using var scope = BeginShaderScope(world, view, projection, rasterizerState, depthState, blendState, samplerState);
        for (int i = 0; i < meshes.Count; i++)
            scope.Draw(meshes[i]);
    }

    public static void DrawMeshes(
        Effect effect,
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        IReadOnlyList<PrimitiveMesh> meshes,
        string? transformMatrixParam = "uTransformMatrix",
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null,
        SamplerState? samplerState = null)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));
        if (meshes == null)
            throw new ArgumentNullException(nameof(meshes));

        using var scope = BeginShaderScope(effect, world, view, projection, transformMatrixParam, rasterizerState, depthState, blendState, samplerState);
        for (int i = 0; i < meshes.Count; i++)
            scope.Draw(meshes[i]);
    }

    public static void DrawMeshes(
        Effect effect,
        in Matrix world,
        in Matrix view,
        in Matrix projection,
        IReadOnlyList<PrimitiveMeshView> meshes,
        string? transformMatrixParam = "uTransformMatrix",
        RasterizerState? rasterizerState = null,
        DepthStencilState? depthState = null,
        BlendState? blendState = null,
        SamplerState? samplerState = null)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));
        if (meshes == null)
            throw new ArgumentNullException(nameof(meshes));

        using var scope = BeginShaderScope(effect, world, view, projection, transformMatrixParam, rasterizerState, depthState, blendState, samplerState);
        for (int i = 0; i < meshes.Count; i++)
            scope.Draw(meshes[i]);
    }

    private static void DrawMeshCore(
        GraphicsDevice device,
        Effect effect,
        PrimitiveMeshView mesh,
        int primitiveCount)
    {
        bool usesTexture = mesh.UsesTexture;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            if (usesTexture)
            {
                device.DrawUserIndexedPrimitives(
                    mesh.PrimitiveType,
                    mesh.TexturedVertices,
                    mesh.VertexOffset,
                    mesh.VertexCount,
                    mesh.Indices,
                    mesh.IndexOffset,
                    primitiveCount);
            }
            else
            {
                device.DrawUserIndexedPrimitives(
                    mesh.PrimitiveType,
                    mesh.ColorVertices,
                    mesh.VertexOffset,
                    mesh.VertexCount,
                    mesh.Indices,
                    mesh.IndexOffset,
                    primitiveCount);
            }
        }
    }

    private static int GetPrimitiveCount(PrimitiveType primitiveType, int indexCount)
    {
        return primitiveType switch
        {
            PrimitiveType.TriangleList => indexCount / 3,
            PrimitiveType.TriangleStrip => Math.Max(indexCount - 2, 0),
            PrimitiveType.LineList => indexCount / 2,
            PrimitiveType.LineStrip => Math.Max(indexCount - 1, 0),
            PrimitiveType.PointListEXT => indexCount,
            _ => throw new NotSupportedException(primitiveType.ToString())
        };
    }
}
