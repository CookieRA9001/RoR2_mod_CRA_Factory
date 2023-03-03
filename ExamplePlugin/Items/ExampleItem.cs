using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CRA_Factory.Items {
    class ExampleItem {
        public ItemDef itemDef;

        public void Init() {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();

            // Language Tokens, check AddTokens() below.
            itemDef.name = "EXAMPLE_CLOAKONKILL_NAME";
            itemDef.nameToken = "EXAMPLE_CLOAKONKILL_NAME";
            itemDef.pickupToken = "EXAMPLE_CLOAKONKILL_PICKUP";
            itemDef.descriptionToken = "EXAMPLE_CLOAKONKILL_DESC";
            itemDef.loreToken = "EXAMPLE_CLOAKONKILL_LORE";

            //The tier determines what rarity the item is:
            //Tier1=white, Tier2=green, Tier3=red, Lunar=Lunar, Boss=yellow,
            //and finally NoTier is generally used for helper items, like the tonic affliction
            #pragma warning disable Publicizer001
            itemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier2Def.asset").WaitForCompletion();
            #pragma warning restore Publicizer001

            //You can create your own icons and prefabs through assetbundles, but to keep this boilerplate brief, we'll be using question marks.
            itemDef.pickupIconSprite = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
            itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");

            //Can remove determines if a shrine of order, or a printer can take this item, generally true, except for NoTier items.
            itemDef.canRemove = true;

            //Hidden means that there will be no pickup notification,
            //and it won't appear in the inventory at the top of the screen.
            //This is useful for certain noTier helper items, such as the DrizzlePlayerHelper.
            itemDef.hidden = false;

            //Now let's turn the tokens we made into actual strings for the game:
            AddTokens();

            //You can add your own display rules here, where the first argument passed are the default display rules: the ones used when no specific display rules for a character are found.
            //For this example, we are omitting them, as they are quite a pain to set up without tools like ItemDisplayPlacementHelper
            var displayRules = new ItemDisplayRuleDict(null);

            //Then finally add it to R2API
            ItemAPI.Add(new CustomItem(itemDef, displayRules));

            //But now we have defined an item, but it doesn't do anything yet. So we'll need to define that ourselves.
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        // on kill effect
        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport report) {
            //If a character was killed by the world, we shouldn't do anything.
            if (!report.attacker || !report.attackerBody) {
                return;
            }

            var attackerCharacterBody = report.attackerBody;

            //We need an inventory to do check for our item
            if (attackerCharacterBody.inventory) {
                //store the amount of our item we have
                var garbCount = attackerCharacterBody.inventory.GetItemCount(itemDef.itemIndex);
                if (garbCount > 0 &&
                    //Roll for our 50% chance.
                    Util.CheckRoll(50, attackerCharacterBody.master)) {
                    //Since we passed all checks, we now give our attacker the cloaked buff.
                    //Note how we are scaling the buff duration depending on the number of the custom item in our inventory.
                    attackerCharacterBody.AddTimedBuff(RoR2Content.Buffs.Cloak, 3 + garbCount);
                }
            }
        }

        private void AddTokens() {
            //The Name should be self explanatory
            LanguageAPI.Add("EXAMPLE_CLOAKONKILL_NAME", "Cuthroat's Garb");

            //The Pickup is the short text that appears when you first pick this up. This text should be short and to the point, numbers are generally ommited.
            LanguageAPI.Add("EXAMPLE_CLOAKONKILL_PICKUP", "Chance to cloak on kill");

            //The Description is where you put the actual numbers and give an advanced description.
            LanguageAPI.Add("EXAMPLE_CLOAKONKILL_DESC", "Whenever you <style=cIsDamage>kill an enemy</style>, you have a <style=cIsUtility>5%</style> chance to cloak for <style=cIsUtility>4s</style> <style=cStack>(+1s per stack)</style>.");

            //The Lore is, well, flavor. You can write pretty much whatever you want here.
            LanguageAPI.Add("EXAMPLE_CLOAKONKILL_LORE", "Those who visit in the night are either praying for a favour, or preying on a neighbour.");
        }
    }
}
