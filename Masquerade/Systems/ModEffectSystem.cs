using Il2CppVampireSurvivors.Objects.Characters;
using Masquerade.Models;
using Masquerade.Util;

namespace Masquerade.Systems
{
    public class ModEffectSystem
    {
        public ModEffectSystem()
        {
            Instances = new Dictionary<int, ICollection<ModCharacterEffect>>();
        }

        internal IDictionary<int, ICollection<ModCharacterEffect>> Instances { get; private set; }

        public void AddInstance(CharacterController controller, ModCharacterEffect effect, bool allowDuplicates = false)
        {
            if (!TryGetInstanceId(controller, out int instanceId) || (!allowDuplicates && HasEffect(effect.ContentId, instanceId))) return;

            var effects = new List<ModCharacterEffect>(GetOrAddCharacter(instanceId));
            effects.Add(effect);
            Instances[instanceId] = effects;
            LoggerHelper.Logger.Msg($"Added effect {effect.DisplayName} to character {controller.name} id {instanceId}");
        }

        public void RemoveInstance(CharacterController controller, int effectId)
        {
            if (!TryGetInstanceId(controller, out int instanceId) || !HasEffect(effectId, instanceId)) return;

            var effects = new List<ModCharacterEffect>(GetOrAddCharacter(instanceId));
            Instances[instanceId] = effects.Where(x => x.ContentId != effectId).ToList();
            LoggerHelper.Logger.Msg($"Removed effect id {effectId} from character {controller.name} id {instanceId}");
        }

        internal void CleanUpInstances()
        {
            Instances?.Clear();
        }

        internal bool HasEffect(int effectContentId, int characterInstanceId)
        {
            if (characterInstanceId < 0 || !Instances.Any(x => x.Key == characterInstanceId)) return false;

            return Instances[characterInstanceId].Any(x => x.ContentId == effectContentId);
        }

        internal void InternalUpdateEffects(CharacterController controller)
        {
            if (!Instances.Any() || !TryGetInstanceId(controller, out int instanceId) || instanceId < 0 || !Instances.Keys.Any(x => x == instanceId)) return;
            foreach (var effect in Instances[instanceId])
            {
                effect.OnUpdate(controller);
            }
        }

        internal void ResetCharacterEffects(CharacterController controller)
        {
            if (!TryGetInstanceId(controller, out int instanceId)) return;

            Instances[instanceId].Clear();
        }

        internal bool TryGetEffect<T>(CharacterController controller, out T effect) where T : ModCharacterEffect
        {
            effect = null;
            if (!TryGetInstanceId(controller, out int instanceId)) return false;

            var contentId = Masquerade.Api.GetModCharacterEffect<T>().ContentId;

            if (!HasEffect(contentId, instanceId)) return false;
            effect = Instances[instanceId].SingleOrDefault(x => x.ContentId == contentId) as T;
            return effect != null;
        }

        internal bool TryGetInstanceId(CharacterController controller, out int instanceId)
        {
            instanceId = -1;
            if (!controller.gameObject.TryGetComponent<GlobalInstanceComponent>(out GlobalInstanceComponent instance)) return false;
            instanceId = instance.GlobalInstanceId;
            return true;
        }

        private IEnumerable<ModCharacterEffect> GetOrAddCharacter(int instanceId)
        {
            if (Instances.ContainsKey(instanceId)) return Instances[instanceId];

            Instances.Add(instanceId, new List<ModCharacterEffect>());
            return Instances[instanceId];
        }
    }
}