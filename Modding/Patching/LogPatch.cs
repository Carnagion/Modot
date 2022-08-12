using System.Xml;

using JetBrains.Annotations;

using Godot.Serialization;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// An <see cref="IPatch"/> that logs the state of any <see cref="XmlNode"/>s before and after applying a separate patch to them.
    /// </summary>
    [PublicAPI]
    public class LogPatch : IPatch
    {
        /// <summary>
        /// Initialises a new <see cref="LogPatch"/> with the specified parameters.
        /// </summary>
        /// <param name="patch">The patch to apply before and after logging the <see cref="XmlNode"/>.</param>
        public LogPatch(IPatch patch)
        { 
            this.Patch = patch;
        }
        
        private LogPatch()
        {
        }
        
        /// <summary>
        /// The patch to apply before and after logging the <see cref="XmlNode"/>.
        /// </summary>
        [Serialize]
        public IPatch Patch
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
        /// <summary>
        /// Logs the XML string representation of <paramref name="data"/> before and after applying <see cref="Patch"/> to it.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data)
        {
            Log.Write($"Before: {data.OuterXml}");
            this.Patch.Apply(data);
            Log.Write($"After: {data.OuterXml}");
        }
    }
}