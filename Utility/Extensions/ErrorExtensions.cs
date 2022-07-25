using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace Godot.Utility.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="Error"/>.
    /// </summary>
    public static class ErrorExtensions
    {
        /// <summary>
        /// Checks if <paramref name="error"/> indicates success.
        /// </summary>
        /// <param name="error">The <see cref="Error"/> to check.</param>
        /// <returns><see langword="true"/> if <paramref name="error"/> is <see cref="Error.Ok"/>, else <see langword="false"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Success(this Error error)
        {
            return error is Error.Ok;
        }
        
        /// <summary>
        /// Throws an exception if <paramref name="error"/> indicates failure.
        /// </summary>
        /// <param name="error">The <see cref="Error"/> to check.</param>
        /// <exception cref="ErrorException">Thrown if <paramref name="error"/> is not <see cref="Error.Ok"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(this Error error)
        {
            if (error is not Error.Ok)
            {
                throw new ErrorException(error);
            }
        }
    }
}