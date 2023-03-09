using RoR2;
using UnityEngine;
using System.Collections.ObjectModel;
using TILER2;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using RoR2.Projectile;

namespace CRA_Factory {
    static class PrefabAssembler {
        static public GameObject DelayedImpactEffect;

        static public void Init() {
            init_DelayedImpactEffect();
        }

        static public void init_DelayedImpactEffect() {
            DelayedImpactEffect = CRAPlugin.resources.LoadAsset<GameObject>("Assets/Prefabs/delayed_impact_effect.prefab");

            ProjectileController projectileController = DelayedImpactEffect.AddComponent<ProjectileController>();
            ProjectileDamage projectileDamage = DelayedImpactEffect.AddComponent<ProjectileDamage>();
            ProjectileExplosion projectileExplosion = DelayedImpactEffect.AddComponent<ProjectileExplosion>();

            projectileExplosion.blastRadius = 1;

            projectileDamage.crit = true;
            projectileDamage.damageType = DamageType.AOE;
        }
    }
}
