using System.Xml;

using JetBrains.Annotations;

namespace Godot.Modding.Patching.Conditions
{
    /// <summary>
    /// An <see cref="ICondition"/> that checks if a particular <see cref="Mod"/> has been loaded.
    /// </summary>
    [PublicAPI]
    public class ModLoadedCondition : ICondition
    {
        /// <summary>
        /// Initialises a new <see cref="ModLoadedCondition"/> with the specified parameters.
        /// </summary>
        /// <param name="modId">The ID of the <see cref="Mod"/> to check for.</param>
        public ModLoadedCondition(string modId)
        {
            this.ModId = modId;
        }
        
        /// <summary>
        /// The ID of the <see cref="Mod"/> to check for.
        /// </summary>
        public string ModId
        {
            get;
        }
        
        /// <summary>
        /// Checks if any loaded <see cref="Mod"/>'s ID equals <see cref="ModId"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on. This is not used by the <see cref="ModLoadedCondition"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="Mod"/> given by <see cref="ModId"/> is loaded, else <see langword="false"/>.</returns>
        public bool Check(XmlNode data)
        {
            return ModLoader.LoadedMods.TryGetValue(this.ModId, out _);
        }
    }
}