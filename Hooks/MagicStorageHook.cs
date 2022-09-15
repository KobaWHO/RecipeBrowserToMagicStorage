using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MagicStorage;
using MagicStorage.Common.Systems;
using MagicStorage.Components;
using MagicStorage.UI;
using MagicStorage.UI.States;
using RecipeBrowserToMagicStorage.Utils;
using Terraria;
using Terraria.ModLoader;

namespace RecipeBrowserToMagicStorage.Hooks
{
    public static class MagicStorageReflection
    {
        public static void SetMagicStorageFilterName(string name)
        {
            Type type = null;
            Action refreshItemsAction = null;
            object storagePage = null;

            var openedStorageType = GetCurrentOpenedStorageType();
            switch (openedStorageType)
            {
                case StorageType.None:
                    return;
                case StorageType.Crafting:
                    type = typeof(CraftingGUI);
                    refreshItemsAction = CraftingGUI.RefreshItems;
                    storagePage = MagicUI.craftingUI.GetPage("Crafting");
                    break;
                case StorageType.Storage:
                    type = typeof(StorageGUI);
                    refreshItemsAction = StorageGUI.RefreshItems;
                    storagePage = MagicUI.storageUI.GetPage("Storage");
                    break;
            }
            
            var searchBar = ReflectionUtils.GetField<UISearchBar>(storagePage, "searchBar");
            if (searchBar == null)
                return;

            ReflectionUtils.SetValue(searchBar, "Text", name);
            ReflectionUtils.SetValue(searchBar, "cursorPosition", name.Length);
            refreshItemsAction?.Invoke();

            if (openedStorageType == StorageType.Crafting)
            {
                SelectFirstAvailableRecipe(name);
            }
        }

        private static void SelectFirstAvailableRecipe(object data)
        {
            try
            {
                var type = typeof(CraftingGUI);

                var threadRecipes = ReflectionUtils.GetField<List<Recipe>>(null, "recipes", type);
                var threadRecipesAvailable = ReflectionUtils.GetField<List<bool>>(null, "recipeAvailable", type);
                if (threadRecipes == null || threadRecipesAvailable == null)
                    return;

                var threadRecipesValid = new List<Recipe>();
                var threadRecipesAvailableValid = new List<bool>();
                var name = (string)data;

                for (var i = 0; i < threadRecipes.Count; i++)
                {
                    if (threadRecipes[i].createItem.Name != name)
                        continue;

                    threadRecipesValid.Add(threadRecipes[i]);
                    threadRecipesAvailableValid.Add(threadRecipesAvailable[i]);
                }

                var index = threadRecipesAvailableValid.IndexOf(true);
                var selectRecipe = index != -1 ? threadRecipesValid[index] : threadRecipesValid.FirstOrDefault();
                SelectRecipe(selectRecipe);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void SelectRecipe(Recipe selectRecipe)
        {
            ReflectionUtils.SetValue(null, "selectedRecipe", selectRecipe, typeof(CraftingGUI));
            StorageGUI.RefreshItems();
        }

        private static StorageType GetCurrentOpenedStorageType()
        {
            var storageAccess = Main.player[Main.myPlayer].GetModPlayer<StoragePlayer>().ViewingStorage();

            if (!Main.playerInventory || storageAccess.X < 0 || storageAccess.Y < 0)
                return StorageType.None;

            var modTile = TileLoader.GetTile(Main.tile[storageAccess.X, storageAccess.Y].TileType);
            var heart = (modTile as StorageAccess)?.GetHeart(storageAccess.X, storageAccess.Y);
            if (heart == null)
                return StorageType.None;

            return modTile is CraftingAccess ? StorageType.Crafting : StorageType.Storage;
        }

        private enum StorageType
        {
            None,
            Crafting,
            Storage
        }
    }
}