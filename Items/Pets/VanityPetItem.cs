using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public abstract class VanityPetItem<TProjectile, TBuff> : ModItem
        where TProjectile : ModProjectile
        where TBuff : ModBuff
    {
        protected virtual int PetBuffDuration => 3600;

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<TProjectile>(), ModContent.BuffType<TBuff>());
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, PetBuffDuration, true);
            }
        }
    }
}
