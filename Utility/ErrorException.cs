using System;

namespace Godot.Utility
{
    public class ErrorException : Exception
    {
        public ErrorException(Error error) : base(error.ToString())
        {
            this.Error = error;
        }

        public Error Error
        {
            get;
        }
    }
}