using System;
using System.Collections.Generic;
using System.IO;

public static class ModDependencyManager
{
    private static DictionaryList<string, ModDependent> loadedDependents;

    public static void LoadModDependencies()
    {
        if(loadedDependents != null)
        {
            return;
        }

        loadedDependents = new DictionaryList<string, ModDependent>();

        List<Mod> mods = ModManager.GetLoadedMods();
        foreach (Mod mod in mods)
        {
            try
            {
                ModDependent modDependent = ModDependent.LoadFromMod(mod);
                if (modDependent != null)
                {
                    loadedDependents.Add(modDependent.ModName, modDependent);
                }
            }
            catch (Exception e)
            {
                Log.Error("[MODS] Failed loading mod dependencies from folder: " + Path.GetFileName(mod.Path));
                Log.Exception(e);
            }
        }
    }

    public static ModDependent GetModDependent(string modName)
    {
        return ModDependentLoaded(modName) ? loadedDependents.dict[modName] : null;
    }

    public static bool ModDependentLoaded(string modName)
    {
        return loadedDependents != null ? loadedDependents.dict.ContainsKey(modName) : false;
    }

    public static List<ModDependent> GetLoadedDependents()
    {
        return loadedDependents != null ? loadedDependents.list : null;
    }
}

