using System;

namespace Errlock.Lib.Repository
{
    public enum CollectionEventType
    {
        Updated,
        Deleted
    }

    public class RepositoryCollectionChangedEventArgs<T> : EventArgs
    {
        public CollectionEventType EventType { get; private set; }
        public T CollectionItem { get; private set; }

        public RepositoryCollectionChangedEventArgs(CollectionEventType eventType, T item)
        {
            this.EventType = eventType;
            this.CollectionItem = item;
        }
    }
}