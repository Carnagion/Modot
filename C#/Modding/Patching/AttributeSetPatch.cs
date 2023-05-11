using System.Xml;

using JetBrains.Annotations;

using Godot.Serialization;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// An <see cref="IPatch"/> that sets the value of an attribute in an <see cref="XmlElement"/>.
    /// </summary>
    [PublicAPI]
    public class AttributeSetPatch : IPatch
    {
        /// <summary>
        /// Initialises a new <see cref="AttributeSetPatch"/> with the specified parameters.
        /// </summary>
        /// <param name="attribute">The name of the attribute to add/set.</param>
        /// <param name="value">The value of the attribute to add/set.</param>
        public AttributeSetPatch(string attribute, string value)
        {
            this.Attribute = attribute;
            this.Value = value;
        }
        
        [UsedImplicitly]
        private AttributeSetPatch()
        {
        }
        
        /// <summary>
        /// The name of the attribute to add/set.
        /// </summary>
        [Serialize]
        public string Attribute
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
        /// <summary>
        /// The value of the attribute to add/set.
        /// </summary>
        [Serialize]
        public string Value
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
        /// <summary>
        /// Sets <see cref="Attribute"/> to <see cref="Value"/> on <paramref name="data"/> if <paramref name="data"/> is an <see cref="XmlElement"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data)
        {
            if (data is XmlElement element)
            {
                element.SetAttribute(this.Attribute, this.Value);
            }
        }
    }
}