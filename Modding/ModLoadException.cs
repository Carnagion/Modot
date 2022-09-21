using System;

namespace Godot.Modding
{
    /// <summary>
    /// The exception thrown when an error occurs while loading a <see cref="Mod"/>.
    /// </summary>
    public class ModLoadException : Exception
    {
        /// <summary>
        /// Initializes a new <see cref="ModLoadException"/> with the specified arguments.
        /// </summary>
        /// <param name="directoryPath">The directory path from where an attempt was made to load the <see cref="Mod"/>.</param>
        /// <param name="message">A brief description of the issue.</param>
        public ModLoadException(string directoryPath, string message) : base($"Could not load mod at {directoryPath}: {message}")
        {
        }
        
        /// <summary>
        /// Initializes a new <see cref="ModLoadException"/> with the specified arguments.
        /// </summary>
        /// <param name="directoryPath">The directory path from where an attempt was made to load the <see cref="Mod"/>.</param>
        /// <param name="cause">The <see cref="Exception"/> that caused the loading to fail.</param>
        public ModLoadException(string directoryPath, Exception cause) : base($"Could not load mod at {directoryPath}.{System.Environment.NewLine}{cause}")
        {
        }
    }
}