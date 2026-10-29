using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{

    //Draws the front arm of the armor over the shoulderpad when walking. 
    public class FrontARmOverShoulderpadLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.shadow == 0f || !drawInfo.drawPlayer.dead;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Item bodyItem = drawPlayer.armor[1];
            if (drawPlayer.armor[11].type > ItemID.None)
                bodyItem = drawPlayer.armor[11];

            if (ModContent.GetModItem(bodyItem.type) is IDrawArmOverShoulderpad frontArmDrawer)
            {
                string equipSlotName = frontArmDrawer.EquipSlotName(drawPlayer) != "" ? frontArmDrawer.EquipSlotName(drawPlayer) : bodyItem.ModItem.Name;
                int equipSlot = EquipLoader.GetEquipSlot(Mod, equipSlotName, EquipType.Body);
                if (drawPlayer.body != equipSlot)
                    return;


                BodyExtensionDrawHelper.Draw(ref drawInfo, frontArmDrawer.FrontArmTexture);
            }
        }
    }
}
