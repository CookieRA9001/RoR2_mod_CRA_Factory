using System.Collections.Generic;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace CRA_Factory.Items {
    class _item {
        public ItemDef itemDef;
        public int active_items;

        virtual public void Init() {
            InitStart();
            AddTokens("TOKEN_NAME_NEEDED!");
            // function
            InitFinish();
        }

        virtual public void InitStart() {
            active_items = 0;
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            InitVisuals();
            InitDefaults();
        }

        virtual public void InitFinish() {
            var displayRules = new ItemDisplayRuleDict(null);
            ItemAPI.Add(new CustomItem(itemDef, displayRules));
            Handler.Items.dict[itemDef.name] = itemDef;
        }

        virtual public void AddTokens(
            string base_token,
            string name = "dumb item [WIP]",
            string pickup = "Pick me up fool! WIP",
            string desc = "Idk, you figure it out! WIP",
            string lore = "[ lore go brrr... ]"
        ) {
            Log.LogInfo(base_token);

            itemDef.name = base_token + "_NAME";
            itemDef.nameToken = itemDef.name;
            itemDef.pickupToken = base_token + "_PICKUP";
            itemDef.descriptionToken = base_token + "_DESC";
            itemDef.loreToken = base_token + "_LORE";

            LanguageAPI.Add(itemDef.nameToken, name);
            LanguageAPI.Add(itemDef.pickupToken, pickup);
            LanguageAPI.Add(itemDef.descriptionToken, desc);
            LanguageAPI.Add(itemDef.loreToken, lore);
        }

        virtual public void InitVisuals() {
            #pragma warning disable Publicizer001
            itemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier2Def.asset").WaitForCompletion();
            #pragma warning restore Publicizer001

            itemDef.pickupIconSprite = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
            itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");
        }

        virtual public void InitDefaults() {
            itemDef.canRemove = true;
            itemDef.hidden = false;
        }

        virtual public int getCount(CharacterBody self) {
            return self.inventory.GetItemCount(itemDef.itemIndex);
        }

        virtual public bool isCountValid(CharacterBody self) {
            return getCount(self) >= 1;
        }

        virtual public bool isCountValid(int count) {
            return count >= 1;
        }

        virtual public int getItemCountChangeDif(CharacterBody self) {
            int c = getCount(self);
            int r = 0;

            if (isCountValid(c)) {
                r = (c - active_items);
                active_items = c;
            }

            return r;
        }
    }
}
