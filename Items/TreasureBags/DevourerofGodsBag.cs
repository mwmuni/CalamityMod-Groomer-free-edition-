using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class DevourerofGodsBag : BossTreasureBag, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        protected override bool ForceIntoWorld => true;
        protected override int BagWidth => 36;
        protected override int BagHeight => 34;
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/TreasureBags/DevourerofGodsBagGlow").Value);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<DevourerofGodsHead>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<CosmiliteBar>(), 1, 75, 90);
            itemLoot.Add(ModContent.ItemType<CosmiliteBrick>(), 1, 200, 320);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<MawOfInfinity>(),
                ModContent.ItemType<TheObliterator>(),
                ModContent.ItemType<ThreadOfEradication>(),
                ModContent.ItemType<HyperdeathRiftScepter>(),
                ModContent.ItemType<VoidEaterMarionette>(),
                ModContent.ItemType<DimensionTearingDisk>()
            }));
            itemLoot.Add(ModContent.ItemType<CosmicDischarge>(), 10);

            // Equipment
            itemLoot.Add(ModContent.ItemType<NebulousCore>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<DevourerofGodsMask>(), 7);
            itemLoot.AddIf((info) => CalamityWorld.death && info.player.difficulty == 2, ModContent.ItemType<CosmicPlushie>());
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
