using System.Xml;

using JetBrains.Annotations;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// An <see cref="IPatch"/> that removes the <see cref="XmlNode"/> it is applied on.
    /// </summary>
    [PublicAPI]
    public class NodeRemovePatch : IPatch
    {
        /// <summary>
        /// Removes <paramref name="data"/> from its parent <see cref="XmlNode"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data)
        {
            data.ParentNode!.RemoveChild(data);
        }
    }
}