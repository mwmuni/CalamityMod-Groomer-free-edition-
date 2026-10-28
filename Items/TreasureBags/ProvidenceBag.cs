using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class ProvidenceBag : BossTreasureBag, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Providence>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<UnholyEssence>(), 1, 40, 50);
            itemLoot.Add(ModContent.ItemType<DivineGeode>(), 1, 60, 70);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<HolyCollider>(),
                ModContent.ItemType<BurningRevelation>(),
                ModContent.ItemType<BlissfulBombardier>(),
                ModContent.ItemType<TelluricGlare>(),
                ModContent.ItemType<PurgeGuzzler>(),
                ModContent.ItemType<DazzlingStabberStaff>(),
                ModContent.ItemType<MoltenAmputator>(),
            }));
            itemLoot.Add(ModContent.ItemType<PristineFury>(), 10);

            // Equipment
            itemLoot.Add(ModContent.ItemType<DivineProvidence>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<ProvidenceMask>(), 7);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
