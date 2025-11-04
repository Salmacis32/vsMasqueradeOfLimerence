using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Weapons;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Pools;
using Il2CppVampireSurvivors.Objects.Projectiles;
using Il2CppVampireSurvivors.Objects.Weapons;
using Masquerade.Util;
using UnityEngine;

namespace Masquerade.Models.Projectiles
{
    /// <summary>
    /// My interpretation of the KnifeProjectile code.
    /// </summary>
    public class ModKnifeProjectile : ModProjectile
    {
        public static Il2CppSystem.Nullable<float> Volume;
        public Il2CppSystem.Nullable<float> Empty;

        /// <summary>
        /// As close to a one-to-one recreation of the KnifeProjectile InitProjectile method as I could manage.
        /// </summary>
        /// <param name="proj">Projectile instance that's being hooked</param>
        /// <param name="pool">Hooked method argument</param>
        /// <param name="weapon">Hooked method argument</param>
        /// <param name="index">Hooked method argument</param>
        public override void InitProjectile(ref Projectile proj, BulletPool pool, Weapon weapon, int index)
        {
            base.InitProjectile(ref proj, pool, weapon, index);
            if (Empty == null) Empty = new Il2CppSystem.Nullable<float>();
            proj.body.setCircle(radius: 8.0f, offsetX: Empty, offsetY: Empty, worldSpace: false);
            proj._speed = 2.0f;
            proj.SetScaleToArea();

            Vector3 ret = proj.CachedTrans.position;
            float randomX = UnityEngine.Random.value; float randomY = UnityEngine.Random.value;
            float offset = (proj.IndexInWeapon == 0) ? 0.0f : 0.4f;
            float area = weapon.PArea();
            ret.Set(area * (randomX - 0.5f) * offset + ret.x, ret.y - area * (randomY - 0.5f) * offset, ret.z);
            proj.CachedTrans.position = ret;

            if (!proj._weapon.IsHoming)
            {
                proj.ApplyPlayerFacingVelocity(weapon.Owner.LastMovementDirection);
            }
            else
            {
                proj.AimForNearestEnemy();
            }

            if (Volume == null)
            {
                Volume = SafeAccess.GetProperty(weapon._currentWeaponData, nameof(WeaponData._volume_k__BackingField), Empty);
            }
            if (!Volume.HasValue) Volume.value = 0.4f;
            SoundManager.PlaySound(SfxType.Shot, new SoundManager.SoundConfig() { Volume = Volume, Detune = proj._indexInWeapon * -100, Rate = 1.0f }, durationMillis: 200.0f);

            proj._weapon.CritIndex += 1;
            var mod = proj._weapon.CritIndex % weapon.CritChancesArray._size;

            if (0.5f <= weapon.CritChancesArray[mod + 1]) proj._bounces = weapon.PBounces();
        }
    }
}
