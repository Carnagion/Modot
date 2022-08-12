using System;

using JetBrains.Annotations;

namespace Godot.Utility
{
    /// <summary>
    /// A wrapper around <see cref="Godot.Error"/> to make it throwable.
    /// </summary>
    [PublicAPI]
    public class ErrorException : Exception
    {
        /// <summary>
        /// Initialises a new <see cref="ErrorException"/> with the specified <see cref="Godot.Error"/>.
        /// </summary>
        /// <param name="error">The Godot <see cref="Godot.Error"/>.</param>
        public ErrorException(Error error) : base(error.ToString())
        {
            this.Error = error;
        }
        
        /// <summary>
        /// The Godot <see cref="Godot.Error"/>.
        /// </summary>
        public Error Error
        {
            get;
        }
    }
}