using BepInEx;
using R2API;
using R2API.Utils;
using System.Reflection;
using UnityEngine;
using BepInEx.Configuration;
using Path = System.IO.Path;
using TILER2;
using static TILER2.MiscUtil;
using RoR2;

// stole some stuff from https://github.com/ThinkInvis/RoR2-TinkersSatchel
// not sorry XD

namespace CRA_Factory {
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(TILER2Plugin.ModGuid, TILER2Plugin.ModVer)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(PrefabAPI), nameof(RecalculateStatsAPI), nameof(DirectorAPI), nameof(DeployableAPI), nameof(DamageAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class CRAPlugin: BaseUnityPlugin {
        public const string ModVer = "0.0.1";
        public const string ModName = "CRAFactory";
        public const string ModGuid = "com.CookieRA.CRAFactory";

        private static ConfigFile cfgFile;

        internal static FilingDictionary<T2Module> allModules = new();

        internal static AssetBundle resources;

        private void Awake() {
            Log.Init(Logger);

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CRA_Factory.cra_bundle")) {
                resources = AssetBundle.LoadFromStream(stream);
            }

            cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);

            var modInfo = new T2Module.ModInfo {
                displayName = "CRA Factory of Vices",
                longIdentifier = "CRA_Factory",
                shortIdentifier = "CRAF",
                mainConfigFile = cfgFile
            };
            allModules = T2Module.InitAll<T2Module>(modInfo);

            T2Module.SetupAll_PluginAwake(allModules);
        }

        private void Start() {
            T2Module.SetupAll_PluginStart(allModules);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F2)) {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(AlienAvocado.instance.itemDef.itemIndex), transform.position, transform.forward * 20f);
            }
            if (Input.GetKeyDown(KeyCode.F3)) {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(InfinentDumbell.instance.itemDef.itemIndex), transform.position, transform.forward * 20f);
            }
        }
    }
}