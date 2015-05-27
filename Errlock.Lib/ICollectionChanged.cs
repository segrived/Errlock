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

    public class ItemChangedEventArgs<T> : EventArgs
    {
        public CollectionChangeType ChangeType { get; private set; }
        public T CollectionItem { get; private set; }

        public ItemChangedEventArgs(CollectionChangeType changeType, T item)
        {
            this.ChangeType = changeType;
            this.CollectionItem = item;
        }
    }
}
