using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class IEnumerableExtensions
{
    #region Methods

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
        {
            throw new ArgumentNullException("source");
        }

        if (action == null)
        {
            throw new ArgumentNullException("action");
        }

        foreach (T item in source)
        {
            action(item);
        }
    }

    #endregion Methods
}