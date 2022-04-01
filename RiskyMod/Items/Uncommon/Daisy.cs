using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace RiskyMod.Items.Uncommon
{
    public class Daisy
    {
        public static bool enabled = true;


        public Daisy()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            On.RoR2.TeleporterInteraction.ChargingState.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    int daisyCount = Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.TPHealingNova.itemIndex, false, true);
                    if (daisyCount > 0)
                    {
                        DaisyBehavior db = self.gameObject.GetComponent<DaisyBehavior>();
                        if (!db)
                        {
                            db = self.gameObject.AddComponent<DaisyBehavior>();
                        }
                    }
                }
            };
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.TPHealingNova);
        }
    }

    public class DaisyBehavior : MonoBehaviour
    {
        public static GameObject wardPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/Shrines/ShrineHealingWard");
        public GameObject wardInstance;
        public HealingWard healingWard;

        public int daisyCount
        {
            get
            {
                return Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.TPHealingNova.itemIndex, false, true);
            }
        }

        public void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            //Spawn Ward
            if (!wardInstance)
            {
                this.wardInstance = UnityEngine.Object.Instantiate<GameObject>(DaisyBehavior.wardPrefab, base.transform.position, base.transform.rotation);
                this.wardInstance.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
                this.healingWard = this.wardInstance.GetComponent<HealingWard>();
                NetworkServer.Spawn(this.wardInstance);
            }

            //Update Ward Stats
            if (healingWard)
            {
                int stack = Mathf.Max(0, daisyCount - 1);

                healingWard.Networkradius = 16f;
                //healingWard.radius = 16f;
                float healFraction = 0.045f + 0.0225f * stack;
                healingWard.healFraction = healFraction;
            }
        }
    }
}
