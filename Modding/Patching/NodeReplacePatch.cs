using System.Xml;

using JetBrains.Annotations;

using Godot.Serialization;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// An <see cref="IPatch"/> that replaces an <see cref="XmlNode"/> with another one.
    /// </summary>
    [PublicAPI]
    public class NodeReplacePatch : IPatch
    {
        /// <summary>
        /// Initialises a new <see cref="NodeReplacePatch"/> with the specified parameters.
        /// </summary>
        /// <param name="replacement">The <see cref="XmlNode"/> to add in place of the removed <see cref="XmlNode"/>.</param>
        public NodeReplacePatch(XmlNode replacement)
        {
            this.Replacement = replacement;
        }
        
        [UsedImplicitly]
        private NodeReplacePatch()
        {
        }
        
        /// <summary>
        /// The <see cref="XmlNode"/> to add in place of the removed <see cref="XmlNode"/>.
        /// </summary>
        [Serialize]
        public XmlNode Replacement
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
        /// <summary>
        /// Removes <paramref name="data"/> and replaces it with <see cref="Replacement"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data)
        {
            XmlNode? previous = data.PreviousSibling;
            XmlNode parent = data.ParentNode!;
            XmlNode replacement = data.OwnerDocument!.ImportNode(this.Replacement, true);
            parent.RemoveChild(data);
            if (previous is null)
            {
                parent.PrependChild(replacement);
            }
            else
            {
                parent.InsertAfter(replacement, previous);
            }
        }
    }
}