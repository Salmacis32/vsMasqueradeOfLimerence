using Il2Cpp;
using Il2CppVampireSurvivors.Interfaces;
using Il2CppVampireSurvivors.Objects.Characters;
using UnityEngine;

namespace Masquerade.Examples
{
    public class ExampleEffect : ModCharacterEffect
    {
        public float CurrentMultiplier { get; set; }

        public override void OnUpdate(CharacterController owner)
        {
            var charlot = owner.TryCast<TP_Charlotte_Character>();
            if (charlot == null || charlot._chargeTime <= 0 || charlot.Walked != 0f || CurrentMultiplier == 0f)
                return;
            var increase = PauseSystem.DeltaTimeMillis * Mathf.Min(charlot.PGrowth(), 5f) * CurrentMultiplier;
            charlot._chargeTime += increase;
        }
    }
}
