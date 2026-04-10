using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Algorithm;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Signals;
using Masquerade.Models;
using Masquerade.Util;
using UnityEngine;

namespace Masquerade.Patches
{
    public class AccessoriesFacadePatches : ClassPatcher<AccessoriesFacade>
    {
        public static bool PreOnAdded(AccessoriesFacade __instance, WeaponType accessoryType, CharacterController characterController, bool removeFromStore = true)
        {
            if (!Masquerade.Api.ModdedEquipmentCheck(accessoryType))
                return true;

            if (CheckLevelUp(accessoryType, __instance, characterController, removeFromStore)) return false;
            var accessory = InstatiateAccessory(accessoryType, __instance, characterController, removeFromStore);
            if (accessory == null) return false;
            PostInstatiation(accessory, __instance, characterController, removeFromStore);

            return false;
        }

        public static void PreOnRemoved(AccessoriesFacade __instance, WeaponType accessoryType, CharacterController characterController, bool notifyRemove = true)
        {
            if (!Masquerade.Api.ModdedEquipmentCheck(accessoryType))
                return;

            RemoveModdedInstances(accessoryType, characterController);
        }

        public override IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            var ints = new List<PatchInstruction>()
            {
                new PatchInstruction(TargetClass, nameof(AccessoriesFacade.AddAccessory), typeof(AccessoriesFacadePatches).GetMethod(nameof(PreOnAdded)), prefix: true),
                new PatchInstruction(TargetClass, nameof(AccessoriesFacade.RemoveAccessory), typeof(AccessoriesFacadePatches).GetMethod(nameof(PreOnRemoved)), prefix: true),
            };
            return ints;
        }

        private static void CheckArcanas(AccessoriesFacade facade)
        {
            facade._arcanaManager.CheckSilent();
        }

        private static bool CheckLevelUp(WeaponType accessoryType, AccessoriesFacade facade, CharacterController characterController, bool remove)
        {
            Accessory accessoryByType = characterController.AccessoriesManager.GetAccessoryByType(accessoryType);
            if (accessoryByType == null || accessoryByType.CurrentAccessoryData.allowDuplicates)
                return false;

            if (!accessoryByType.LevelUp())
                facade._playerOptions.AddCoins(10f, characterController);
            if (remove)
                facade._levelUpFactory.RemoveFromStore(accessoryType, characterController);
            return true;
        }

        private static void CreateModdedInstances(WeaponType accessoryType, CharacterController characterController)
        {
            var container = Masquerade.Api.GetOrAddCharacterInstance(characterController);
            if (container == null)
            {
                LoggerHelper.Logger.Error("Character container failed to load!");
                return;
            }

            var instance = Masquerade.Api.GetOrAddModAccessoryInstance((int)accessoryType, container, characterController);

            instance.OnAccessoryAdded(characterController);
        }

        private static Accessory InstatiateAccessory(WeaponType accessoryType, AccessoriesFacade facade, CharacterController characterController, bool remove)
        {
            Accessory accessoryPrefab = facade._accessoriesFactory.GetAccessoryPrefab(accessoryType);
            if (accessoryPrefab == null)
            {
                LoggerHelper.Logger.Error($"Prefab for accessory {accessoryType} not found!");
                return null;
            }
            Accessory component = UnityEngine.Object.Instantiate(facade._accessoriesFactory.GetAccessoryPrefab(accessoryType), characterController.transform.position, Quaternion.identity, characterController.transform)
                .GetComponent<Accessory>();
            component.Init(characterController, accessoryType);

            characterController.AccessoriesManager.AddEquipment(component);
            CreateModdedInstances(accessoryType, characterController);

            facade._signalBus.Fire(new GameplaySignals.AccessoryAddedToCharacterSignal
            {
                Character = characterController,
                Accessory = accessoryType,
                Data = component.CurrentAccessoryData
            });
            return component;
        }

        private static void PostInstatiation(Accessory accessory, AccessoriesFacade facade, CharacterController characterController, bool removeFromStore = true)
        {
            if (removeFromStore)
                facade._levelUpFactory.RemoveFromStore(accessory.Type, characterController);
            else
                facade._levelUpFactory.CalculateWeights(characterController);

            if (characterController.IsFollower)
            {
                CharacterADControl deficiencyControl = characterController.DeficiencyControl;
                if (deficiencyControl == null || deficiencyControl.LevelupType != LevelupType.ManualSelection)
                    return;
            }
            CheckArcanas(facade);
        }

        private static void RemoveModdedInstances(WeaponType accessoryType, CharacterController characterController)
        {
            var container = Masquerade.Api.GetOrAddCharacterInstance(characterController);
            if (container == null)
            {
                LoggerHelper.Logger.Error("Character container failed to load!");
                return;
            }
            Masquerade.Api.GetOrAddModAccessoryInstance((int)accessoryType, container, characterController).OnAccessoryRemoved(characterController);
            Masquerade.Api.DeleteEquipmentInstance((int)accessoryType, container);
        }
    }
}