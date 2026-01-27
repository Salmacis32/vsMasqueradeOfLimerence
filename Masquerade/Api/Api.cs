using Il2CppVampireSurvivors.Objects.Characters;
using Masquerade.Character;
using Masquerade.Models;
using static MelonLoader.MelonLogger;

namespace Masquerade.Api
{
    public partial class MasqueradeApi
    {
        private HashSet<CharacterContainer> _characters;
        private int _nextFreeGlobalInstanceId = 0;
        internal IEnumerable<MasqMod> LoadedMods;

        internal ContentFactory<ModAccessory> AccessoryFactory { get; set; }

        internal MasqueradeApi() { AccessoryFactory = new ContentFactory<ModAccessory>(); _characters = new HashSet<CharacterContainer>(); LoadedMods = []; }

        internal ModAccessory GetOrAddModAccessoryInstance(int contentId, CharacterContainer container)
        {
            if (!AccessoryFactory.DoesContentExist(contentId))
            {
                Masquerade.Logger.Error($"Content {contentId} is not of type ModAccessory");
                return null;
            }
            if (contentId >= 0 && container.HasEquipment(contentId))
                return container.GetEquipment(contentId) as ModAccessory;

            var proto = Masquerade.Api.GetModAccessory(contentId);
            var inst = Activator.CreateInstance(proto.GetType()) as ModAccessory;
            if (inst == null)
            {
                Masquerade.Logger.Error($"Unable to create instance of accessory {contentId}");
                return null;
            }
            inst.Mod = proto.Mod;
            inst.ContentId = proto.ContentId;
            inst.Owner = container;
            container.AddEquipmentId(contentId);
            container.AddModEquipment(inst);
            return inst;
        }

        internal CharacterContainer GetOrAddCharacterInstance(CharacterController controller, GlobalInstanceComponent component)
        {
            var instanceId = component.GlobalInstanceId;
            if (instanceId >= 0 && _characters.Any(x => x.InstanceId == instanceId))
                return _characters.Single(x => x.InstanceId == instanceId);

            var inst = new CharacterContainer();
            SyncCharacterContainer(ref inst, controller);
            if (inst == null)
            {
                Masquerade.Logger.Error($"Unable to create instance of character {controller.name}");
                return null;
            }
            instanceId = GetNextFreeGlobalInstanceId();
            inst.InstanceId = instanceId;
            component.GlobalInstanceId = instanceId;
            _characters.Add(inst);
            return inst;
        }

        // Add this to: RemoveAllEquipmentFromPlayers
        internal void DeleteEquipmentInstance(int contentId, CharacterContainer container)
        {
            var instance = _characters.SingleOrDefault(x => x.InstanceId == contentId);
            if (contentId < 0|| !container.HasEquipment(contentId))
                return;

            container.RemoveModEquipment(contentId);
        }

        internal void CleanUpInstances()
        {
            _characters.Clear();
            _nextFreeGlobalInstanceId = 0;
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
                if (equip != null && !container.HasEquipment((int)equip.Type))
                    container.AddEquipmentId((int)equip.Type);
            }
        }
    }
}
