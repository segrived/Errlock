using System;
using System.Collections.Generic;
using System.IO;
using Errlock.Lib.Helpers;
using Errlock.Lib.Repository;
using LiteDB;

namespace Errlock.Lib.Sessions
{
    public sealed class SessionLiteDbRepository : IRepository<Session>, IRepositoryCollectionChanged<Session>
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
            this.OnCollectionChanged(CollectionEventType.Updated, item);
        }
        
        public void Delete(Session item)
        {
            this._collection.Delete(item.Id);
            this.OnCollectionChanged(CollectionEventType.Deleted, item);
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

        public event EventHandler<RepositoryCollectionChangedEventArgs<Session>> CollectionChanged;

        private void OnCollectionChanged(CollectionEventType eventType, Session item)
        {
            var handler = CollectionChanged;
            handler.Raise(this, new RepositoryCollectionChangedEventArgs<Session>(eventType, item));
        }
    }
}
