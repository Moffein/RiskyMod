using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

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
                if (NetworkServer.active && self != null && self.gameObject && self.transform)
                {
                    int daisyCount = Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.TPHealingNova.itemIndex, false, true);
                    if (daisyCount > 0)
                    {
                        DaisyBehavior db = self.gameObject.GetComponent<DaisyBehavior>();
                        if (!db)
                        {
                            db = self.gameObject.AddComponent<DaisyBehavior>();
                            db.wardOrigin = self.transform;
                        }
                    }
                }
            };

            On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.FixedUpdate += (orig, self) =>
            {
                orig(self);
                int daisyCount = Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.TPHealingNova.itemIndex, false, true);
                if (daisyCount > 0)
                {
                    if (self.gameObject && self.childLocator)
                    {
                        Transform transform = self.childLocator.FindChild("CenterOrbEffect");
                        if (transform)
                        {
                            DaisyBehavior db = self.gameObject.GetComponent<DaisyBehavior>();
                            if (!db)
                            {
                                db = self.gameObject.AddComponent<DaisyBehavior>();
                                db.wardOrigin = transform;
                            }
                        }
                    }
                }
            };

            On.EntityStates.MeridianEvent.Phase1.OnEnter += Phase1_OnEnter;
        }

        private void Phase1_OnEnter(On.EntityStates.MeridianEvent.Phase1.orig_OnEnter orig, EntityStates.MeridianEvent.Phase1 self)
        {
            orig(self);
            if (NetworkServer.active && self.meridianEventTriggerInteraction && self.meridianEventTriggerInteraction.falseSonEntryFXPosition)
            {
                int daisyCount = Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.TPHealingNova.itemIndex, false, true);
                if (daisyCount > 0)
                {
                    DaisyBehavior db = self.gameObject.GetComponent<DaisyBehavior>();
                    if (!db)
                    {
                        db = self.gameObject.AddComponent<DaisyBehavior>();
                        db.wardOrigin = self.meridianEventTriggerInteraction.falseSonEntryFXPosition;
                    }
                }
            }
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
        public Transform wardOrigin;

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
            if (!wardInstance && wardOrigin != null)
            {
                this.wardInstance = UnityEngine.Object.Instantiate<GameObject>(DaisyBehavior.wardPrefab, wardOrigin.position, wardOrigin.rotation);
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
                float healFractionPerSecond = 0.05f + stack * 0.025f;
                healingWard.healFraction = healFractionPerSecond * healingWard.interval;

                //Debug.Log(healingWard.interval); 0.25f
            }
        }
    }
}
