using System;
using RoR2.ContentManagement;
using RoR2;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using RoR2.Skills;

namespace RiskyMod.Content
{
    public class Content : IContentPackProvider
    {
        public static ContentPack content = new ContentPack();
        public static List<GameObject> networkedObjectPrefabs = new List<GameObject>();
        public static List<GameObject> bodyPrefabs = new List<GameObject>();
        public static List<GameObject> masterPrefabs = new List<GameObject>();
        public static List<BuffDef> buffDefs = new List<BuffDef>();
        public static List<EffectDef> effectDefs = new List<EffectDef>();
        public static List<GameObject> projectilePrefabs = new List<GameObject>();
        public static List<Type> entityStates = new List<Type>();
        public static List<SkillDef> skillDefs = new List<SkillDef>();
        public static List<SkillFamily> skillFamilies = new List<SkillFamily>();
        public static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        public string identifier => "RiskyMod.content";

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(content, args.output);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            content.networkedObjectPrefabs.Add(networkedObjectPrefabs.ToArray());
            content.bodyPrefabs.Add(bodyPrefabs.ToArray());
            content.masterPrefabs.Add(masterPrefabs.ToArray());
            content.buffDefs.Add(buffDefs.ToArray());
            content.effectDefs.Add(effectDefs.ToArray());
            content.projectilePrefabs.Add(projectilePrefabs.ToArray());
            content.entityStateTypes.Add(entityStates.ToArray());
            content.skillDefs.Add(skillDefs.ToArray());
            content.skillFamilies.Add(skillFamilies.ToArray());
            content.networkSoundEventDefs.Add(networkSoundEventDefs.ToArray());
            yield break;
        }
    }
}
