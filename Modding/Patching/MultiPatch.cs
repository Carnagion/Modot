using System.Collections.Generic;
using System.Linq;
using System.Xml;

using JetBrains.Annotations;

using Godot.Serialization;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// An <see cref="IPatch"/> that applies multiple patches in sequence onto the same <see cref="XmlNode"/>.
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
        
        [UsedImplicitly]
        private MultiPatch()
        {
        }
        
        /// <summary>
        /// The patches to apply in sequence.
        /// </summary>
        [Serialize]
        public IEnumerable<IPatch> Patches
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
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