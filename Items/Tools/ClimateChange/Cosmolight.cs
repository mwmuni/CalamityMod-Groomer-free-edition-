using CalamityMod.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;

namespace CalamityMod.Items.Tools.ClimateChange
{
    public class Cosmolight : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.useAnimation = 9;
            Item.useTime = 9;
            Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item60;
            Item.consumable = false;
            Item.channel = true;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.ToolsOther;
        }


        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
                if (Main.netMode != NetmodeID.Server && player == Main.LocalPlayer && (player.altFunctionUse == 2 || (CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled)))
                {
                    var power = CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>();
                    NetPacket packet = NetCreativePowersModule.PreparePacket(power.PowerId, 1);
                    packet.Writer.Write(!power.Enabled);
                    NetManager.Instance.SendToServerOrLoopback(packet);
                }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Bakidon>().
                AddIngredient(ItemID.Sundial).
                AddIngredient<AstralBar>(12).
                AddIngredient(ItemID.FragmentSolar, 15).
                AddTile(TileID.DemonAltar).
                Register();

            CreateRecipe().
                AddIngredient<Bakidon>().
                AddIngredient(ItemID.Moondial).
                AddIngredient<AstralBar>(12).
                AddIngredient(ItemID.FragmentSolar, 15).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }

    public class CosmolightTimeRateChange : ModSystem
    {
        public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
        {
            int cosmolightType = ModContent.ItemType<Cosmolight>();
            foreach (Player player in Main.player)
            {
                if (player.active && player.channel && player.altFunctionUse != 2 && player.HeldItem.type == cosmolightType)
                {
                    timeRate *= 120;
                    break;
                }
            }
        }
    }
}
