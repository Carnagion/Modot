using System.Xml;

namespace Godot.Modding.Patching.Conditions
{
    /// <summary>
    /// Represents a condition that can be tested on an <see cref="XmlNode"/>.
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Checks if a <see cref="Mod"/>'s XML data satisfies the <see cref="ICondition"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        /// <returns><see langword="true"/> if the condition succeeds, else <see langword="false"/>.</returns>
        public bool Check(XmlNode data);
    }
}