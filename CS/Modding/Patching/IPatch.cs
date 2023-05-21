using System.Xml;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// Represents a modification that can be applied to the XML data of a <see cref="Mod"/>.
    /// </summary>
    public interface IPatch
    {
        /// <summary>
        /// Applies the patch to <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data);
    }
}