using System.Xml;

using JetBrains.Annotations;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// An <see cref="IPatch"/> that adds an <see cref="XmlNode"/> as a child to another <see cref="XmlNode"/>.
    /// </summary>
    [PublicAPI]
    public class NodeAddPatch : IPatch
    {
        /// <summary>
        /// Initialises a new <see cref="NodeAddPatch"/> with the specified parameters.
        /// </summary>
        /// <param name="value">The <see cref="XmlNode"/> to add as a child.</param>
        public NodeAddPatch(XmlNode value)
        {
            this.Value = value;
        }
        
        /// <summary>
        /// The <see cref="XmlNode"/> to add as a child.
        /// </summary>
        public XmlNode Value
        {
            get;
        }
        
        /// <summary>
        /// Adds <see cref="Value"/> as a child to <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data)
        {
            data.AppendChild(data.OwnerDocument!.ImportNode(this.Value, true));
        }
    }
}