using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using Masquerade.Character;
using Masquerade.Equipment;
using Masquerade.Stats;

namespace Masquerade.Examples
{
    public sealed class ExampleEffectAccessory : ModAccessory
    {
        public override string DisplayName => "Example Effect Accessory";

        public override string Description => $"Gain bonuses after standing still and charge {ChargePrecentage * 100}% faster!";

        public override string Tips => "Increases ALL charge times.";

        public override int AppearenceRate => 5000;

        public override int MaxLevel => 2;

        private readonly float ChargePrecentage = 2f;

        private int _effectId;

        public ExampleEffectAccessory() : base()
        {
            LevelingManager.AddAtLevel(level: 2, datapoint: WeaponDataNames.CustomDescription, "Charge Speed increased by an additional %0%");
            LevelingManager.AddAtLevel(level: 2, stat: WeaponDataNames.CustomDescriptionVariable, ChargePrecentage * 100);
        }

        public override ShopTags ShopTags => ShopTags.StartsSeen | ShopTags.StartsUnlocked;

        public override void OnAccessoryAdded(CharacterController controller)
        {
            if (controller == null) return;
            var charlot = controller.TryCast<TP_Charlotte_Character>();
            if (charlot == null) return;
            var effect = Masquerade.Api.GetModCharacterEffect<ExampleEffect>();
            _effectId = effect.ContentId;
            effect.CurrentMultiplier = ChargePrecentage;
            Masquerade.Instance.ModEffectSystem.AddInstance(controller, effect);
        }

        public override void OnAccessoryRemoved(CharacterController controller)
        {
            Masquerade.Instance.ModEffectSystem.RemoveInstance(controller, _effectId);
        }

        public override void OnLevelUp(CharacterController controller)
        {
            if (controller == null) return;
            var charlot = controller.TryCast<TP_Charlotte_Character>();
            if (charlot == null) return;

            if (Masquerade.Instance.ModEffectSystem.TryGetEffect<ExampleEffect>(controller, out ExampleEffect effect))
                effect.CurrentMultiplier = ChargePrecentage * controller.AccessoriesManager.GetAccessoryByType((WeaponType)WeaponTypeId).Level;
        }
    }
}
