using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.CalClone
{
    [AutoloadBossHead]
    public class Cataclysm : CalamitasCloneBrother
    {
        public static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Custom/CalamitasClone/CataclysmHit", 3);
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/Custom/CalamitasClone/CataclysmDeath");
        public static readonly SoundStyle FlamethrowerStart = new("CalamityMod/Sounds/Custom/CalamitasClone/BrimstoneFlamethrowerCast");
        public static readonly SoundStyle FlamethrowerLoop = new("CalamityMod/Sounds/Custom/CalamitasClone/BrimstoneFlamethrowerLoop");

        public static int FlamethrowerDamage = 30; // 120

        protected override bool IsCataclysm => true;
        protected override int OtherBrotherType => ModContent.NPCType<Catastrophe>();
        protected override int PrimaryAttackProjectileType => ModContent.ProjectileType<CataclysmicFlame>();
        protected override int PrimaryAttackDamage => FlamethrowerDamage;
        protected override SoundStyle HitSoundStyle => HitSound;
        protected override SoundStyle DeathSoundStyle => DeathSound;
        protected override string BestiaryEntryKey => "Cataclysm";
        protected override int BestiaryPositionYOffset => 0;
        protected override string GoreNamePrefix => "Cataclysm";
        protected override Asset<Texture2D> GlowTextureAsset => GlowTexture;

        protected override void SetGlowTextureAsset(Asset<Texture2D> asset) => GlowTexture = asset;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            IItemDropRuleCondition killedLast = DropHelper.If(() => !NPC.AnyNPCs(OtherBrotherType), desc: DropHelper.CataclysmKilledLast);
            npcLoot.Add(ItemDropRule.ByCondition(killedLast, ModContent.ItemType<CataclysmTrophy>(), 5));
            npcLoot.Add(ItemDropRule.ByCondition(killedLast, ModContent.ItemType<HavocsBreath>()));
        }
    }
}
