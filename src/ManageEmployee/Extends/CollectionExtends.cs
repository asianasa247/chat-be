namespace ManageEmployee.Extends;

public static class CollectionExtends
{
    public static IEnumerable<T> TraverseX<T>(
    this IEnumerable<T> source,
    Func<T, IEnumerable<T>> fnRecurse)
    {
        if (source != null)
        {
            Stack<IEnumerator<T>> enumerators = new Stack<IEnumerator<T>>();
            try
            {
                enumerators.Push(source.GetEnumerator());
                while (enumerators.Count > 0)
                {
                    var top = enumerators.Peek();
                    while (top.MoveNext())
                    {
                        yield return top.Current;

                        var children = fnRecurse(top.Current);
                        if (children != null)
                        {
                            top = children.GetEnumerator();
                            enumerators.Push(top);
                        }
                    }

                    enumerators.Pop().Dispose();
                }
            }
            finally
            {
                while (enumerators.Count > 0)
                    enumerators.Pop().Dispose();
            }
        }
    }

    public static void Prepend<T>(this List<T> source, T element) => source.Insert(0, element);
}
