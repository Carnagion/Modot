using System;
using System.Collections.Generic;
using System.Linq;

namespace Godot.Utility.Extensions
{
    public static class EnumerableExtensions
    {
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
                        bool dependenciesValid = dependencies.Invoke(t)
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