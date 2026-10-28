using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Perforator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class PerforatorBag : BossTreasureBag, ILocalizedModType
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
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<PerforatorHive>()));

            // Materials
            itemLoot.Add(ItemID.CrimtaneBar, 1, 15, 20);
            itemLoot.Add(ItemID.Vertebrae, 1, 15, 20);
            itemLoot.AddIf(() => Main.hardMode, ItemID.Ichor, 1, 25, 30);
            itemLoot.Add(ItemID.CrimsonSeeds, 1, 10, 15);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new WeightedItemStack[]
            {
                ModContent.ItemType<Aorta>(),
                ModContent.ItemType<SausageMaker>(),
                ModContent.ItemType<VeinBurster>(),
                ModContent.ItemType<Eviscerator>(),
                ModContent.ItemType<BloodBath>(),
                ModContent.ItemType<FleshOfInfidelity>(),
                ModContent.ItemType<ToothBall>(),
            }));

            // Equipment
            itemLoot.Add(ModContent.ItemType<BloodstainedGlove>(), DropHelper.BagWeaponDropRateFraction);
            itemLoot.Add(ModContent.ItemType<BloodyWormTooth>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<PerforatorMask>(), 7);
            itemLoot.Add(ModContent.ItemType<BloodyVein>(), 10);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
