using System;

namespace Godot.Modding
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ModStartupAttribute : Attribute
    {
        public ModStartupAttribute() : this(null)
        {
        }
        
        public ModStartupAttribute(params object[]? parameters)
        {
            this.Parameters = parameters;
        }
        
        public object[]? Parameters
        {
            get;
        }
    }
}