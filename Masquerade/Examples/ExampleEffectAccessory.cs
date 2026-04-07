using HarmonyLib;
using Il2CppVampireSurvivors.Objects.Characters;
using Masquerade.Character;
using Masquerade.Stats;
using Masquerade.Util;

namespace Masquerade.Examples
{
    public sealed class ExampleEffectAccessory : ModAccessory
    {
        public override string DisplayName => "Example Effect Accessory";

        public override string Description => $"Gain bonuses after standing still and charge {ChargePrecentage * 100}% faster!";

        public override string Tips => "Increases ALL charge times.";

        public override int AppearenceRate => 5000;

        public override int MaxLevel => 2;

        private readonly float ChargePrecentage = 0.5f;

        public ExampleEffectAccessory() : base()
        {
            LevelingManager.AddAtLevel(level: 2, datapoint: "customDescValue", "Charge Speed increased by %0. ");
            LevelingManager.AddAtLevel(level: 2, stat: "customDesc", ChargePrecentage * 100);
        }

        public override ShopTags ShopTags => ShopTags.StartsSeen | ShopTags.StartsUnlocked;

        public override void OnAccessoryAdded(CharacterContainer character, CharacterController controller)
        {
            if (controller == null) return;
            var charlot = controller.TryCast<TP_Charlotte_Character>();
            if (charlot == null) return;
            charlot._maxChargeTimeMS *= ChargePrecentage;
        }

        public override void OnAccessoryRemoved(CharacterContainer character)
        {
            
        }

        public override void OnLevelUp(CharacterController controller)
        {
            if (controller == null) return;
            var charlot = controller.TryCast<TP_Charlotte_Character>();
            if (charlot == null) return;
            charlot._maxChargeTimeMS *= ChargePrecentage;
        }
    }
}
