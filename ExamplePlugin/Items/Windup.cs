using RoR2;
using UnityEngine;
using System.Collections.ObjectModel;
using TILER2;
using R2API;

namespace CRA_Factory {
    public class Windup: Item<Windup> {

        ////// Item Data //////

        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new(new[] { ItemTag.Damage });

        ////// TILER2 Module Setup //////

        public Windup() {
            modelResource = CRAPlugin.resources.LoadAsset<GameObject>("Assets/Import/Windup/object.prefab");
            iconResource = CRAPlugin.resources.LoadAsset<Sprite>("Assets/Import/Windup/icon.png");
        }

        public override void SetupModifyItemDef() {
            base.SetupModifyItemDef();

            Utils.ItemUtil.AddTokens(
                itemDef,
                "WINDUP",
                "Windup",
                "",
                $""
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


        }
    }
}
