using System.Xml;

using JetBrains.Annotations;

namespace Godot.Modding.Patching.Conditions
{
    /// <summary>
    /// An <see cref="ICondition"/> that checks if a descendant of an <see cref="XmlNode"/> matching an XPath string exists.
    /// </summary>
    [PublicAPI]
    public class NodeExistsCondition : ICondition
    {
        /// <summary>
        /// Initialises a new <see cref="NodeExistsCondition"/> with the specified parameters.
        /// </summary>
        /// <param name="xPath">The XPath string to check.</param>
        public NodeExistsCondition(string xPath)
        {
            this.XPath = xPath;
        }
        
        /// <summary>
        /// The XPath string to use when checking if an <see cref="XmlNode"/> exists.
        /// </summary>
        public string XPath
        {
            get;
        }
        
        /// <summary>
        /// Checks if any <see cref="XmlNode"/>s match the XPath given by <see cref="XPath"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        /// <returns><see langword="true"/> if <see cref="XPath"/> selects at least one <see cref="XmlNode"/>, else <see langword="false"/>.</returns>
        public bool Check(XmlNode data)
        {
            return data.SelectSingleNode(this.XPath) is not null;
        }
    }
}