using System;

namespace Errlock.Lib.Repository
{
    public interface IRepositoryCollectionChanged<T>
    {
        event EventHandler<RepositoryCollectionChangedEventArgs<T>> CollectionChanged;
    }
}
