using System;

namespace Godot.Modding
{
    public class ModLoadException : Exception
    {
        public ModLoadException(string directoryPath, string message) : base($"Could not load mod at {directoryPath}: {message}")
        {
        }
        
        public ModLoadException(string directoryPath, Exception cause) : base($"Could not load mod at {directoryPath}.{System.Environment.NewLine}{cause}")
        {
        }
    }
}