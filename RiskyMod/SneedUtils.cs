﻿using BepInEx.Configuration;
using R2API;
using RiskyMod.Content;
using RoR2;
using RoR2.CharacterAI;
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

        public static GameObject teleportHelperPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/DirectorSpawnProbeHelperPrefab.prefab").WaitForCompletion();
        public enum MonsterCategories
        {
            BasicMonsters, Minibosses, Champions
        }

        public static void SetPrioritizePlayers(GameObject masterObject)
        {
            if (!masterObject)
            {
                Debug.LogError("SneedUtils: Attempted to call SetPrioritizePlayers on null gameobject.");
                return;
            }

            BaseAI ai = masterObject.GetComponent<BaseAI>();
            if (!ai)
            {
                Debug.LogError("SneedUtils: Attempted to call SetPrioritizePlayers on GameObject without a BaseAI " + masterObject);
                return;
            }

            ai.prioritizePlayers = true;
        }

        public static bool HasShieldOnly(CharacterBody self)
        {
            return self.HasBuff(RoR2Content.Buffs.AffixLunar) || (self.inventory && self.inventory.GetItemCount(RoR2Content.Items.ShieldOnly) > 0);
        }

        public static void AddCooldownBuff(CharacterBody body, BuffDef buff, int cooldown, float bonusDuration = 0f)
        {
            body.ClearTimedBuffs(buff);
            for (int i = 0; i < cooldown; i++)
            {
                body.AddTimedBuff(buff, i + 1f + bonusDuration);
            }
        }

        public static bool RemoveMonsterSpawnCardFromCategory(DirectorCardCategorySelection categorySelection, SpawnCard spawnCard, MonsterCategories monsterCategory)
        {
            int categoryIndex = FindCategoryIndexByName(categorySelection, monsterCategory);
            if (categoryIndex >= 0 && categorySelection.categories[categoryIndex].cards != null)
            {
                int origLength = categorySelection.categories[categoryIndex].cards.Length;
                categorySelection.categories[categoryIndex].cards = categorySelection.categories[categoryIndex].cards.Where(dc => dc.spawnCard != spawnCard).ToArray();
                return categorySelection.categories[categoryIndex].cards.Length < origLength;
            }
            return false;
        }

        public static bool AddMonsterDirectorCardToCategory(DirectorCardCategorySelection categorySelection, DirectorCard directorCard, MonsterCategories monsterCategory)
        {
            int categoryIndex = FindCategoryIndexByName(categorySelection, monsterCategory);
            if (categoryIndex >= 0)
            {
                categorySelection.AddCard(categoryIndex, directorCard);
                return true;
            }
            return false;
        }

        //Minibosses
        //Basic Monsters
        //Champions
        public static int FindCategoryIndexByName(DirectorCardCategorySelection dcs, MonsterCategories category)
        {
            string categoryName;
            switch(category)
            {
                case MonsterCategories.BasicMonsters:
                    categoryName = "Basic Monsters";
                    break;
                case MonsterCategories.Minibosses:
                    categoryName = "Minibosses";
                    break;
                case MonsterCategories.Champions:
                    categoryName = "Champions";
                    break;
                default:
                    return -1;
                    break;
            }
            return FindCategoryIndexByName(dcs, categoryName);
        }

        public static int FindCategoryIndexByName(DirectorCardCategorySelection dcs, string categoryName)
        {
            //Debug.Log("Dumping categories for " + dcs.name);
            for (int i = 0; i < dcs.categories.Length; i++)
            {
                //Debug.Log(dcs.categories[i].name);
                if (string.CompareOrdinal(dcs.categories[i].name, categoryName) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool ReplaceSkillDef(SkillFamily skillFamily, SkillDef targetSkill, SkillDef newSkill)
        {
            bool successfullyReplaced = false;

            if (skillFamily.variants != null && targetSkill != null && newSkill != null)
            {
                for (int i = 0; i < skillFamily.variants.Length; i++)
                {
                    if (skillFamily.variants[i].skillDef == targetSkill)
                    {
                        skillFamily.variants[i].skillDef = newSkill;
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
            spawnCard.prefab = teleportHelperPrefab;
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

        public static BuffDef CreateBuffDef(string name, bool canStack, bool isCooldown, bool isDebuff, Color color, Sprite iconSprite, bool blacklistFromNoxiousThorn = false)
        {
            BuffDef bd = ScriptableObject.CreateInstance<BuffDef>();
            bd.name = name;
            bd.canStack = canStack;
            bd.isCooldown = isCooldown;
            bd.isDebuff = isDebuff;
            bd.buffColor = color;
            bd.iconSprite = iconSprite;
            if (blacklistFromNoxiousThorn)
            {
                bd.flags |= BuffDef.Flags.ExcludeFromNoxiousThorns;
            }
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

        public static List<HealthComponent> FindEnemiesInSphere(float radius, Vector3 position, TeamIndex team, bool airborneOnly = false)
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
                        if (healthComponent.body && healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != team)
                        {
                            if (!airborneOnly ||
                                (healthComponent.body.isFlying ||
                                (healthComponent.body.characterMotor && !healthComponent.body.characterMotor.isGrounded)))
                            {
                                hcList.Add(healthComponent);
                            }
                        }
                    }
                }
            }
            return hcList;
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

        public static void DumpAddressableEntityStateConfig(string addressablePath)
        {
            EntityStateConfiguration esc = Addressables.LoadAssetAsync<EntityStateConfiguration>(addressablePath).WaitForCompletion();
            DumpEntityStateConfig(esc);
        }

        public static void SetAddressableEntityStateField(string fullEntityStatePath, string fieldName, string value)
        {
            Addressables.LoadAssetAsync<EntityStateConfiguration>(fullEntityStatePath).Completed += handle => SetAddressableEntityStateField_String_Completed(handle, fieldName, value);
        }

        private static void SetAddressableEntityStateField_String_Completed(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<EntityStateConfiguration> handle, string fieldName, string value)
        {
            EntityStateConfiguration esc = handle.Result;
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue = value;
                    return;
                }
            }
            Debug.LogError("RiskyMod: " + esc + " does not have field " + fieldName);
        }

        public static void SetAddressableEntityStateField(string fullEntityStatePath, string fieldName, UnityEngine.Object newObject)
        {
            Addressables.LoadAssetAsync<EntityStateConfiguration>(fullEntityStatePath).Completed += handle => SetAddressableEntityStateField_Object_Completed(handle, fieldName, newObject);
        }

        private static void SetAddressableEntityStateField_Object_Completed(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<EntityStateConfiguration> handle, string fieldName, Object newObject)
        {
            EntityStateConfiguration esc = handle.Result;
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue = newObject;
                    return;
                }
            }
            Debug.LogError("RiskyMod: " + esc + " does not have field " + fieldName);
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
            Debug.LogError("RiskyMod: " + fullEntityStatePath + " does not have field " + fieldName);
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
            Debug.LogError("RiskyMod: " + fullEntityStatePath + " does not have field " + fieldName);
            return string.Empty;
        }

        //Embarassing code, there has to be a better way.
        public static string FloatToString(float f)
        {
            int whole = Mathf.FloorToInt(f);
            int dec = Mathf.FloorToInt((f - whole) * 100f);
            return whole + "." + dec;
        }

        public static bool AddSkillToFamily(SkillFamily skillFamily, SkillDef skillDef)
        {
            if (!skillDef)
            {
                Debug.LogError("SkillsReturns: Could not add " + skillDef.skillName + ": SkillDef is null.");
                return false;
            }

            if (skillFamily == null)
            {
                Debug.LogError("SkillsReturns: Could not add " + skillDef.skillName + ": SkillFamily is null.");
                return false;
            }

            System.Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
            return true;
        }
    }
}
