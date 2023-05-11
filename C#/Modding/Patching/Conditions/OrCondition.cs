using System.Collections.Generic;
using System.Linq;
using System.Xml;

using JetBrains.Annotations;

using Godot.Serialization;

namespace Godot.Modding.Patching.Conditions
{
    /// <summary>
    /// An <see cref="ICondition"/> that succeeds if at least one of a specified sequence of conditions succeed.
    /// </summary>
    [PublicAPI]
    public class OrCondition : ICondition
    {
        /// <summary>
        /// Initialises a new <see cref="OrCondition"/> with the specified parameters.
        /// </summary>
        /// <param name="conditions">The conditions to check.</param>
        public OrCondition(IEnumerable<ICondition> conditions)
        {
            this.Conditions = conditions;
        }
        
        [UsedImplicitly]
        private OrCondition()
        {
        }
        
        /// <summary>
        /// The conditions to check.
        /// </summary>
        public IEnumerable<ICondition> Conditions
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
        /// <summary>
        /// Succeeds if at least one condition in <see cref="Conditions"/> succeeds.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        /// <returns><see langword="true"/> if at least one condition in <see cref="Conditions"/> succeeds, else <see langword="false"/>.</returns>
        public bool Check(XmlNode data)
        {
            return this.Conditions.Any(condition => condition.Check(data));
        }
    }
}