using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CRA_Factory {
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI))]

    public class CRAPlugin : BaseUnityPlugin {
        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "CookieRA";
        public const string PluginName = "CRAFactory";
        public const string PluginVersion = "1.0.0";

        public void Awake()
        {
            Log.Init(Logger);

            Handler.Items.Init();

            Log.LogInfo(nameof(Awake) + " done.");
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F2)) {
                //Get the player body to use a position:
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                //And then drop our defined item in front of the player.
                Log.LogInfo($"Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(Handler.Items.dict["ALIEN_AVOCADO_NAME"].itemIndex), transform.position, transform.forward * 20f);
            }
        }
    }
}
