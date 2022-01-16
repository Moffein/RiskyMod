using System;
using RoR2.ContentManagement;
using System.Collections;
using UnityEngine;
using R2API;
using System.Collections.Generic;

namespace RiskyMod.Content
{
    public class Content : IContentPackProvider
    {
        public static ContentPack content = new ContentPack();
        public static List<GameObject> bodyPrefabs = new List<GameObject>();
        public static List<GameObject> masterPrefabs = new List<GameObject>();

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
            content.bodyPrefabs.Add(bodyPrefabs.ToArray());
            content.masterPrefabs.Add(masterPrefabs.ToArray());
            yield break;
        }
    }
}
