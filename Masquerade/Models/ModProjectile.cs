using Il2Cpp;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Pools;
using Il2CppVampireSurvivors.Objects.Projectiles;
using Il2CppVampireSurvivors.Objects.Weapons;
using Masquerade.Util;
using UnityEngine;

namespace Masquerade.Models
{
    /// <summary>
    /// Base class for custom projectile logic.
    /// </summary>
    /// <remarks>
    /// This is mostly placeholder. I wanted a proof of concept to make sure this would work before going with a better pattern.
    /// The plan is to adapt this into a house for projectile behaviors in a decorator pattern.
    /// </remarks>
    public class ModProjectile
    {
        /// <summary>
        /// As close to a one-to-one recreation of the Projectile.InitProjectile method as I could manage.
        /// </summary>
        /// <param name="proj">Projectile instance that's being hooked</param>
        /// <param name="pool">The new bullet pool. Hooked method argument</param>
        /// <param name="weapon">The weapon initializing the projectile. Hooked method argument</param>
        /// <param name="index">The projectile's indexInWeapon to be assigned. Hooked method argument</param>
        /// <remarks>
        /// I'm weary of keeping this in as it is, given it is technically source code. Eventually I will piecemeal this into behaviors for the decorator pattern.
        /// </remarks>
        public virtual void InitProjectile(ref Projectile proj, BulletPool pool, Weapon weapon, int index)
        {
            proj._gameSessionData = GM.Core.GameSessionData;
            proj._pool = pool;
            proj._weapon = weapon;

            // Converts the ObjectsHit hashset into a managed type and clears it. (Unsure if this is what the original does)
            var objectsHit = proj._objectsHit.ToHashSet();
            if (objectsHit.Count > 0)
            {
                objectsHit.Clear();
            }
            proj._objectsHit = objectsHit.ToIl2CppHashSet();
            proj._indexInWeapon = index;

            proj._penetrating = proj._weapon.Penetrating;
            proj._bounces = proj._weapon.PBounces();

            // This is how the source creates a body for the object. This has something to do with Phaser porting and is necessary.
            if (proj.body == null)
            {
                ArcadePhysics.s_scene.add._world.enableBody(proj, PhysicsType.DYNAMIC_BODY);
            }
            proj.body._enable = true;

            // Adds the projectile to the current instance's PhysicsManager to keep track of physics stuff, still needs more investigation.
            PhysicsManager._sInstance._bulletGroup?.add(proj._sprite);
            proj._spriteTrail?.Reset();

            // Adds this projectile to the weapon's SpawnedProjectiles list.
            var spawnedProj = weapon._spawnedProjectiles.ToSystemList();
            if (spawnedProj.Count == 0 || !spawnedProj.Contains(proj))
            {
                spawnedProj.Add(proj);
            }
            weapon._spawnedProjectiles = spawnedProj.ToIl2CppList();

            // Registers the projectile into the GameManager's ParticleManager.
            GM.Core.ParticleManager.RegisterParticleSystem(proj.GetComponentInChildren<ParticleSystem>());
        }
    }
}
