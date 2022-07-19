using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Godot.Modding.Utility.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Topologically sorts the given sequence of elements.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to sort.</param>
        /// <param name="dependencies">A <see cref="Func{T,TResult}"/> that returns an <see cref="IEnumerable{T}"/> of dependencies for each element in <paramref name="source"/>.</param>
        /// <param name="cyclic">An optional <see cref="Action{T}"/> that is invoked if a cyclic dependency is found while sorting.</param>
        /// <typeparam name="T">The <see cref="Type"/> of elements in <paramref name="source"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of elements from <paramref name="source"/> sorted topologically, or <see langword="null"/> if no valid topological sorting exists.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either <paramref name="source"/> or <paramref name="dependencies"/> is <see langword="null"/>.</exception>
        [MustUseReturnValue]
        public static IEnumerable<T>? TopologicalSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies, Action<T>? cyclic = null)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (dependencies is null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            List<T> sorted = new();
            Dictionary<T, bool?> states = new();

            bool allValid = source
                .Select(VisitDependencies)
                .All(boolean => boolean);
            return allValid ? sorted : null;

            bool VisitDependencies(T t)
            {
                states.TryAdd(t, false);
                switch (states[t])
                {
                    case true:
                        return true;
                    case false:
                        states[t] = null;
                        bool dependenciesValid = dependencies
                            .Invoke(t)
                            .Select(VisitDependencies)
                            .All(boolean => boolean);
                        if (!dependenciesValid)
                        {
                            return false;
                        }
                        states[t] = true;
                        sorted.Add(t);
                        return true;
                    case null:
                        cyclic?.Invoke(t);
                        return false;
                }
            }
        }
    }
}