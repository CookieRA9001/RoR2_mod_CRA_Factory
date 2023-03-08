using RoR2;
using UnityEngine;
using System.Collections.ObjectModel;
using TILER2;
using R2API;

namespace CRA_Factory {
	public class AlienAvocado: Item<AlienAvocado> {

		////// Item Data //////

		public override ItemTier itemTier => ItemTier.Tier2;
		public override ReadOnlyCollection<ItemTag> itemTags => new(new[] { ItemTag.Healing, ItemTag.Damage });

        public float health_per_hit_percent_base = 0.05f;
        public float health_per_hit_percent_ext = 0.0025f;
        public float health_bost_base = 50;
        public float health_bost_ext = 20;
        public float armor_bost_base = 20;
        public float armor_bost_ext = 5;
        public float shield_bost_base = 50;
        public float shield_bost_ext = 20;
        public float regen_bost_base = 1;
        public float regen_bost_ext = 0.5f;

        ////// TILER2 Module Setup //////

        public AlienAvocado() {
			modelResource = CRAPlugin.resources.LoadAsset<GameObject>("Assets/Import/AlienAvocado/object.prefab");
			iconResource = CRAPlugin.resources.LoadAsset<Sprite>("Assets/Import/AlienAvocado/icon.png");
        }

        public override void SetupModifyItemDef() {
            base.SetupModifyItemDef();

            Utils.ItemUtil.AddTokens(
                itemDef,
                "ALIEN_AVOCADO",
                "Alien Avocado",
                "Health out of this world",
                $"Get <style=cIsHealing>{health_bost_base} max health</style> <style=cStack>(+{health_bost_ext} per extra stack)</style>, " +
                $"<style=cIsHealing>{shield_bost_base} max shield</style> <style=cStack>(+{shield_bost_ext} per extra stack)</style>, " +
                $"<style=cIsHealing>{armor_bost_base} armor</style> <style=cStack>(+{armor_bost_ext} per extra stack)</style> and " +
                $"<style=cIsHealing>{regen_bost_base} regen</style> <style=cStack>(+{regen_bost_ext} per extra stack)</style> to your <style=cIsHealth>base stats</style>. " +
                $"Heal for <style=cIsHealing>{health_per_hit_percent_base*100}%</style> <style=cStack>(+{health_per_hit_percent_ext}% per extra stack)</style> " +
                $"of <style=cIsHealth>damage dealt</style> on hit."
            );
        }

        public override void Install() {
			base.Install();
			RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

		public override void Uninstall() {
			base.Uninstall();
			RecalculateStatsAPI.GetStatCoefficients -= RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
        }

        ////// Hooks //////

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args) {
			if (!sender) return;
			int item_count = GetCount(sender);

            if (item_count <= 0) return;

            item_count--;
            args.baseHealthAdd += health_bost_base + item_count * health_bost_ext;
            args.baseShieldAdd += shield_bost_base + item_count * shield_bost_ext;
            args.baseRegenAdd += regen_bost_base + item_count * regen_bost_ext;
			args.armorAdd += armor_bost_base + item_count * armor_bost_ext;
		}

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo) {
            orig(self, damageInfo);

            if (damageInfo == null || damageInfo.rejected || !damageInfo.attacker || damageInfo.attacker == self.gameObject || damageInfo.attacker.GetComponent<CharacterBody>().inventory == null) {
                return;
            }

            int item_count = GetCount(damageInfo.attacker.GetComponent<CharacterBody>());

            if (item_count <= 0) {
                return;
            }

            var hc = damageInfo.attacker.GetComponent<HealthComponent>();
            if (hc) {
                float num = health_per_hit_percent_base + (item_count - 1) * health_per_hit_percent_ext;
                float dam = damageInfo.damage;
                hc.Heal(num * dam, default, false);
            }
        }
    }
}