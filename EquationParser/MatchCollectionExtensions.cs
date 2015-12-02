using System.Collections;
using System.Collections.Generic;

namespace Equation
{
    public static class EnumberableExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return (T)enumerator.Current;
            }
        }
    }
}
