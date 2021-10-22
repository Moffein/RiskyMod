using UnityEngine;
using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Risky_Mod.MonoBehaviours
{
    public class SquidMinionManagerComponent : MonoBehaviour
    {
        private List<GameObject> squidList;
        private Inventory inventory = null;

        class SquidMinion
        {
            public GameObject gameObject;
            public HealthComponent healthComponent = null;
        }

        public void Awake()
        {
            squidList = new List<GameObject>();

            CharacterBody cb = base.gameObject.GetComponent<CharacterBody>();
            if (cb && cb.inventory)
            {
                inventory = cb.inventory;
            }
        }

        public bool CanSpawnSquid()
        {
            return squidList.Count < (inventory ? inventory.GetItemCount(RoR2Content.Items.Squid) : 0);
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                UpdateSquids();
            }
        }

        private void UpdateSquids()
        {
            List<GameObject> toRemove = new List<GameObject>();
            foreach (GameObject sm in squidList)
            {
                if (!sm.gameObject)
                {
                    toRemove.Add(sm);
                }
            }

            if (toRemove.Count > 0)
            {
                foreach (GameObject sm in toRemove)
                {
                    squidList.Remove(sm);
                }
                toRemove.Clear();
            }
        }

        public void AddSquid(GameObject go)
        {
            squidList.Add(go);
        }
    }
}
