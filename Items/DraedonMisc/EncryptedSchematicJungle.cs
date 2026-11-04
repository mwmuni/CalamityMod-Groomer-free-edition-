using System;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Placeables.PlaceableTurrets;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.UI.DraedonLogs;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicJungle : EncryptedSchematic
    {
        protected override string SchematicName => "Jungle";

        protected override ref bool HasFoundSchematic => ref RecipeUnlockHandler.HasFoundJungleSchematic;

        protected override ref bool HasUnlockedRecipes => ref RecipeUnlockHandler.HasUnlockedT3ArsenalRecipes;

        protected override Type SchematicGUIType => typeof(DraedonSchematicJungleGUI);

        protected override SchematicDisplay[] Displays => new[]
        {
            Display<GalvanizingGlaive>("MeleeDisplay", new Color(149, 243, 43)),
            Display<Nidhogg>("RangedDisplay", new Color(236, 255, 31)),
            Display<CountermeasureMitt>("MageDisplay", new Color(255, 64, 31)),
            Display<PulseTurretRemote>("SummonDisplay", new Color(201, 41, 255)),
            Display<SystemBane>("RogueDisplay", new Color(31, 242, 245)),
            Display<PlagueTurret>("CodeDisplay", MachineColor),
            Display<AdvancedDisplay>("CodeDisplay", MachineColor),
            Display<Abombination>("MeleeDisplay", new Color(149, 169, 182))
        };
    }
}
