using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

using Godot.Modding.Utility.Extensions;

namespace Godot.Modding
{
    /// <summary>
    /// Provides methods and properties for loading <see cref="Mod"/>s at runtime, obtaining all loaded <see cref="Mod"/>s, and finding a loaded <see cref="Mod"/> by its ID.
    /// </summary>
    [PublicAPI]
    public static class ModLoader
    {
        private static readonly Dictionary<string, Mod> loadedMods = new();
        
        /// <summary>
        /// All the <see cref="Mod"/>s that have been loaded at runtime.
        /// </summary>
        public static IReadOnlyDictionary<string, Mod> LoadedMods
        {
            get
            {
                return ModLoader.loadedMods;
            }
        }
        
        /// <summary>
        /// Loads a <see cref="Mod"/> from <paramref name="modDirectoryPath"/> and runs all methods marked with <see cref="ModStartupAttribute"/> in its assemblies if specified.
        /// </summary>
        /// <param name="modDirectoryPath">The directory path containing the <see cref="Mod"/>'s metadata, assemblies, data, and resource packs.</param>
        /// <param name="executeAssemblies">Whether any code in any assemblies of the loaded <see cref="Mod"/> gets executed.</param>
        /// <returns>The <see cref="Mod"/> loaded from <paramref name="modDirectoryPath"/>.</returns>
        /// <remarks>This method only loads a <see cref="Mod"/> individually, and does not check whether it has been loaded with all dependencies and in the correct load order. To load multiple <see cref="Mod"/>s in a safe and orderly manner, <see cref="LoadMods"/> should be used.</remarks>
        public static Mod LoadMod(string modDirectoryPath, bool executeAssemblies = true)
        {
            Mod mod = new(Mod.Metadata.Load(modDirectoryPath));
            ModLoader.loadedMods.Add(mod.Meta.Id, mod);
            if (executeAssemblies)
            {
                ModLoader.StartupMod(mod);
            }
            return mod;
        }
        
        /// <summary>
        /// Loads <see cref="Mod"/>s from <paramref name="modDirectoryPaths"/> and runs all methods marked with <see cref="ModStartupAttribute"/> in their assemblies if specified.
        /// </summary>
        /// <param name="modDirectoryPaths">The directory paths to load the <see cref="Mod"/>s from, containing each <see cref="Mod"/>'s metadata, assemblies, data, and resource packs.</param>
        /// <param name="executeAssemblies">Whether any code in any assemblies of the loaded <see cref="Mod"/>s gets executed.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of the loaded <see cref="Mod"/>s in the correct load order. <see cref="Mod"/>s that could not be loaded due to issues will not be contained in the sequence.</returns>
        /// <remarks>This method loads multiple <see cref="Mod"/>s after sorting them according to the load order specified in their metadata. To load a <see cref="Mod"/> individually without regard to its dependencies and load order, <see cref="LoadMod"/> should be used.</remarks>
        public static IEnumerable<Mod> LoadMods(IEnumerable<string> modDirectoryPaths, bool executeAssemblies = true)
        {
            List<Mod> mods = ModLoader.SortModMetadata(ModLoader.FilterModMetadata(ModLoader.LoadModMetadata(modDirectoryPaths)))
                .Select(metadata => new Mod(metadata))
                .ToList();
            mods.ForEach(mod => ModLoader.loadedMods.Add(mod.Meta.Id, mod));
            if (executeAssemblies)
            {
                mods.ForEach(ModLoader.StartupMod);
            }
            return mods;
        }

        private static void StartupMod(Mod mod)
        {
            // Invoke all static methods annotated with [Startup] along with the supplied parameters (if any)
            mod.Assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public))
                .Select(method => (method, method.GetCustomAttribute<ModStartupAttribute>()))
                .Where(pair => pair.Item2 is not null)
                .ForEach(pair => pair.Item1.Invoke(null, pair.Item2.Parameters));
        }
        
        [MustUseReturnValue]
        private static Dictionary<string, Mod.Metadata> LoadModMetadata(IEnumerable<string> modDirectories)
        {
            Dictionary<string, Mod.Metadata> loadedMetadata = new();
            foreach (string modDirectory in modDirectories)
            {
                Mod.Metadata metadata = Mod.Metadata.Load(modDirectory);
                
                // Fail if the metadata is incompatible with any of the loaded metadata (and vice-versa), or if the ID already exists
                IEnumerable<Mod.Metadata> incompatibleMetadata = metadata.Incompatible
                    .Select(id => loadedMetadata.GetValueOrDefault(id))
                    .NotNull()
                    .Concat(loadedMetadata.Values.Where(loaded => loaded.Incompatible.Contains(metadata.Id)));
                if (incompatibleMetadata.Any())
                {
                    Log.Error(new ModLoadException(metadata.Directory, "Incompatible with other loaded mods"));
                }
                else if (!loadedMetadata.TryAdd(metadata.Id, metadata))
                {
                    Log.Error(new ModLoadException(metadata.Directory, "Duplicate ID"));
                }
            }
            return loadedMetadata;
        }
        
        [MustUseReturnValue]
        private static Dictionary<string, Mod.Metadata> FilterModMetadata(Dictionary<string, Mod.Metadata> loadedMetadata)
        {
            // If the dependencies of any metadata have not been loaded, remove that metadata and try again
            IEnumerable<Mod.Metadata> invalidMetadata = loadedMetadata.Values
                .Where(metadata => metadata.Dependencies
                    .Select(dependency => loadedMetadata.TryGetValue(dependency, out _))
                    .Any(dependency => !dependency));
            foreach (Mod.Metadata metadata in invalidMetadata)
            {
                Log.Error(new ModLoadException(metadata.Directory, "Not all dependencies are loaded"));
                loadedMetadata.Remove(metadata.Id);
                return ModLoader.FilterModMetadata(loadedMetadata);
            }
            return loadedMetadata;
        }
        
        [MustUseReturnValue]
        private static IEnumerable<Mod.Metadata> SortModMetadata(Dictionary<string, Mod.Metadata> filteredMetadata)
        {
            // Create a graph of each metadata ID and the IDs of those that need to be loaded after it
            Dictionary<string, HashSet<string>> dependencyGraph = new();
            foreach (Mod.Metadata metadata in filteredMetadata.Values)
            {
                dependencyGraph.TryAdd(metadata.Id, new());
                metadata.After.ForEach(after => dependencyGraph[metadata.Id].Add(after));
                foreach (string before in metadata.Before)
                {
                    dependencyGraph.TryAdd(before, new());
                    dependencyGraph[before].Add(metadata.Id);
                }
            }
            
            // Topologically sort the dependency graph, removing cyclic dependencies if any
            IEnumerable<string>? sortedMetadataIds = dependencyGraph.Keys.TopologicalSort(id => dependencyGraph.GetValueOrDefault(id) ?? Enumerable.Empty<string>(), cyclic =>
            {
                Log.Error(new ModLoadException(filteredMetadata[cyclic].Directory, "Cyclic dependencies with other mod(s)"));
                filteredMetadata.Remove(cyclic);
            });
            
            // If there is no valid topological sorting (cyclic dependencies detected), remove the cyclic metadata and try again
            return sortedMetadataIds?
                .Select(filteredMetadata.GetValueOrDefault)
                .NotNull() ?? ModLoader.SortModMetadata(ModLoader.FilterModMetadata(filteredMetadata));
        }
    }
}