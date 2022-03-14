using RiskyMod.Content;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SneedUtils
{
    public class SneedUtils
    {
        public static void FixSkillName(SkillDef skillDef)
        {
            (skillDef as UnityEngine.Object).name = "RiskyMod_" + skillDef.skillName;
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

        public static void RemoveItemTag(ItemDef itemDef, ItemTag tag)
        {
            if (itemDef.ContainsTag(tag))
            {
                List<ItemTag> tags = itemDef.tags.ToList();
                tags.Remove(tag);
                itemDef.tags = tags.ToArray();
            }
        }

        public static void AddItemTag(ItemDef itemDef, ItemTag tag)
        {
            if (!itemDef.ContainsTag(tag))
            {
                List<ItemTag> tags = itemDef.tags.ToList();
                tags.Add(tag);
                itemDef.tags = tags.ToArray();
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
    }
}
