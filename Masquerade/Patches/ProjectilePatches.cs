using HarmonyLib;
using Il2CppVampireSurvivors.Objects.Pools;
using Il2CppVampireSurvivors.Objects.Projectiles;
using Il2CppVampireSurvivors.Objects.Weapons;
using MelonLoader;
using System.Reflection;
using Masquerade.Models;
using Masquerade.Models.Projectiles;
using Masquerade.Util;

namespace Masquerade.Patches
{
    /// <summary>
    /// Harmony patches for the Projectile class
    /// </summary>
    public static class ProjectilePatches
    {
        /// <summary>
        /// The methods to be patched
        /// </summary>
        public static MethodInfo[] Methods;

        /// <summary>
        /// Managed array to hold all of the ModProjectile instances created, sorted by their WeaponType starting from when custom content starts.
        /// Second dimension represents their given IndexInWeapon.
        /// </summary>
        /// <remarks>
        /// This could definitely be done better. Pooling is the way to go, but type actually needs pooled needs figured out.
        /// </remarks>
        public static ModProjectile[,] ModPool;

        /// <summary>
        /// An array corresponding to every WeaponType both vanilla and custom.
        /// Holds whether or not a given WeaponType is custom in addition to whether its already been checked.
        /// </summary>
        public static byte[] ModWeaponType;

        public static void Deinitialize()
        {
            Methods = null;
            ModWeaponType = null;
        }

        /// <summary>
        /// Injected method to destroy the ModProjectile corresponding to the projectile being despawned
        /// </summary>
        public static void DespawnPrefix(Projectile __instance)
        {
            if (!SafeProjectileHasParent(__instance, out Weapon weapon)) return;
            if (!IsProjectileModded(weapon)) return;
            ModPool[ModProjectileIdFromWeaponType(weapon), __instance.IndexInWeapon] = null;
        }

        public static void Initialize()
        {
            Methods = TargetMethods();
            ModWeaponType = new byte[3000];
        }

        /// <summary>
        /// Injected method to handle the Projectile.InitProjectile function.
        /// </summary>
        /// <returns>True to skip mod behavior. False to skip vanilla behavior.</returns>
        public static bool InitPrefix(Projectile __instance, Weapon weapon, BulletPool pool, int index)
        {
            AssignAndCheckModdedStatus(weapon);
            if (!IsProjectileModded(weapon)) return true;

            var modIndex = ModProjectileIdFromWeaponType(weapon);
            var modWeapon = ModPool[modIndex, index];
            if (modWeapon == null) modWeapon = new ModKnifeProjectile();

            try
            {
                modWeapon.InitProjectile(ref __instance, pool, weapon, index);
                return false;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex.Message);
                return true;
            }
            finally
            {
                ModPool[modIndex, index] = modWeapon;
            }
        }

        /// <summary>
        /// Placeholder injected method to stop AxeProjectile from firing its InternalUpdate.
        /// </summary>
        public static bool InternalUpdatePrefix(AxeProjectile __instance)
        {
            if (!SafeProjectileHasParent(__instance, out Weapon weapon)) return true;
            if (!IsProjectileModded(weapon)) return true;
            return false;
        }

        public static MethodInfo[] TargetMethods()
        {
            var arr = new MethodInfo[16];
            var methods = AccessTools.GetDeclaredMethods(typeof(Projectile));
            var onlyTypes = methods.Where(x => x.DeclaringType.Name == nameof(Projectile));
            arr[0] = onlyTypes.Single(x => x.Name.Equals(nameof(Projectile.InitProjectile)));
            arr[1] = onlyTypes.Single(x => x.Name.Equals(nameof(Projectile.Despawn)));
            var methtwo = AccessTools.GetDeclaredMethods(typeof(AxeProjectile)).Where(x => x.DeclaringType.Name == nameof(AxeProjectile));
            arr[2] = methtwo.Single(x => x.Name.Equals(nameof(AxeProjectile.InternalUpdate)));
            return arr;
        }

        /// <summary>
        /// Assign weapon a 1 or a 2 and stores that.
        /// 1 represents a vanilla weapon. 2 represents a modded one.
        /// </summary>
        private static void AssignAndCheckModdedStatus(Weapon weapon)
        {
            if (ModWeaponType[(int)weapon.Type] != 0) return;

            if (!vsMLCore.CustomWeapons.Any(x => x.IdAsType.Equals(weapon.Type))) ModWeaponType[(int)weapon.Type] = 1;
            else ModWeaponType[(int)weapon.Type] = 2;
        }

        private static bool IsProjectileModded(Weapon parent) => ModWeaponType[(int)parent.Type] >= 2;

        private static int ModProjectileIdFromWeaponType(Weapon projectileParent) => (int)projectileParent.Type - Constants.WEAPON_START_ID;

        private static bool SafeProjectileHasParent(Projectile __instance, out Weapon weapon)
        {
            weapon = null;
            if (ModPool == null) return false;
            weapon = SafeAccess.GetProperty<Weapon>(__instance, nameof(__instance.Weapon));
            return (weapon != null);
        }
    }
}