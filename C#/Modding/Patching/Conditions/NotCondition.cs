using System.Xml;

using JetBrains.Annotations;

using Godot.Serialization;

namespace Godot.Modding.Patching.Conditions
{
    /// <summary>
    /// An <see cref="ICondition"/> that succeeds if a specified condition fails.
    /// </summary>
    [PublicAPI]
    public class NotCondition : ICondition
    {
        /// <summary>
        /// Initialises a new <see cref="NotCondition"/> with the specified parameters.
        /// </summary>
        /// <param name="condition">The <see cref="ICondition"/> to check.</param>
        public NotCondition(ICondition condition)
        {
            this.Condition = condition;
        }
        
        [UsedImplicitly]
        private NotCondition()
        {
        }
        
        /// <summary>
        /// The condition to check.
        /// </summary>
        [Serialize]
        public ICondition Condition
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
        /// <summary>
        /// Succeeds if <see cref="Condition"/> fails.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        /// <returns><see langword="true"/> if <see cref="Condition"/> fails, else <see langword="false"/>.</returns>
        public bool Check(XmlNode data)
        {
            return !this.Condition.Check(data);
        }
    }
}