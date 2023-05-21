using System.Xml;

using JetBrains.Annotations;

using Godot.Modding.Patching.Conditions;
using Godot.Serialization;

namespace Godot.Modding.Patching
{
    /// <summary>
    /// An <see cref="IPatch"/> that can apply either a "success" or a "failure" patch to an <see cref="XmlNode"/> depending on a condition.
    /// </summary>
    [PublicAPI]
    public class ConditionalPatch : IPatch
    {
        /// <summary>
        /// Initialises a new <see cref="ConditionalPatch"/> with the specified parameters.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="success">The patch to apply if <paramref name="condition"/> succeeds.</param>
        /// <param name="failure">The patch to apply if <paramref name="condition"/> fails.</param>
        public ConditionalPatch(ICondition condition, IPatch? success, IPatch? failure)
        {
            this.Condition = condition;
            this.Success = success;
            this.Failure = failure;
        }
        
        [UsedImplicitly]
        private ConditionalPatch()
        {
        }

        /// <summary>
        /// The condition to check when applying the <see cref="ConditionalPatch"/>.
        /// </summary>
        [Serialize]
        public ICondition Condition
        {
            get;
            [UsedImplicitly]
            private set;
        } = null!;
        
        /// <summary>
        /// The <see cref="IPatch"/> applied if <see cref="Condition"/> succeeds.
        /// </summary>
        public IPatch? Success
        {
            get;
            [UsedImplicitly]
            private set;
        }
        
        /// <summary>
        /// The <see cref="IPatch"/> applied if <see cref="Condition"/> fails.
        /// </summary>
        public IPatch? Failure
        {
            get;
            [UsedImplicitly]
            private set;
        }
        
        /// <summary>
        /// Applies either <see cref="Success"/> or <see cref="Failure"/> to <paramref name="data"/> depending on <see cref="Condition"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlNode"/> to apply the patch on.</param>
        public void Apply(XmlNode data)
        {
            IPatch? patch = this.Condition.Check(data) ? this.Success : this.Failure;
            patch?.Apply(data);
        }
    }
}