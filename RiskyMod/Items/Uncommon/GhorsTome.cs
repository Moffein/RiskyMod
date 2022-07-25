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
                            //On the Player Team, only real players can take Tome gold.
                            bool canPickup = true;
                            bool preventNPC = self.teamFilter.teamIndex == TeamIndex.Player;
                            CharacterBody cb = other.gameObject.GetComponent<CharacterBody>();
                            if (preventNPC)
                            {
                                if (cb && !cb.isPlayerControlled)
                                {
                                    canPickup = false;
                                    runOrig = true;
                                }
                            }

                            //Give gold to single player, instead of splitting it
                            if (canPickup && cb && cb.master)
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
