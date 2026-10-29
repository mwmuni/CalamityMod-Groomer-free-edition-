using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Weapons.Rogue;
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
    public class Catastrophe : CalamitasCloneBrother
    {
        public static Asset<Texture2D> GlowTexture;

        public static int BallDamage = 22; // 88

        protected override bool IsCataclysm => false;
        protected override int OtherBrotherType => ModContent.NPCType<Cataclysm>();
        protected override int PrimaryAttackProjectileType => ModContent.ProjectileType<CatastrophicCinder>();
        protected override int PrimaryAttackDamage => BallDamage;
        protected override SoundStyle HitSoundStyle => SoundID.NPCHit4;
        protected override SoundStyle DeathSoundStyle => SoundID.NPCDeath14;
        protected override string BestiaryEntryKey => "Catastrophe";
        protected override int BestiaryPositionYOffset => -10;
        protected override string GoreNamePrefix => "Catastrophe";
        protected override Asset<Texture2D> GlowTextureAsset => GlowTexture;

        protected override void SetGlowTextureAsset(Asset<Texture2D> asset) => GlowTexture = asset;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            IItemDropRuleCondition killedLast = DropHelper.If(() => !NPC.AnyNPCs(OtherBrotherType), desc: DropHelper.CatastropheKilledLast);
            npcLoot.Add(ItemDropRule.ByCondition(killedLast, ModContent.ItemType<CatastropheTrophy>(), 5));
            npcLoot.Add(ItemDropRule.ByCondition(killedLast, ModContent.ItemType<CrushsawCrasher>()));
        }
    }
}
