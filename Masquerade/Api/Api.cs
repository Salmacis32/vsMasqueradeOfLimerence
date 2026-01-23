using Masquerade.Models;

namespace Masquerade.Api
{
    public partial class MasqueradeApi
    {
        private HashSet<ModCharacter> _characters;
        private HashSet<ModEquipment> _equipment;
        private int _nextFreeInstanceId = 0;
        internal IEnumerable<MasqMod> LoadedMods;
        /*
        internal void AddReferenceFromEquip(Il2CppVampireSurvivors.Objects.Equipment equip)
        {
            if (_characters.Values.Any(x => x.Instance == equip))
            _characters.Add(_characters.Count(), new CharacterReference(equip));
        }
        */
        internal ContentFactory<ModAccessory> AccessoryFactory { get; set; }

        internal MasqueradeApi() { AccessoryFactory = new ContentFactory<ModAccessory>(); _characters = new HashSet<ModCharacter>(); _equipment = new HashSet<ModEquipment>(); LoadedMods = []; }

        internal ModAccessory GetOrAddAccessoryInstance(int contentId, int instanceId)
        {
            if (!AccessoryFactory.DoesContentExist(contentId))
            {
                Masquerade.Logger.Error($"Content {contentId} is not of type ModAccessory");
                return null;
            }
            if (instanceId >= 0 && _equipment.Any(x => x.InstanceId == instanceId))
                return _equipment.Single(x => x.InstanceId == instanceId) as ModAccessory;

            var newId = _nextFreeInstanceId;
            var inst = Activator.CreateInstance(Masquerade.Api.GetModAccessory(contentId).GetType()) as ModAccessory;
            if (inst == null)
            {
                Masquerade.Logger.Error($"Unable to create instance of accessory {contentId}");
                return null;
            }
            inst.InstanceId = newId;
            _equipment.Add(inst);
            _nextFreeInstanceId++;
            return inst;
        }
    }
}
