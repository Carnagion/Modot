using System.Xml;

using JetBrains.Annotations;

using Godot.Serialization;

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
        /// <param name="index">The index to insert <paramref name="value"/> at, or -1 if it should simply be appended to the end.</param>
        public NodeAddPatch(XmlNode value, int index = -1)
        {
            this.Value = value;
            this.Index = index;
        }
        
        [UsedImplicitly]
        private NodeAddPatch()
        {
        }
        
        /// <summary>
        /// The <see cref="XmlNode"/> to add as a child.
        /// </summary>
        [Serialize]
        public XmlNode Value
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
        /// <summary>
        /// The index to insert <see cref="Value"/> at.
        /// </summary>
        public int Index
        {
            get;
            [UsedImplicitly]
            private set;
        } = -1;
        
        /// <summary>
        /// Adds <see cref="Value"/> as a child to <paramref name="data"/> at the index specified by <see cref="Index"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data)
        {
            XmlNode value = data.OwnerDocument!.ImportNode(this.Value, true);
            switch (this.Index)
            {
                case < 0:
                    data.AppendChild(value);
                    break;
                case 0:
                    data.PrependChild(value);
                    break;
                default:
                    data.InsertBefore(value, data.ChildNodes[this.Index]);
                    break;
            }
        }
    }
}