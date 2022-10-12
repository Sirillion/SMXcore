using System.Collections.Generic;
using System.IO;

public class ModDependent
{
    public Mod Mod { get; internal set; }
    public List<DependencyInfo> Dependencies { get; internal set; }

    public string ModName { get { return Mod.ModInfo.Name.Value; } }


    public static ModDependent LoadFromMod(Mod mod)
    {
        string dependencyXMLPath = mod.Path + "/Dependencies.xml";
        if (!File.Exists(dependencyXMLPath))
        {
            //Log.Warning("[MODS] Folder " + mod.FolderName + " does not contain a Dependencies.xml, ignoring");
            return null;
        }
        List<DependencyInfo> dependencies = DependencyInfoLoader.ParseXml(dependencyXMLPath, File.ReadAllText(dependencyXMLPath));

        if(dependencies.Count == 0)
        {
            Log.Warning("[MODS] " + mod.FolderName + "/Dependencies.xml does not specify any dependencies, ignoring");
            return null;
        }

        if (ModDependencyManager.ModDependentLoaded(mod.ModInfo.Name.Value))
        {
            Log.Warning("[MODS] " + mod.FolderName + "/ModDependency with same mod name already loaded, ignoring");
            return null;
        }

        return new ModDependent(mod, dependencies);
    }

    private ModDependent(Mod mod, List<DependencyInfo> dependencyInfos)
    {
        Mod = mod;
        Dependencies = dependencyInfos;

        foreach(DependencyInfo dependencyInfo in dependencyInfos)
        {
            dependencyInfo.DependentMod = this;
        }
    }

    public DependencyInfo GetDependencyInfo(Mod mod)
    {
        foreach (DependencyInfo dependencyInfo in Dependencies)
        {
            if (dependencyInfo.ModName.Equals(mod.ModInfo.Name.Value))
            {
                return dependencyInfo;
            }
        }

        return null;
    }
}
