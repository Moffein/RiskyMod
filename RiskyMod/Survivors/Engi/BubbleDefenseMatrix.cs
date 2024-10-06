using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Engi
{
    public class BubbleDefenseMatrix
    {
        public static bool enabled = true;

        public BubbleDefenseMatrix()
        {
            if (!enabled) return;

            On.EntityStates.Engi.EngiBubbleShield.Deployed.FixedUpdate += Deployed_FixedUpdate;
            On.EntityStates.Engi.EngiBubbleShield.Deployed.OnExit += Deployed_OnExit;

            GameObject effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiGrenadeExplosion.prefab").WaitForCompletion().InstantiateClone("RiskyModBubbleShieldDeletionEffect", false);
            effect.GetComponent<EffectComponent>().soundName = "Play_captain_drone_zap";
            Content.Content.effectDefs.Add(new EffectDef(effect));
            BubbleShieldDefenseComponent.deletionEffectPrefab = effect;
        }

        private void Deployed_OnExit(On.EntityStates.Engi.EngiBubbleShield.Deployed.orig_OnExit orig, EntityStates.Engi.EngiBubbleShield.Deployed self)
        {
            orig(self);
            BubbleShieldDefenseComponent dmr = self.GetComponent<BubbleShieldDefenseComponent>();
            if (dmr && dmr.info != null)
            {
                DefenseMatrixManager.DefenseMatrixManager.RemoveMatrix(dmr.info);
            }
        }

        private void Deployed_FixedUpdate(On.EntityStates.Engi.EngiBubbleShield.Deployed.orig_FixedUpdate orig, EntityStates.Engi.EngiBubbleShield.Deployed self)
        {
            bool wasDeployed = self.hasDeployed;
            orig(self);
            if (!wasDeployed && self.hasDeployed)
            {
                TeamFilter tf = self.GetComponent<TeamFilter>();
                if (tf)
                {
                    ChildLocator cl = self.GetComponent<ChildLocator>();
                    if (cl)
                    {
                        BubbleShieldDefenseComponent dmr = self.GetComponent<BubbleShieldDefenseComponent>();
                        if (!dmr) dmr = self.gameObject.AddComponent<BubbleShieldDefenseComponent>();
                        dmr.teamIndex = tf.teamIndex;

                        GameObject bubbleObject = cl.FindChild(EntityStates.Engi.EngiBubbleShield.Deployed.childLocatorString).gameObject;

                        Collider[] colliders = bubbleObject.GetComponentsInChildren<Collider>();
                        foreach (var collider in colliders)
                        {
                            collider.gameObject.layer = LayerIndex.entityPrecise.intVal;
                            collider.enabled = false;
                        }

                        SphereCollider[] sc = bubbleObject.GetComponentsInChildren<SphereCollider>();
                        foreach (var s in sc)
                        {

                            if (s.radius > dmr.radius)
                            {
                                dmr.radius = s.radius;
                                dmr.colliderOrigin = s.transform.position;
                            }
                        }

                        dmr.info = new DefenseMatrixManager.DefenseMatrixManager.DefenseMatrixInfo(colliders, tf.teamIndex);
                        DefenseMatrixManager.DefenseMatrixManager.AddMatrix(dmr.info);
                    }
                }
            }
        }

        public class BubbleShieldDefenseComponent : MonoBehaviour
        {
            public static GameObject deletionEffectPrefab;
            public DefenseMatrixManager.DefenseMatrixManager.DefenseMatrixInfo info;
            public float radius = 0f;
            public TeamIndex teamIndex;
            public Vector3 colliderOrigin;

            private void FixedUpdate()
            {
                DeleteProjectilesServer();
            }

            private void DeleteProjectilesServer()
            {
                if (!NetworkServer.active || radius <= 0f) return;
                List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
                List<ProjectileController> deleteList = new List<ProjectileController>();

                float radiusSqr = radius * radius;

                foreach (ProjectileController pc in instancesList)
                {
                    if (pc.cannotBeDeleted || pc.teamFilter.teamIndex == teamIndex || (pc.transform.position - colliderOrigin).sqrMagnitude > radiusSqr) continue;
                    ProjectileSimple ps = pc.gameObject.GetComponent<ProjectileSimple>();
                    ProjectileCharacterController pcc = pc.gameObject.GetComponent<ProjectileCharacterController>();
                    if ((!ps || (ps && ps.desiredForwardSpeed == 0f)) && !pcc) continue;

                    deleteList.Add(pc);
                }

                foreach (ProjectileController pc in deleteList)
                {
                    EffectManager.SimpleEffect(deletionEffectPrefab, pc.transform.position, pc.transform.rotation, true);
                    UnityEngine.Object.Destroy(pc.gameObject);
                }
            }
        }
    }
}
