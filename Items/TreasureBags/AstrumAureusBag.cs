using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    [LegacyName("AstrageldonBag")]
    public class AstrumAureusBag : BossTreasureBag, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<AstrumAureus>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<AureusCell>(), 1, 12, 16);
            itemLoot.Add(ModContent.ItemType<StarblightSoot>(), 1, 30, 40);
            itemLoot.Add(ItemID.FallenStar, 1, 20, 30);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<Nebulash>(),
                ModContent.ItemType<AuroraBlazer>(),
                ModContent.ItemType<AlulaAustralis>(),
                ModContent.ItemType<BorealisBomber>(),
                ModContent.ItemType<AuroradicalThrow>()
            }));
            itemLoot.Add(ModContent.ItemType<LeonidProgenitor>(), 10);

            // Equipment
            itemLoot.Add(ModContent.ItemType<SuspiciousLookingJellyBean>());
            itemLoot.Add(ModContent.ItemType<InterstellarStompers>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<AstrumAureusMask>(), 7);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

            // Other
            itemLoot.AddIf((info) => CalamityWorld.revenge && !info.player.Calamity().adrenalineBoostTwo, ModContent.ItemType<StarlightFuelCell>());
        }
    }
}
