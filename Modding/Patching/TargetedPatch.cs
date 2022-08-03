using System.Linq;
using System.Xml;

using JetBrains.Annotations;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// An <see cref="IPatch"/> that can be used to select descendants of an <see cref="XmlNode"/> according to an XPath string and apply a separate patch on them.
    /// </summary>
    [PublicAPI]
    public class TargetedPatch : IPatch
    {
        /// <summary>
        /// Initialises a new <see cref="TargetedPatch"/> with the specified parameters.
        /// </summary>
        /// <param name="targets">An XPath string that specifies descendant <see cref="XmlNode"/>s to apply <paramref name="patch"/> on.</param>
        /// <param name="patch">The patch to apply on all <see cref="XmlNode"/>s selected by <paramref name="targets"/>.</param>
        public TargetedPatch(string targets, IPatch patch)
        {
            this.Targets = targets;
            this.Patch = patch;
        }
        
        /// <summary>
        /// The targets to apply the <see cref="TargetedPatch"/> on, in the form of an XPath.
        /// </summary>
        public string Targets
        {
            get;
        }
        
        /// <summary>
        /// The patch to apply on <see cref="XmlNode"/>s that match <see cref="Targets"/>.
        /// </summary>
        public IPatch Patch
        {
            get;
        }
        
        /// <summary>
        /// Applies <see cref="Patch"/> to all <see cref="XmlNode"/>s under <paramref name="data"/> that match <see cref="Targets"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data)
        {
            data.SelectNodes(this.Targets)?
                .Cast<XmlNode>()
                .ForEach(this.Patch.Apply);
        }
    }
}