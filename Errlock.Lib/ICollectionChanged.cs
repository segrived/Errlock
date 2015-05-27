using System;

namespace Errlock.Lib
{
    public enum CollectionChangeType
    {
        Updated, Deleted
    }

    public interface ICollectionChanged<T>
    {
        event EventHandler<ItemChangedEventArgs<T>> CollectionChanged;
    }
}
