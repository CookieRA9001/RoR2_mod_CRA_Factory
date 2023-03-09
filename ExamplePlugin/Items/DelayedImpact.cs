using RoR2;
using UnityEngine;
using System.Collections.ObjectModel;
using TILER2;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using RoR2.Projectile;
using System.Collections;
using System.Collections.Generic;

namespace CRA_Factory {
    public class DelayedImpact: Item<DelayedImpact> {

        ////// Item Data //////

        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new(new[] { ItemTag.Damage });

        public float impactDelay = 10;
        public float impactBaseDamage = 10;
        public float impactScaling = 1.1f;
        public BuffDef delayedImpactDebuff { get; private set; }

        ////// TILER2 Module Setup //////

        public DelayedImpact() {
            modelResource = CRAPlugin.resources.LoadAsset<GameObject>("Assets/Import/DelayedImpact/object.prefab");
            iconResource = CRAPlugin.resources.LoadAsset<Sprite>("Assets/Import/DelayedImpact/icon.png");
            HitTraker = new Dictionary<int, int>();
        }

        public override void SetupModifyItemDef() {
            base.SetupModifyItemDef();

            Utils.ItemUtil.AddTokens(
                itemDef,
                "DELAYED_IMPACT",
                "Delayed Impact",
                "",
                $""
            );
        }

        public override void SetupAttributes() {
            base.SetupAttributes();

            delayedImpactDebuff = ScriptableObject.CreateInstance<BuffDef>();
            delayedImpactDebuff.buffColor = Color.white;
            delayedImpactDebuff.canStack = true;
            delayedImpactDebuff.isDebuff = true;
            delayedImpactDebuff.name = "Delayed_Impact";
            delayedImpactDebuff.iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/texDifficultyEasyIcon.png") //need my own icon
                .WaitForCompletion();
            ContentAddition.AddBuffDef(delayedImpactDebuff);
        }

        public override void Install() {
            base.Install();
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
        }

        public override void Uninstall() {
            base.Uninstall();
            On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.OnBuffFinalStackLost -= CharacterBody_OnBuffFinalStackLost;
        }

        ////// Hooks //////

        // scufed method
        public Dictionary<int, int> HitTraker;

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim) {
            orig(self, damageInfo, victim);

            if (NetworkServer.active && damageInfo != null && damageInfo.attacker) {
                var count = GetCount(damageInfo.attacker.GetComponent<CharacterBody>());

                if (count > 0 && damageInfo.attacker != victim) {
                    if (victim.TryGetComponent<CharacterBody>(out var vbody)) {
                        BuffIndex bi = delayedImpactDebuff.buffIndex;

                        if (!vbody.HasBuff(bi)) {
                            vbody.AddTimedBuff(delayedImpactDebuff, impactDelay);

                            if (HitTraker.ContainsKey(self.GetInstanceID())) HitTraker.Remove(self.GetInstanceID());

                            HitTraker.Add(self.GetInstanceID(), 1);
                        }
                        else if (HitTraker.ContainsKey(self.GetInstanceID())) {
                            HitTraker[self.GetInstanceID()] += 1;
                        }
                        else {
                            HitTraker.Add(self.GetInstanceID(), 1);
                        }
                    }
                }
            }
        }

        private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef) {
            orig(self, buffDef);
            if (self && buffDef == delayedImpactDebuff && self.modelLocator) {
                GameObject blast = GameObject.Instantiate(PrefabAssembler.DelayedImpactEffect, self.gameObject.transform.position, self.gameObject.transform.rotation);
                
                //ProjectileController projectileController = blast.AddComponent<ProjectileController>();
                ProjectileDamage projectileDamage = blast.AddComponent<ProjectileDamage>();
                ProjectileExplosion projectileExplosion = blast.AddComponent<ProjectileExplosion>();

                int b = 1;

                if (HitTraker.ContainsKey(self.GetInstanceID())) {
                    b = HitTraker[self.GetInstanceID()];
                    HitTraker.Remove(self.GetInstanceID());
                }

                projectileDamage.damage = impactBaseDamage * Mathf.Pow(impactScaling, b);
                //projectileExplosion.blastRadius = 1 + 0.1f * b;
            }
        }
    }
}
