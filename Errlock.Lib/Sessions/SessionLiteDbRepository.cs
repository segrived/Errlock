using System;
using System.Collections.Generic;
using System.IO;
using Errlock.Lib.Helpers;
using LiteDB;

namespace Errlock.Lib.Sessions
{
    public sealed class SessionLiteDbRepository : IRepository<Session>, ICollectionChanged<Session>
    {
        private static readonly string DbDefaultFileName =
            Path.Combine(AppHelpers.DefaultConfigPath, "Errlock.db");

        private readonly LiteCollection<Session> _collection;

        public SessionLiteDbRepository() : this(DbDefaultFileName)
        {
        }

        public SessionLiteDbRepository(string dbFileName)
        {
            this._collection = new LiteDatabase(dbFileName).GetCollection<Session>();
        }

        public void InsertOrUpdate(Session item)
        {
            this._collection.InsertOrUpdate(item);
            this.OnCollectionChanged(CollectionChangeType.Updated, item);
        }
        
        public void Delete(Session item)
        {
            this._collection.Delete(item.Id);
            this.OnCollectionChanged(CollectionChangeType.Deleted, item);
        }

        public IEnumerable<Session> EnumerateAll()
        {
            return this._collection.FindAll();
        }

        public Session GetItemById(Guid id)
        {
            return this._collection.FindById(id);
        }

        public bool Exists(Guid id)
        {
            return this._collection.FindById(id) != null;
        }

        public event EventHandler<ItemChangedEventArgs<Session>> CollectionChanged;

        private void OnCollectionChanged(CollectionChangeType changeType, Session item)
        {
            var handler = CollectionChanged;
            handler.Raise(this, new ItemChangedEventArgs<Session>(changeType, item));
        }
    }
}
