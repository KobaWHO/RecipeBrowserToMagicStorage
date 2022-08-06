using System;
using System.Reflection;
using MagicStorage;
using MagicStorage.Components;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RecipeBrowserToMagicStorage.Hooks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace RecipeBrowserToMagicStorage
{
	public class RecipeBrowserToMagicStorage : Mod
    {
        public override void Load()
        {
            RecipeBrowserHook.Load();
        }
        
        public override void Unload()
        {
            RecipeBrowserHook.Unload();
        }
    }
}