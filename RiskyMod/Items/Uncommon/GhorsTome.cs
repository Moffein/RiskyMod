using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Items.Uncommon
{
    public class GhorsTome
    {
        public static bool enabled = true;
        public GhorsTome()
        {
            if (!enabled) return;

            //This seems like a convoluted way to do this.
            On.RoR2.MoneyPickup.OnTriggerStay += (orig, self, other) =>
            {
                bool runOrig = true;
                if(self.baseObject.name == "BonusMoneyPack(Clone)") //Only modify Tome, in case any other mod wants to add MoneyPickups
                {
                    if (NetworkServer.active && self.alive)
                    {
                        TeamIndex objectTeam = TeamComponent.GetObjectTeam(other.gameObject);
                        if (objectTeam == self.teamFilter.teamIndex)
                        {
                            bool modifyPickup = true;
                            CharacterBody cb = other.gameObject.GetComponent<CharacterBody>();
                            if (cb && !cb.isPlayerControlled)
                            {
                                modifyPickup = false;
                            }

                            //Give gold to single player, instead of splitting it
                            if (modifyPickup && cb && cb.master)
                            {
                                runOrig = false;
                                self.alive = false;
                                Vector3 position = self.transform.position;

                                cb.master.GiveMoney((uint)self.goldReward);
                                if (self.pickupEffectPrefab)
                                {
                                    EffectManager.SimpleEffect(self.pickupEffectPrefab, position, Quaternion.identity, true);
                                }
                                UnityEngine.Object.Destroy(self.baseObject);
                            }
                        }
                    }
                }


                if (runOrig) orig(self, other);
            };
        }
    }
}
