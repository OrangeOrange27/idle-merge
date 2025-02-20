
namespace System.Collections.Generic.Enumerable
{
    interface IEnumerableIList<T> : IEnumerable<T>
    {
        new EnumeratorIList<T> GetEnumerator();
    }
}
