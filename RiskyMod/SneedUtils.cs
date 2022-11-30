using BepInEx.Configuration;
using RiskyMod.Content;
using RoR2;
using RoR2.Navigation;
using RoR2.Skills;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SneedUtils
{
    public class SneedUtils
    {
        public static bool ReplaceSkillDef(SkillFamily skillFamily, SkillDef targetSkill, SkillDef newSkill)
        {
            bool successfullyReplaced = false;

            SkillFamily.Variant[] skillVariants = skillFamily.variants;
            if (skillVariants != null && targetSkill != null && newSkill != null)
            {
                for (int i = 0; i < skillVariants.Length; i++)
                {
                    SkillFamily.Variant variant = skillVariants[i];
                    if (variant.skillDef == targetSkill)
                    {
                        variant.skillDef = newSkill;
                        successfullyReplaced = true;
                        break;
                    }
                }
            }

            if (!successfullyReplaced)
            {
                Debug.LogError("RiskyMod: Could not replace TargetSkill " + targetSkill);
            }
            return successfullyReplaced;
        }

        //If Inferno Compat is disabled, or if Inferno isn't active, allow the enemy change to be run.
        public static bool ShouldRunInfernoChange()
        {
            return !RiskyMod.Enemies.EnemiesCore.infernoCompat || !CheckInfernoActive();
        }

        public static bool CheckInfernoActive()
        {
            bool infernoActive = false;
            if (RiskyMod.SoftDependencies.InfernoPluginLoaded) infernoActive = CheckInfernoActiveInternal();
            return infernoActive;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool CheckInfernoActiveInternal()
        {
            return Run.instance != null && Run.instance.selectedDifficulty == Inferno.Main.InfernoDiffIndex;
        }

        //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }

        public static Vector3 FindSafeTeleportPosition(GameObject gameObject, Vector3 targetPosition)
        {
            return FindSafeTeleportPosition(gameObject, targetPosition, float.NegativeInfinity, float.NegativeInfinity);
        }

        public static Vector3 FindSafeTeleportPosition(GameObject gameObject, Vector3 targetPosition, float idealMinDistance, float idealMaxDistance)
        {
            Vector3 vector = targetPosition;
            SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
            spawnCard.hullSize = HullClassification.Human;
            spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            spawnCard.prefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
            Vector3 result = vector;
            GameObject teleportGameObject = null;
            if (idealMaxDistance > 0f && idealMinDistance < idealMaxDistance)
            {
                teleportGameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                    minDistance = idealMinDistance,
                    maxDistance = idealMaxDistance,
                    position = vector
                }, RoR2Application.rng));
            }
            if (!teleportGameObject)
            {
                teleportGameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                    position = vector
                }, RoR2Application.rng));
                if (teleportGameObject)
                {
                    result = teleportGameObject.transform.position;
                }
            }
            if (teleportGameObject)
            {
                UnityEngine.Object.Destroy(teleportGameObject);
            }
            UnityEngine.Object.Destroy(spawnCard);
            return result;
        }

        public static void StunEnemiesInSphere(CharacterBody body, float radius)
        {
            if (body && body.teamComponent)
            {
                List<HealthComponent> hcList = new List<HealthComponent>();
                Collider[] array = Physics.OverlapSphere(body.corePosition, radius, LayerIndex.entityPrecise.mask);
                for (int i = 0; i < array.Length; i++)
                {
                    HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                    if (hurtBox && hurtBox.healthComponent && !hcList.Contains(hurtBox.healthComponent))
                    {
                        hcList.Add(hurtBox.healthComponent);
                        if (hurtBox.healthComponent.body.teamComponent && hurtBox.healthComponent.body.teamComponent.teamIndex != body.teamComponent.teamIndex)
                        {
                            SetStateOnHurt ssoh = hurtBox.healthComponent.gameObject.GetComponent<SetStateOnHurt>();
                            if (ssoh && ssoh.canBeStunned)
                            {
                                ssoh.SetStun(1f);
                            }
                        }
                    }
                }
            }
        }

        public static void FixSkillName(SkillDef skillDef)
        {
            (skillDef as UnityEngine.Object).name =skillDef.skillName;// "RiskyMod_" + 
        }

        public static BuffDef CreateBuffDef(string name, bool canStack, bool isCooldown, bool isDebuff, Color color, Sprite iconSprite)
        {
            BuffDef bd = ScriptableObject.CreateInstance<BuffDef>();
            bd.name = name;
            bd.canStack = canStack;
            bd.isCooldown = isCooldown;
            bd.isDebuff = isDebuff;
            bd.buffColor = color;
            bd.iconSprite = iconSprite;
            Content.buffDefs.Add(bd);

            (bd as UnityEngine.Object).name = bd.name;
            return bd;
        }

        public static bool IsLocalUser(GameObject playerObject)
        {
            foreach (LocalUser user in LocalUserManager.readOnlyLocalUsersList)
            {
                if (playerObject == user.cachedBodyObject)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsLocalUser(CharacterBody playerBody)
        {
            foreach (LocalUser user in LocalUserManager.readOnlyLocalUsersList)
            {
                if (playerBody == user.cachedBody)
                {
                    return true;
                }
            }

            return false;
        }

        public static int FindEnemiesInSphere(float radius, Vector3 position, TeamIndex team, bool airborneOnly = false)
        {
            int hitCount = 0;
            List<HealthComponent> hcList = new List<HealthComponent>();
            Collider[] array = Physics.OverlapSphere(position, radius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    if (healthComponent && !hcList.Contains(healthComponent))
                    {
                        hcList.Add(healthComponent);
                        if (healthComponent.body && healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != team)
                        {
                            if (!airborneOnly ||
                                (healthComponent.body.isFlying ||
                                (healthComponent.body.characterMotor && !healthComponent.body.characterMotor.isGrounded)))
                            {
                                hitCount++;
                            }
                        }
                    }
                }
            }
            return hitCount;
        }

        public static bool IsEnemyInSphere(float radius, Vector3 position, TeamIndex team, bool airborneOnly = false)
        {
            List<HealthComponent> hcList = new List<HealthComponent>();
            Collider[] array = Physics.OverlapSphere(position, radius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    if (healthComponent && !hcList.Contains(healthComponent))
                    {
                        hcList.Add(healthComponent);
                        if (healthComponent.body && healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != team)
                        {
                            if (!airborneOnly ||
                                (healthComponent.body.isFlying ||
                                (healthComponent.body.characterMotor && !healthComponent.body.characterMotor.isGrounded)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static void RemoveItemTag(ItemDef itemDef, ItemTag tag)
        {
            if (itemDef.ContainsTag(tag))
            {
                List<ItemTag> tags = itemDef.tags.ToList();
                tags.Remove(tag);
                itemDef.tags = tags.ToArray();

                ItemIndex index = itemDef.itemIndex;
                if (index != ItemIndex.None && ItemCatalog.itemIndicesByTag != null && ItemCatalog.itemIndicesByTag[(int)tag] != null)
                {
                    List<ItemIndex> itemList = ItemCatalog.itemIndicesByTag[(int)tag].ToList();
                    if (itemList.Contains(index))
                    {
                        itemList.Remove(index);
                        ItemCatalog.itemIndicesByTag[(int)tag] = itemList.ToArray();
                    }
                }
            }
        }

        public static void AddItemTag(ItemDef itemDef, ItemTag tag)
        {
            if (!itemDef.ContainsTag(tag))
            {
                List<ItemTag> tags = itemDef.tags.ToList();
                tags.Add(tag);
                itemDef.tags = tags.ToArray();

                ItemIndex index = itemDef.itemIndex;
                if (index != ItemIndex.None && ItemCatalog.itemIndicesByTag != null)
                {
                    List<ItemIndex> itemList = ItemCatalog.itemIndicesByTag[(int)tag].ToList();
                    if (!itemList.Contains(index))
                    {
                        itemList.Add(index);
                        ItemCatalog.itemIndicesByTag[(int)tag] = itemList.ToArray();
                    }
                }
            }
        }

        public static void DumpEntityStateConfig(EntityStateConfiguration esc)
        {

            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue)
                {
                    Debug.Log(esc.serializedFieldsCollection.serializedFields[i].fieldName + " - " + esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue);
                }
                else
                {
                    Debug.Log(esc.serializedFieldsCollection.serializedFields[i].fieldName + " - " + esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue);
                }
            }
        }
        public static void DumpEntityStateConfig(string entityStateName)
        {
            EntityStateConfiguration esc = LegacyResourcesAPI.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
            DumpEntityStateConfig(esc);
        }


        public static void DumpAddressableEntityStateConfig(string addressablePath)
        {
            EntityStateConfiguration esc = Addressables.LoadAssetAsync<EntityStateConfiguration>(addressablePath).WaitForCompletion();
            DumpEntityStateConfig(esc);
        }

        public static Object GetEntityStateFieldObject(string entityStateName, string fieldName)
        {
            EntityStateConfiguration esc = LegacyResourcesAPI.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    return esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue;
                }
            }
            return null;
        }

        public static string GetEntityStateFieldString(string entityStateName, string fieldName)
        {
            EntityStateConfiguration esc = LegacyResourcesAPI.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    return esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue;
                }
            }
            return string.Empty;
        }

        public static bool SetEntityStateField(string entityStateName, string fieldName, Object newObject)
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

        public static bool SetEntityStateField(string entityStateName, string fieldName, string value)
        {
            EntityStateConfiguration esc = LegacyResourcesAPI.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue = value;
                    return true;
                }
            }
            return false;
        }

        public static bool SetAddressableEntityStateField(string fullEntityStatePath, string fieldName, string value)
        {
            EntityStateConfiguration esc = Addressables.LoadAssetAsync<EntityStateConfiguration>(fullEntityStatePath).WaitForCompletion();
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue = value;
                    return true;
                }
            }
            return false;
        }

        public static bool SetAddressableEntityStateField(string fullEntityStatePath, string fieldName, Object newObject)
        {
            EntityStateConfiguration esc = Addressables.LoadAssetAsync<EntityStateConfiguration>(fullEntityStatePath).WaitForCompletion();
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

        public static Object GetAddressableEntityStateFieldObject(string fullEntityStatePath, string fieldName)
        {
            EntityStateConfiguration esc = Addressables.LoadAssetAsync<EntityStateConfiguration>(fullEntityStatePath).WaitForCompletion();
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    return esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue;
                }
            }
            return null;
        }

        public static string GetAddressableEntityStateFieldString(string fullEntityStatePath, string fieldName)
        {
            EntityStateConfiguration esc = Addressables.LoadAssetAsync<EntityStateConfiguration>(fullEntityStatePath).WaitForCompletion();
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    return esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue;
                }
            }
            return string.Empty;
        }

        //Embarassing code, there has to be a better way.
        public static string FloatToString(float f)
        {
            int whole = Mathf.FloorToInt(f);
            int dec = Mathf.FloorToInt((f - whole) * 100f);
            return whole + "." + dec;
        }
    }
}
