using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiP.Blazor.Behaviors
{
    internal class ReferenceComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
