using System.Xml;

using JetBrains.Annotations;

using Godot.Serialization;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// An <see cref="IPatch"/> that removes an attribute from an <see cref="XmlElement"/>.
    /// </summary>
    [PublicAPI]
    public class AttributeRemovePatch : IPatch
    {
        /// <summary>
        /// Initialises a new <see cref="AttributeRemovePatch"/> with the specified parameters.
        /// </summary>
        /// <param name="attribute">The name of the attribute to remove.</param>
        public AttributeRemovePatch(string attribute)
        {
            this.Attribute = attribute;
        }
        
        [UsedImplicitly]
        private AttributeRemovePatch()
        {
        }
        
        /// <summary>
        /// The name of the attribute to remove.
        /// </summary>
        [Serialize]
        public string Attribute
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
        /// <summary>
        /// Removes <see cref="Attribute"/> from <paramref name="data"/> if  <paramref name="data"/> is an <see cref="XmlElement"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data)
        {
            if (data is XmlElement element)
            {
                element.RemoveAttribute(this.Attribute);
            }
        }
    }
}