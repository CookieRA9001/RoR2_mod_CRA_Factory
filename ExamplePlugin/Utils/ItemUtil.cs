using RoR2;
using R2API;

namespace CRA_Factory.Utils {
    static class ItemUtil {
        static public void AddTokens(
            ItemDef itemDef,
            string base_token,
            string name = "dumb item [WIP]",
            string pickup = "Pick me up fool! WIP",
            string desc = "Idk, you figure it out! WIP",
            string lore = "[ lore go brrr... ]"
       ) {
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
    }
}
