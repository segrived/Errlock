using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Errlock.Lib.Sessions
{
    public class SessionLiteDbRepository : IRepository<Session>
    {
        private const string DbFileName = "Errlock.db";
        private readonly LiteCollection<Session> _collection = 
            new LiteDatabase(DbFileName).GetCollection<Session>("sessions");

        public void InsertOrUpdate(Session item)
        {
            this._collection.Insert(item);
        }

        public void Delete(Session item)
        {
            this._collection.Delete(item.Id);
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
    }
}
