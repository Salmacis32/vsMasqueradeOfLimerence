using HarmonyLib;
using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Algorithm;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppVampireSurvivors.Signals;
using Masquerade.Api;
using Masquerade.Models;
using UnityEngine;

namespace Masquerade.Patches
{
    public class AccessoriesFacadePatches : IClassPatcher
    {
        public Type TargetClass => typeof(AccessoriesFacade);

        public static bool PreOnAdded(AccessoriesFacade __instance, WeaponType accessoryType, CharacterController characterController, bool removeFromStore = true)
        {
            if (!MasqueradeApi.ModdedCheck(accessoryType))
            {
                return true;
            }
            AddAccessory(__instance, accessoryType, characterController, removeFromStore);
            return false;
        }

        public static bool PreOnRemoved(AccessoriesFacade __instance, WeaponType accessoryType, CharacterController characterController, bool notifyRemove = true)
        {
            if (!MasqueradeApi.ModdedCheck(accessoryType))
            {
                return true;
            }
            RemoveAccessory(__instance, accessoryType, characterController, notifyRemove);
            return false;
        }

        public IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            PatchInstruction AddInstruction = new(TargetClass, nameof(AccessoriesFacade.AddAccessory), typeof(AccessoriesFacadePatches).GetMethod(nameof(PreOnAdded)), prefix: true);
            var ints = new List<PatchInstruction>()
            {
                AddInstruction,
                new PatchInstruction(TargetClass, nameof(AccessoriesFacade.RemoveAccessory), typeof(AccessoriesFacadePatches).GetMethod(nameof(PreOnRemoved)), prefix: true)
            };
            return ints;
        }

        private static void AddAccessory(AccessoriesFacade facade, WeaponType accessoryType, CharacterController characterController, bool removeFromStore = true)
        {
            Accessory accessoryByType = characterController.AccessoriesManager.GetAccessoryByType(accessoryType, false);
            if (accessoryByType != null)
            {
                CheckLevelUp(facade, accessoryType, characterController, removeFromStore, accessoryByType);
                return;
            }

            Accessory accessoryPrefab = facade._accessoriesFactory.GetAccessoryPrefab(accessoryType);
            if (accessoryPrefab == null)
            {
                Masquerade.Logger.Error(string.Format("Accessory prefab is NULL for type {0}. Likely not generated a prefab for this yet...", accessoryType));
                return;
            }

            Transform parent = characterController.transform;
            Accessory component = UnityEngine.Object.Instantiate(accessoryPrefab, parent.position, Quaternion.identity, parent).GetComponent<Accessory>();
            component.Init(characterController, accessoryType);
            characterController.AccessoriesManager.AddEquipment(component);

            if (!CreateModdedInstances(accessoryType, characterController))
                return;

            facade._signalBus.Fire(new GameplaySignals.AccessoryAddedToCharacterSignal
            {
                Character = characterController,
                Accessory = accessoryType,
                Data = component.CurrentAccessoryData
            });
            if (removeFromStore)
            {
                facade._levelUpFactory.RemoveFromStore(component.Type, characterController);
            }
            else
            {
                facade._levelUpFactory.CalculateWeights(characterController);
            }
            if (characterController.IsFollower)
            {
                CharacterADControl deficiencyControl = characterController.DeficiencyControl;
                if (deficiencyControl == null || deficiencyControl.LevelupType != LevelupType.ManualSelection)
                {
                    return;
                }
            }
            facade._arcanaManager.CheckSilent();
        }

        private static void CheckLevelUp(AccessoriesFacade facade, WeaponType accessoryType, CharacterController characterController, bool removeFromStore, Accessory accessoryByType)
        {
            if (!accessoryByType.LevelUp())
                facade._playerOptions.AddCoins(10f, characterController);

            if (removeFromStore)
                facade._levelUpFactory.RemoveFromStore(accessoryType, characterController);
        }

        private static bool CreateModdedInstances(WeaponType accessoryType, CharacterController characterController)
        {
            var charComponent = characterController.gameObject.GetOrAddComponent<GlobalInstanceComponent>();
            var container = Masquerade.Api.GetOrAddCharacterInstance(characterController, charComponent);
            if (container == null)
            {
                Masquerade.Logger.Error("Character container failed to load!");
                return false;
            }

            var instance = Masquerade.Api.GetOrAddModAccessoryInstance((int)accessoryType, container, characterController);

            instance.OnAccessoryAdded(container);
            return true;
        }

        private static void RemoveAccessory(AccessoriesFacade facade, WeaponType accessoryType, CharacterController characterController, bool notifyRemove = true)
        {
            if (characterController == null)
            {
                return;
            }
            Accessory accessoryByType = characterController.AccessoriesManager.GetAccessoryByType(accessoryType, false);
            if (!accessoryByType)
            {
                return;
            }
            if (notifyRemove)
            {
                characterController.AccessoriesManager.RemoveEquipment(accessoryByType);
            }
            accessoryByType.OnAccessoryRemovedFromEquipment();

            if (!characterController.gameObject.TryGetComponent<GlobalInstanceComponent>(out var charComponent))
            {
                Masquerade.Logger.Warning($"Accessory {(int)accessoryType} has no GlobalInstanceComponent!");
                characterController.gameObject.AddComponent<GlobalInstanceComponent>();
            }
            var container = Masquerade.Api.GetOrAddCharacterInstance(characterController, charComponent);
            if (container == null)
            {
                Masquerade.Logger.Error("Character container failed to load!");
                return;
            }
            Masquerade.Api.GetOrAddModAccessoryInstance((int)accessoryType, container, characterController).OnAccessoryRemoved(container);
            Masquerade.Api.DeleteEquipmentInstance((int)accessoryType, container);

            accessoryByType.Cleanup();
            if (notifyRemove)
            {
                facade._signalBus.Fire(new GameplaySignals.AccessoryRemovedFromCharacterSignal
                {
                    Character = characterController,
                    Accessory = accessoryType
                });
            }
        }
    }
}