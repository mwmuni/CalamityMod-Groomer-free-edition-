using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    [LongDistanceNetSync(SyncWith = typeof(OarfishHead))]
    public class OarfishBody : OarfishSegmentBase
    {
        public override void SetDefaults()
        {
            NPC.damage = 30; //70
            NPC.width = 16; //28
            NPC.height = 16; //28
            NPC.defense = 20;
            NPC.lifeMax = 4000;
            NPC.aiStyle = -1; //new
            AIType = -1; //new
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.chaseable = false;
            Banner = ModContent.NPCType<OarfishHead>();
            BannerItem = ModContent.ItemType<OarfishBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("OarfishBody").Type, 1f);
            }
        }
    }
}
