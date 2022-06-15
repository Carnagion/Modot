using System;

namespace Godot.Modding
{
    /// <summary>
    /// Indicates that the marked method is to be invoked after the loading of the <see cref="Mod"/> assemblies in which it is contained.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ModStartupAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new <see cref="ModStartupAttribute"/>.
        /// </summary>
        public ModStartupAttribute() : this(null)
        {
        }
        
        /// <summary>
        /// Initializes a new <see cref="ModStartupAttribute"/> with the specified arguments.
        /// </summary>
        /// <param name="parameters">The parameters to supply to the marked method when invoking it.</param>
        public ModStartupAttribute(params object[]? parameters)
        {
            this.Parameters = parameters;
        }
        
        /// <summary>
        /// The parameters that are supplied to the marked method when invoking it.
        /// </summary>
        public object[]? Parameters
        {
            get;
        }
    }
}