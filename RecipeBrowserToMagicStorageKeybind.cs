using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace RecipeBrowserToMagicStorage
{
    public class RecipeBrowserToMagicStorageKeybind : ModSystem
    {
        public static ModKeybind ActivateHotKey { get; set; }

        public override void Load()
        {
            ActivateHotKey = KeybindLoader.RegisterKeybind(Mod, "Find in Storage", "LeftControl");
        }
        
        public override void Unload()
        {
            ActivateHotKey = null;
        }
    }
}
