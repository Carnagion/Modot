using System.Collections.Generic;
using System.Linq;
using System.Xml;

using JetBrains.Annotations;

using Godot.Serialization;

namespace Godot.Modding.Patching.Conditions
{
    /// <summary>
    /// An <see cref="ICondition"/> that succeeds if all of a specified sequence of conditions succeed.
    /// </summary>
    [PublicAPI]
    public class AndCondition : ICondition
    {
        /// <summary>
        /// Initialises a new <see cref="AndCondition"/> with the specified parameters.
        /// </summary>
        /// <param name="conditions">The conditions to check.</param>
        public AndCondition(IEnumerable<ICondition> conditions)
        {
            this.Conditions = conditions;
        }
        
        [UsedImplicitly]
        private AndCondition()
        {
        }
        
        /// <summary>
        /// The conditions to check.
        /// </summary>
        [Serialize]
        public IEnumerable<ICondition> Conditions
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
        /// <summary>
        /// Succeeds if all conditions in <see cref="Conditions"/> succeed.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        /// <returns><see langword="true"/> if all conditions in <see cref="Conditions"/> succeed, else <see langword="false"/>.</returns>
        public bool Check(XmlNode data)
        {
            return this.Conditions.All(condition => condition.Check(data));
        }
    }
}