using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Microsoft.Xna.Framework
{
    public struct Vector2(float x, float y)
    {
        public float X = x, Y = y;
        public static Vector2 UnitX => new(1, 0);
        public static Vector2 UnitY => new(0, 1);
        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator *(Vector2 a, float b) => new(a.X * b, a.Y * b);
        public static Vector2 operator /(Vector2 a, float b) => new(a.X / b, a.Y / b);
        public Point ToPoint() => new();
    }
    public struct Point;
    public struct Matrix;
    public struct Rectangle(int x, int y, int width, int height)
    {
        public int X = x, Y = y, Width = width, Height = height;
        public Vector2 Size() => new(Width, Height);
    }
    public struct Color
    {
        public static Color OrangeRed => default;
        public Color MultiplyRGB(Color color) => color;
    }
    public class GameTime;
}

namespace Microsoft.Xna.Framework.Graphics
{
    public class Effect;
    public class Texture2D;
    public sealed class BlendState
    {
        public static readonly BlendState AlphaBlend = new(), Additive = new(), NonPremultiplied = new();
    }
    public sealed class SamplerState { public static readonly SamplerState LinearClamp = new(); }
    public sealed class DepthStencilState { public static readonly DepthStencilState None = new(); }
    public sealed class RasterizerState
    {
        public bool ScissorTestEnable;
        public static readonly RasterizerState CullNone = new();
    }
    public enum SpriteSortMode { Deferred, Immediate }
    public enum SpriteEffects { None }
    public sealed class GraphicsDevice
    {
        public RasterizerState RasterizerState = new();
        public Rectangle ScissorRectangle;
    }
    public sealed class SpriteBatch
    {
        public bool IsDrawing;
        public void Begin(SpriteSortMode mode, BlendState blend, SamplerState sampler, DepthStencilState depth,
            RasterizerState rasterizer, Effect effect, Matrix matrix)
        {
            if (IsDrawing)
                throw new InvalidOperationException("Unbalanced sprite-batch begin.");
            IsDrawing = true;
        }
        public void End()
        {
            if (!IsDrawing)
                throw new InvalidOperationException("Unbalanced sprite-batch end.");
            IsDrawing = false;
        }
        public void Draw(Texture2D texture, Vector2 position, Rectangle rectangle, Color color, float rotation,
            Vector2 origin, float scale, SpriteEffects effects, float depth) =>
            throw new NotSupportedException("GPU sprite drawing is not implemented by the regression double.");
    }
}

namespace ReLogic.Content
{
    public enum AssetRequestMode { AsyncLoad }
    public sealed class Asset<T>(T value)
    {
        public T Value => value;
        public Rectangle Frame(int columns, int rows, int x, int y) => new(0, 0, 1, 1);
    }
}

namespace Terraria
{
    public sealed class Player
    {
        public bool trippy;
        public Vector2 Center;
    }
    public sealed class ViewMatrix { public Matrix TransformationMatrix; }
    public sealed class GraphicsManager { public GraphicsDevice GraphicsDevice = new(); }
    public class Main
    {
        private static readonly ConcurrentQueue<Action> _mainThreadActions = new();
        public static int netMode;
        public static bool dedServ => netMode == 2;
        public static bool gamePaused;
        public static readonly Player LocalPlayer = new();
        public static readonly GraphicsManager graphics = new();
        public static readonly SpriteBatch spriteBatch = new();
        public static readonly RasterizerState Rasterizer = new();
        public static readonly ViewMatrix GameViewMatrix = new();
        public static int screenWidth = 1920, screenHeight = 1080;
        public static Vector2 screenPosition;
        public static Color DiscoColor;
        public static string SavePath = Path.GetTempPath();
        public static void QueueMainThreadAction(Action action) => _mainThreadActions.Enqueue(action);
        public static void RunMainThreadActions()
        {
            while (_mainThreadActions.TryDequeue(out var action))
                action();
        }
    }
    public static class Netplay { public static bool HasClients; }
    public static class Lighting { public static Color GetColor(Point point) => default; }
    public static class On_Main
    {
        public delegate void orig_DoUpdate(Main self, ref GameTime gameTime);
        public delegate void hook_DoUpdate(orig_DoUpdate orig, Main self, ref GameTime gameTime);
        public delegate void orig_DoDraw(Main self, GameTime gameTime);
        public delegate void hook_DoDraw(orig_DoDraw orig, Main self, GameTime gameTime);
        public static event hook_DoUpdate DoUpdate;
        public static event hook_DoDraw DoDraw;
        public static bool HasUpdateHook => DoUpdate is not null;
        public static bool HasDrawHook => DoDraw is not null;
        public static bool HasHooks => HasUpdateHook || HasDrawHook;
        public static void RunUpdate()
        {
            var time = new GameTime();
            DoUpdate?.Invoke(static (Main self, ref GameTime gameTime) => { }, new Main(), ref time);
        }
        public static void RunDraw() => DoDraw?.Invoke(static (self, time) => { }, new Main(), new GameTime());
    }
}

namespace Terraria.ID
{
    public static class NetmodeID
    {
        public const int SinglePlayer = 0, MultiplayerClient = 1, Server = 2;
    }
}

