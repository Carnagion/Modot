using System.Collections.Generic;
using System.Linq;
using System.Xml;

using JetBrains.Annotations;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// An <see cref="IPatch"/> that can be used to apply multiple patches in sequence onto the same <see cref="XmlNode"/>.
    /// </summary>
    [PublicAPI]
    public class MultiPatch : IPatch
    {
        /// <summary>
        /// Initialises a new <see cref="MultiPatch"/> with the specified parameters.
        /// </summary>
        /// <param name="patches">The patches to apply in sequence.</param>
        public MultiPatch(IEnumerable<IPatch> patches)
        {
            this.Patches = patches;
        }
        
        /// <summary>
        /// The patches to apply in sequence.
        /// </summary>
        public IEnumerable<IPatch> Patches
        {
            get;
        }
        
        /// <summary>
        /// Applies all patches in <see cref="Patches"/> to <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data)
        {
            this.Patches.ForEach(patch => patch.Apply(data));
        }
    }
}