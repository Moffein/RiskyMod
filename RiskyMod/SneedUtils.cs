using RoR2;
using UnityEngine;

namespace SneedUtils
{
    public class SneedUtils
    {
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
            EntityStateConfiguration esc = Resources.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
            DumpEntityStateConfig(esc);
        }

        public static Object GetEntityStateFieldObject(string entityStateName, string fieldName)
        {
            EntityStateConfiguration esc = Resources.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
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
            EntityStateConfiguration esc = Resources.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
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
            EntityStateConfiguration esc = Resources.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
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
            EntityStateConfiguration esc = Resources.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
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
