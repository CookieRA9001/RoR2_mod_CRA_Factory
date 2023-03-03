using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CRA_Factory.Items {
    class AlienAvocado: _item {
        public float health_per_hit_percent_base = 0.1f;
        public float health_per_hit_percent_ext = 0.05f;
        public float health_bost_base = 20;
        public float health_bost_ext = 20;
        public float armor_bost_base = 10;
        public float armor_bost_ext = 5;
        public float shield_bost_base = 50;
        public float shield_bost_ext = 10;
        public float regen_bost_base = 1;
        public float regen_bost_ext = 0.5f;

        override public void Init() {
            InitStart();

            AddTokens(
                "ALIEN_AVOCADO",
                "Alien Avocado",
                "Health out of this world"
            );

            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;

            InitFinish();
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim) {
            if (damageInfo == null || damageInfo.rejected || !damageInfo.attacker || damageInfo.attacker == victim.gameObject ||
                damageInfo.attacker.GetComponent<CharacterBody>().inventory == null) {
                orig(self, damageInfo, victim);
                return;
            }

            int item_count = getCount(damageInfo.attacker.GetComponent<CharacterBody>());

            if (!isCountValid(item_count)) {
                orig(self, damageInfo, victim);
                return;
            }

            var hc = damageInfo.attacker.GetComponent<HealthComponent>();
            if (hc) {
                float num = health_per_hit_percent_base + (item_count - 1) * health_per_hit_percent_ext;
                float dam = damageInfo.damage;
                hc.Heal(num*dam, default(ProcChainMask), false);
            }

            orig(self, damageInfo, victim);
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) {
            if (self.inventory) {
                Log.LogInfo("CharacterBody_OnInventoryChanged");

                int dif = getItemCountChangeDif(self);
                int item_count = getCount(self);

                Log.LogInfo($"{dif} {item_count}");

                if (dif == 0) {
                    orig(self);
                    return;
                }

                Log.LogInfo($"{self.baseArmor} {self.baseRegen} {self.baseMaxHealth} {self.baseMaxShield}");

                if (item_count == dif) { // prev was zero
                    self.baseArmor += armor_bost_base;
                    self.baseRegen += regen_bost_base;
                    self.baseMaxHealth += health_bost_base;
                    self.baseMaxShield += shield_bost_base;
                    dif -= 1;
                }

                self.baseArmor += armor_bost_ext * dif;
                self.baseRegen += regen_bost_ext * dif;
                self.baseMaxHealth += health_bost_ext * dif;
                self.baseMaxShield += shield_bost_ext * dif;

                Log.LogInfo($"{self.baseArmor} {self.baseRegen} {self.baseMaxHealth} {self.baseMaxShield}");

            }

            orig(self);
        }
    }
}
