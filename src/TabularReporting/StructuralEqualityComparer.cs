using System.Collections;
using System.Collections.Generic;

namespace TabularReporting
{
    /// <summary>
    /// Helper class which wraps StructuralEqualityComparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StructuralEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }

        public static StructuralEqualityComparer<T> Default => new StructuralEqualityComparer<T>();
    }
}
