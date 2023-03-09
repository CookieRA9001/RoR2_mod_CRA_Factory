using RoR2;
using UnityEngine;
using System.Collections.ObjectModel;
using TILER2;
using R2API;

namespace CRA_Factory {
    class Supplements: Item<Supplements> {
        //Assets/Import/Suplements/object.prefab

        ////// Item Data //////

        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new(new[] { ItemTag.Healing, ItemTag.Damage, ItemTag.Utility });

        public float growth = 0.03f;

        ////// TILER2 Module Setup //////

        public Supplements() {
            modelResource = CRAPlugin.resources.LoadAsset<GameObject>("Assets/Import/Suplements/object.prefab");
            iconResource = CRAPlugin.resources.LoadAsset<Sprite>("Assets/Import/Suplements/icon.png");
        }

        public override void SetupModifyItemDef() {
            base.SetupModifyItemDef();

            Utils.ItemUtil.AddTokens(
                itemDef,
                "SUPPLEMENTS",
                "Experimental Supplements",
                "I little bit of everything but not as good as the real thing",
                $"{growth*100}% to all stats (exponestial)."
            );
        }

        public override void Install() {
            base.Install();
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        public override void Uninstall() {
            base.Uninstall();
            RecalculateStatsAPI.GetStatCoefficients -= RecalculateStatsAPI_GetStatCoefficients;
        }

        ////// Hooks //////

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args) {
            if (!sender) return;
            float item_count = GetCount(sender);

            if (item_count <= 0) return;

            //item_count *= percentage_per_stack;
            float amount = Mathf.Pow(1+growth, item_count) - 1;
            args.healthMultAdd += amount;
            args.shieldMultAdd += amount;
            args.regenMultAdd += amount;
            args.damageMultAdd += amount;
            args.moveSpeedMultAdd += amount;
            args.levelMultAdd += amount;
            args.jumpPowerMultAdd += amount;
            args.critDamageMultAdd += amount;
            args.attackSpeedMultAdd += amount;
            args.armorAdd += sender.baseArmor * amount;
        }
    }
}
