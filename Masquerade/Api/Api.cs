using Il2CppVampireSurvivors.Objects.Characters;
using Masquerade.Character;
using Masquerade.Models;
using static MelonLoader.MelonLogger;

namespace Masquerade.Api
{
    public partial class MasqueradeApi
    {
        private HashSet<CharacterContainer> _characters;
        private HashSet<ModEquipment> _modEquipment;
        private int _nextFreeModInstanceId = 0;
        private int _nextFreeGlobalInstanceId = 0;
        internal IEnumerable<MasqMod> LoadedMods;

        internal ContentFactory<ModAccessory> AccessoryFactory { get; set; }

        internal MasqueradeApi() { AccessoryFactory = new ContentFactory<ModAccessory>(); _characters = new HashSet<CharacterContainer>(); _modEquipment = new HashSet<ModEquipment>(); LoadedMods = []; }

        internal ModAccessory GetOrAddModAccessoryInstance(int contentId, int instanceId)
        {
            if (!AccessoryFactory.DoesContentExist(contentId))
            {
                Masquerade.Logger.Error($"Content {contentId} is not of type ModAccessory");
                return null;
            }
            if (instanceId >= 0 && _modEquipment.Any(x => x.InstanceId == instanceId))
                return _modEquipment.Single(x => x.InstanceId == instanceId) as ModAccessory;

            var newId = _nextFreeModInstanceId;
            var inst = Activator.CreateInstance(Masquerade.Api.GetModAccessory(contentId).GetType()) as ModAccessory;
            if (inst == null)
            {
                Masquerade.Logger.Error($"Unable to create instance of accessory {contentId}");
                return null;
            }
            inst.InstanceId = GetNextFreeModInstanceId();
            _modEquipment.Add(inst);
            _nextFreeModInstanceId++;
            return inst;
        }

        internal CharacterContainer GetOrAddCharacterInstance(int instanceId, CharacterController controller)
        {
            if (instanceId >= 0 && _characters.Any(x => x.InstanceId == instanceId))
                return _characters.Single(x => x.InstanceId == instanceId);

            var inst = new CharacterContainer();
            SyncCharacterContainer(ref inst, controller);
            if (inst == null)
            {
                Masquerade.Logger.Error($"Unable to create instance of character {controller.name}");
                return null;
            }
            inst.InstanceId = GetNextFreeGlobalInstanceId();
            _characters.Add(inst);
            _nextFreeModInstanceId++;
            return inst;
        }

        // Add this to: RemoveAllEquipmentFromPlayers
        internal void DeleteEquipmentInstance(int instanceId)
        {
            var instance = _modEquipment.SingleOrDefault(x => x.InstanceId == instanceId);
            if (instanceId < 0|| instance == null)
                return;

            _modEquipment.Remove(instance);
            _nextFreeModInstanceId = instanceId;
        }

        internal void CleanUpInstances()
        {
            _modEquipment.Clear();
            _characters.Clear();
            _nextFreeModInstanceId = 0;
            _nextFreeGlobalInstanceId = 0;
        }

        private int GetNextFreeModInstanceId()
        {
            if  (_modEquipment.Any(x => x.InstanceId == _nextFreeModInstanceId))
            {
                _nextFreeModInstanceId++;
                return GetNextFreeModInstanceId();
            }
            return _nextFreeModInstanceId;
        }
        private int GetNextFreeGlobalInstanceId()
        {
            if (_characters.Any(x => x.InstanceId == _nextFreeGlobalInstanceId))
            {
                _nextFreeGlobalInstanceId++;
                return GetNextFreeGlobalInstanceId();
            }
            return _nextFreeGlobalInstanceId;
        }

        private void SyncCharacterContainer(ref CharacterContainer container, CharacterController controller)
        {
            container.CurrentHP = controller.CurrentHealth();
            container.Exp = controller.Xp;
            container.Name = controller.CharacterType.ToString();
            container.Level = controller.Level;
            container.Stats = new CharacterStats();
            //Set Stats
            foreach (var equip in controller.AccessoriesManager.ActiveEquipment)
            {
                if (equip != null && (equip.gameObject?.TryGetComponent<ModInstanceComponent>(out var component) ?? false) && component.ModInstanceId.Value >= 0)
                    container.AddEquipmentModId(component.ModInstanceId.Value);
            }
        }
    }
}
