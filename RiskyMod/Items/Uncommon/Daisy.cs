using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.Uncommon
{
    public class Daisy
    {
        public static bool enabled = true;
        public static bool disableHealPulse = true;
        public Daisy()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            On.RoR2.HoldoutZoneController.Start += spawnOnHoldout;
            On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.FixedUpdate += SpawnOnMithrix;

            On.EntityStates.MeridianEvent.Phase1.OnEnter += Phase1_OnEnter;

            if (disableHealPulse)
            {
                On.EntityStates.TeleporterHealNovaController.TeleporterHealNovaGeneratorMain.Pulse += DisableHealPulse;
            }
        }

        private void spawnOnHoldout(On.RoR2.HoldoutZoneController.orig_Start orig, HoldoutZoneController self)
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
                        db.holdout = self;
                    }
                }
            }
        }

        private void SpawnOnMithrix(On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.orig_FixedUpdate orig, EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState self)
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
        }

        private void DisableHealPulse(On.EntityStates.TeleporterHealNovaController.TeleporterHealNovaGeneratorMain.orig_Pulse orig, EntityStates.TeleporterHealNovaController.TeleporterHealNovaGeneratorMain self)
        {
            return;
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
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.TPHealingNova);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.TPHealingNova);
        }
    }

    public class DaisyBehavior : MonoBehaviour
    {
        public static GameObject wardPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShrineHealing/ShrineHealingWard.prefab").WaitForCompletion();
        public GameObject wardInstance;
        public HealingWard healingWard;
        public Transform wardOrigin;
        public HoldoutZoneController holdout;

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

            //Shut off ward if holdout is charged. Doesn't do anything if no holdout is assigned.
            bool isFullCharge = holdout && holdout.charge >= 1f;
            if (isFullCharge)
            {
                if (wardInstance)
                {
                    Destroy(wardInstance);
                    wardInstance = null;
                }
                return;
            }

            //Spawn Ward
            if (!wardInstance && wardOrigin != null)
            {
                wardInstance = UnityEngine.Object.Instantiate<GameObject>(DaisyBehavior.wardPrefab, wardOrigin.position, wardOrigin.rotation);
                wardInstance.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
                healingWard = wardInstance.GetComponent<HealingWard>();
                NetworkServer.Spawn(wardInstance);
            }

            //Update Ward Stats
            if (healingWard)
            {
                int stack = Mathf.Max(0, daisyCount - 1);

                healingWard.Networkradius = 16f;
                float healFractionPerSecond = 0.05f + stack * 0.025f;
                healingWard.healFraction = healFractionPerSecond * healingWard.interval;
            }
        }
    }
}