namespace Terraria.ModLoader
{
    public enum ModSide { Client }
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AutoloadAttribute : Attribute { public ModSide Side { get; set; } }
    public class ModSystem
    {
        public Mod Mod => CalamityMod.CalamityMod.Instance;
        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void OnWorldUnload() { }
        public virtual void PostSetupContent() { }
    }
    public static class ModContent
    {
        public static Asset<T> Request<T>(string path, AssetRequestMode mode) where T : new() => new(new T());
    }
    public sealed class Mod
    {
        public CalamityMod.TestLog Logger => CalamityMod.CalamityMod.Log;
        public short NetID => 1;
        public Version Version => new(2, 2, 2);
    }
    [Flags]
    public enum CommandType { Chat = 1, Server = 2, Console = 4 }
    public class ModCommand
    {
        public Mod Mod => CalamityMod.CalamityMod.Instance;
        public virtual string Command => "";
        public virtual CommandType Type => CommandType.Chat;
        public virtual string Usage => "";
        public virtual string Description => "";
        public virtual void Action(CommandCaller caller, string input, string[] args) { }
    }
    public sealed class CommandCaller
    {
        public readonly List<string> Replies = [];
        public void Reply(string message, Color color = default) => Replies.Add(message);
    }
    public sealed class UsageException(string message) : Exception(message);
    public class ModPacket : BinaryWriter
    {
        public short netID;
        public ushort len;
        public int Sends, LastIgnoreClient;
        public ModPacket(short modId) : base(new MemoryStream())
        {
            netID = modId;
            Write((ushort)0);
            Write((byte)250);
            Write((byte)modId);
        }
        public void Send(int toClient = -1, int ignoreClient = -1)
        {
            if (MonoMod.RuntimeDetour.Hook.Callback is { } hook)
                hook(static (packet, to, ignore) => packet.SendCore(to, ignore), this, toClient, ignoreClient);
            else
                SendCore(toClient, ignoreClient);
        }
        private void SendCore(int toClient, int ignoreClient)
        {
            len = checked((ushort)BaseStream.Length);
            Sends++;
            LastIgnoreClient = ignoreClient;
        }
    }
}

namespace MonoMod.RuntimeDetour
{
    using Terraria.ModLoader;
    public sealed class Hook : IDisposable
    {
        public static Action<Action<ModPacket, int, int>, ModPacket, int, int> Callback;
        public static bool IsActive => Callback is not null;
        public static bool FailNextCreation;
        public static int CreatedOnThread, DisposedOnThread;
        public Hook(MethodInfo method, Delegate callback)
        {
            if (FailNextCreation)
            {
                FailNextCreation = false;
                throw new InvalidOperationException("Simulated detour installation failure.");
            }
            if (method.Name != nameof(ModPacket.Send) || IsActive)
                throw new InvalidOperationException("Unexpected regression hook.");
            Callback = (Action<Action<ModPacket, int, int>, ModPacket, int, int>)callback;
            CreatedOnThread = Environment.CurrentManagedThreadId;
        }
        public void Dispose()
        {
            Callback = null;
            DisposedOnThread = Environment.CurrentManagedThreadId;
        }
    }
}

namespace CalamityMod
{
    public sealed class TestLog
    {
        public readonly List<string> Errors = [], Warnings = [], Information = [];
        public void Error(string message, Exception exception) => Errors.Add(message);
        public void Warn(string message) => Warnings.Add(message);
        public void Info(string message) => Information.Add(message);
    }
    public static class CalamityMod
    {
        public static readonly Terraria.ModLoader.Mod Instance = new();
        public static readonly TestLog Log = new();
    }
    public sealed class CalamityClientConfig
    {
        public static readonly CalamityClientConfig Instance = new();
        public int ParticleLimit = 5000;
    }
    public static class CalamityNetcode { public static string[] GetPacketHandlerNames() => ["KnownPacket"]; }
    public static class CalamityUtils { public static Terraria.Player Calamity(this Terraria.Player player) => player; }
    public static class ReflectionHelper
    {
        public static void IterateEveryModsTypes<T>(Action<Type> action) =>
            throw new NotSupportedException("Cross-mod content loading is not implemented by the regression double.");
    }
}

namespace CalamityMod.Particles
{
    public abstract class Particle
    {
        public int Time, Lifetime, Type, FrameVariants = 1, Variant;
        public bool Pixelate, AffectedByLight;
        public float Rotation, Scale = 1;
        public Vector2 Position, Velocity;
        public Color Color;
        public GeneralDrawLayer DrawLayer = GeneralDrawLayer.AfterDusts;
        public virtual string Texture => "";
        public virtual AssetRequestMode TextureRequestMode => AssetRequestMode.AsyncLoad;
        public virtual bool Important => false;
        public virtual bool SetLifetime => false;
        public virtual bool UseCustomDraw => false;
        public virtual bool UseAdditiveBlend => false;
        public virtual bool UseHalfTransparency => false;
        public virtual Effect CustomShader => null;
        public virtual void Update() { }
        public virtual void CustomDraw(SpriteBatch spriteBatch) { }
        public virtual void PrepareCustomShader(Effect effect) { }
    }
}

namespace CalamityMod.Systems.Graphic
{
    public static class GeneralDrawLayerSystem
    {
        public static event Action<GeneralDrawLayer> OnDrawLayer;
        public static void Draw(GeneralDrawLayer layer) => OnDrawLayer?.Invoke(layer);
        public static void Reset() => OnDrawLayer = null;
    }
}

namespace CalamityMod.Systems.Graphic.PixelationSystem
{
    public static class PixelationManager
    {
        public static void AddPixelatedDrawer(Action<Matrix> drawer, GeneralDrawLayer layer, BlendState blend) =>
            throw new NotSupportedException("Deferred pixelated rendering is not implemented by the regression double.");
    }
}
