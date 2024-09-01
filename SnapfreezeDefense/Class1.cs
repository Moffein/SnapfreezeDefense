using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using SnapfreezeDefense.Components;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SnapfreezeDefense
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.Moffein.SnapfreezeDefense", "SnapfreezeDefense", "1.0.1")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(PrefabAPI), nameof(ContentAddition))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class SnapfreezeDefense : BaseUnityPlugin
    {
        public static GameObject modifiedIceWallPillarProjectile;
        public static GameObject modifiedIceWallWalkerProjectile;

        public void Awake()
        {
            BuildProjectile();
        }

        private void BuildProjectile()
        {
            modifiedIceWallPillarProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageIcewallPillarProjectile.prefab").WaitForCompletion().InstantiateClone("StandaloneRiskyModIceWallPillarProjectile", true);
            modifiedIceWallPillarProjectile.AddComponent<IceWallDefenseComponent>();
            ContentAddition.AddProjectile(modifiedIceWallPillarProjectile);

            modifiedIceWallWalkerProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageIcewallWalkerProjectile.prefab").WaitForCompletion().InstantiateClone("StandaloneRiskyModIceWallWalkerProjectile", true);
            ProjectileMageFirewallWalkerController walker = modifiedIceWallWalkerProjectile.GetComponent<ProjectileMageFirewallWalkerController>();
            if (walker) walker.firePillarPrefab = modifiedIceWallPillarProjectile;
            ContentAddition.AddProjectile(modifiedIceWallWalkerProjectile);

            SetEntityStateField("EntityStates.Mage.Weapon.PrepWall", "projectilePrefab", modifiedIceWallWalkerProjectile);

            //Build deletion effect
            GameObject deletionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXFrozen.prefab").WaitForCompletion().InstantiateClone("StandaloneRiskyModIceWallDeletionEffect", false);
            EffectComponent ec = deletionEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_captain_drone_zap";

            ContentAddition.AddEffect(deletionEffect);
            IceWallDefenseComponent.projectileDeletionEffectPrefab = deletionEffect;
        }

        private bool SetEntityStateField(string entityStateName, string fieldName, UnityEngine.Object newObject)
        {
            EntityStateConfiguration esc = LegacyResourcesAPI.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue = newObject;
                    return true;
                }
            }
            return false;
        }
    }
}
