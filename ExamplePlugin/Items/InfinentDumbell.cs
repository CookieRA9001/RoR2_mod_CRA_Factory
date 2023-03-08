using RoR2;
using UnityEngine;
using System.Collections.ObjectModel;
using TILER2;
using R2API;

namespace CRA_Factory {
    public class InfinentDumbell: Item<InfinentDumbell> {

        ////// Item Data //////

        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new(new[] { ItemTag.Healing, ItemTag.Damage, ItemTag.Utility });

        public float percentage_per_stack = 0.05f;

        ////// TILER2 Module Setup //////

        public InfinentDumbell() {
            modelResource = CRAPlugin.resources.LoadAsset<GameObject>("Assets/Import/InfDumbbell/object.prefab");
            iconResource = CRAPlugin.resources.LoadAsset<Sprite>("Assets/Import/InfDumbbell/icon.png");
        }

        public override void SetupModifyItemDef() {
            base.SetupModifyItemDef();

            Utils.ItemUtil.AddTokens(
                itemDef,
                "INF_DUMBBELL_SET",
                "Infinit Dumbbell Set",
                "More GAINS from leveling!",
                $"Gain more stats from leveling up. <style=cIsHealing>+10%</style> <style=cStack>(+{percentage_per_stack*100}% per stack)</style> to max health, max shield, damage and regen <style=cIsHealing>per level</style>."
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

            item_count *= percentage_per_stack;
            float level = sender.level - 1; 
            args.healthMultAdd += item_count * level;
            args.shieldMultAdd += item_count * level;
            args.regenMultAdd += item_count * level;
            args.damageMultAdd += item_count * level;
        }
    }
}
