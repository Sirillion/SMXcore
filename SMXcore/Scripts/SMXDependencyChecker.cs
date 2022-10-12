using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class SMXDependencyChecker
{

    public static List<DependencyInfo> GetOutdatedDependencies()
    {
        List<DependencyInfo> outdatedDependencies = new List<DependencyInfo>();
        List<ModDependent> allDependents = ModDependencyManager.GetLoadedDependents();
        foreach(ModDependent dependent in allDependents)
        {
            outdatedDependencies.AddRange(GetOutdatedDependencies(dependent));
        }
        return outdatedDependencies;
    }

    public static List<DependencyInfo> GetOutdatedDependencies(ModDependent dependent)
    {
        List<DependencyInfo> outdatedDependencies = new List<DependencyInfo>();
        List<Mod> allMods = ModManager.GetLoadedMods();

        foreach (DependencyInfo dependencyInfo in dependent.Dependencies)
        {
            foreach (Mod mod in allMods)
            {
                if (dependencyInfo.ModName.Equals(mod.ModInfo.Name.Value) && isDependantModOutdated(mod, dependencyInfo))
                {
                    outdatedDependencies.Add(dependencyInfo);
                }
            }
        }

        return outdatedDependencies;
    }

    public static List<DependencyInfo> GetMissingDependencies()
    {
        List<DependencyInfo> missingDependencies = new List<DependencyInfo>();
        List<ModDependent> allDependents = ModDependencyManager.GetLoadedDependents();
        foreach (ModDependent dependent in allDependents)
        {
            missingDependencies.AddRange(GetMissingDependencies(dependent));
        }
        return missingDependencies;
    }

    public static List<DependencyInfo> GetMissingDependencies(ModDependent dependent)
    {
        List<DependencyInfo> missingDependencies = new List<DependencyInfo>();
        List<Mod> allMods = ModManager.GetLoadedMods();

        foreach (DependencyInfo dependencyInfo in dependent.Dependencies)
        {
            bool isMissing = true;
            foreach (Mod mod in allMods)
            {
                if (dependencyInfo.ModName.Equals(mod.ModInfo.Name.Value))
                {
                    isMissing = false;
                    break;
                }
            }

            if(isMissing)
            {
                missingDependencies.Add(dependencyInfo);
            }
        }

        return missingDependencies;
    }

    public static void CheckOutdatedDependencies()
    {
        ModDependencyManager.LoadModDependencies();
        List<DependencyInfo> outdatedMods = GetOutdatedDependencies();
        foreach(DependencyInfo dependency in outdatedMods)
        {
            ModDependent mod = dependency.DependentMod;

            Log.Error($"[{SMXcoreMod.TAG}] Mod: {mod.ModName} requires {dependency.ModName} version {dependency.Version} or higher");
        }
    }

    public static void CheckMissingDependencies()
    {
        ModDependencyManager.LoadModDependencies();
        List<DependencyInfo> missingMods = GetMissingDependencies();
        foreach (DependencyInfo dependency in missingMods)
        {
            ModDependent mod = dependency.DependentMod;

            Log.Error($"[{SMXcoreMod.TAG}] Mod: {dependency.ModName} not found installed, it is required by {mod.ModName}");
        }
    }

    public static bool isDependantModOutdated(Mod mod, DependencyInfo dependency)
    {
        string modVer = mod.ModInfo.Version.Value;
        string depVer = dependency.Version;
        if (modVer.Equals(depVer))
        {
            return false;
        }

        var modVersion = new Version(modVer);
        var depVersion = new Version(depVer);

        return (depVersion.Game != modVersion.Game && depVersion.Game > modVersion.Game) ||
            (depVersion.Mod != modVersion.Mod && depVersion.Mod > modVersion.Mod);  

    }

    private class Version
    {
        public int Game { get; internal set; }
        public int Mod { get; internal set; }

        public Version(string version)
        {
            string[] split = version.Split('.');
            string gameVersion = Regex.Match(split[0], @"\d+").Value;
            string modVersion = Regex.Match(split[1], @"\d+").Value;

            Game = int.Parse(gameVersion);
            Mod = int.Parse(modVersion);
        }

        public override int GetHashCode()
        {
            int hashCode = 1915619170;
            hashCode = hashCode * -1521134295 + Game.GetHashCode();
            hashCode = hashCode * -1521134295 + Mod.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is Version version &&
                   Game == version.Game &&
                   Mod == version.Mod;
        }

        public override string ToString()
        {
            return $"Game Version: {Game}, Mod Version: {Mod}";
        }
    }
}
