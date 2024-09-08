using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Mage.Components
{
    public class IceWallDefenseComponent : MonoBehaviour
    {
        public static GameObject projectileDeletionEffectPrefab;

        private GameObject matrixObject;
        private DefenseMatrixManager.DefenseMatrixManager.DefenseMatrixInfo defenseMatrixInfo;
        private BoxCollider collider;
        private TeamFilter tf;

        //Do this in Start because TeamFilter might not be initialized in Awake
        public void Start()
        {
            if (!matrixObject)
            {
                tf = base.GetComponent<TeamFilter>();
                if (tf && tf.teamIndex != TeamIndex.None && base.transform)
                {
                    GameObject toInstantiate = new GameObject
                    {
                        layer = LayerIndex.entityPrecise.intVal
                    };
                    BoxCollider bc = toInstantiate.AddComponent<BoxCollider>();
                    bc.size = new Vector3(2.5f, 2.5f, 7f);
                    bc.enabled = false;

                    toInstantiate.transform.localPosition = base.transform.localPosition;
                    toInstantiate.transform.localRotation = base.transform.localRotation;
                    toInstantiate.transform.localScale = base.transform.localScale;

                    matrixObject = toInstantiate;
                    collider = matrixObject.GetComponent<BoxCollider>();
                    if (collider)
                    {
                        defenseMatrixInfo = new DefenseMatrixManager.DefenseMatrixManager.DefenseMatrixInfo(new Collider[] { collider }, tf.teamIndex);
                        DefenseMatrixManager.DefenseMatrixManager.AddMatrix(defenseMatrixInfo);
                    }
                }
            }
        }

        //This will handle the projectile deletion part
        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (tf && collider)
                {
                    List<ProjectileController> deletionList = new List<ProjectileController>();

                    Vector3 sizeVector = collider.size;

                    Collider[] colliders = Physics.OverlapBox(collider.transform.position, sizeVector, collider.transform.rotation, LayerIndex.projectile.mask);
                    foreach (Collider c in colliders)
                    {
                        ProjectileController pc = c.GetComponentInParent<ProjectileController>();
                        if (pc && !pc.cannotBeDeleted)
                        {
                            if (!(pc.teamFilter && pc.teamFilter.teamIndex == tf.teamIndex))
                            {
                                bool cannotDelete = false;
                                ProjectileSimple ps = pc.gameObject.GetComponent<ProjectileSimple>();
                                ProjectileCharacterController pcc = pc.gameObject.GetComponent<ProjectileCharacterController>();

                                if ((!ps || (ps && ps.desiredForwardSpeed == 0f)) && !pcc)
                                {
                                    cannotDelete = true;
                                }

                                if (!cannotDelete)
                                {
                                    deletionList.Add(pc);
                                }
                            }
                        }
                    }

                    int projectilesDeleted = deletionList.Count;
                    for (int i = 0; i < projectilesDeleted; i++)
                    {
                        GameObject toDelete = deletionList[i].gameObject;
                        if (toDelete)
                        {
                            if (toDelete.transform && projectileDeletionEffectPrefab) EffectManager.SimpleEffect(projectileDeletionEffectPrefab, toDelete.transform.position, default, true);
                            Destroy(toDelete);
                        }
                    }
                }
            }
        }

        public void OnDestroy()
        {
            if (defenseMatrixInfo != null) DefenseMatrixManager.DefenseMatrixManager.RemoveMatrix(defenseMatrixInfo);
            if (matrixObject) Destroy(matrixObject);
        }
    }
}
