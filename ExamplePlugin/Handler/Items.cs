using System.Collections.Generic;
using RoR2;
using ItemList = CRA_Factory.Items;

namespace CRA_Factory.Handler {
    static class Items {
        static public Dictionary<string, ItemDef> dict = new Dictionary<string, ItemDef>();

        public static void Init() {
            dict = new Dictionary<string, ItemDef>();
            new ItemList.ExampleItem().Init();
            new ItemList.AlienAvocado().Init();
        }
    }
}
