using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.HiveMind;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class HiveMindBag : BossTreasureBag, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        protected override bool IsPreHardmodeLike => true;
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<HiveMind>()));

            // Materials
            itemLoot.Add(ItemID.DemoniteBar, 1, 15, 20);
            itemLoot.Add(ItemID.RottenChunk, 1, 15, 20);
            itemLoot.AddIf(() => Main.hardMode, ItemID.CursedFlame, 1, 25, 30);
            itemLoot.Add(ItemID.CorruptSeeds, 1, 10, 15);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new WeightedItemStack[]
            {
                ModContent.ItemType<PerfectDark>(),
                ModContent.ItemType<Shadethrower>(),
                ModContent.ItemType<ShaderainStaff>(),
                ModContent.ItemType<DankStaff>(),
                ModContent.ItemType<RotBall>(),
            }));

            // Equipment
            itemLoot.Add(ModContent.ItemType<FilthyGlove>(), DropHelper.BagWeaponDropRateFraction);
            itemLoot.Add(ModContent.ItemType<RottenBrain>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<HiveMindMask>(), 7);
            itemLoot.Add(ModContent.ItemType<RottingEyeball>(), 10);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
