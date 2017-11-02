using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Linq;
using AnimePictureETL.Databases;

namespace AnimePictureETL.Repositories
{
    public class RepositoryBase : IRepository, IDisposable
    {
        protected ISession _session = null;
        protected ITransaction _transaction = null;

        public RepositoryBase()
        {
            _session = AnimeWorksDatabase.OpenSession();
        }

        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
            CloseTransaction();
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
            CloseTransaction();
            CloseSession();
        }

        private void CloseTransaction()
        {
            _transaction.Dispose();
            _transaction = null;
        }

        private void CloseSession()
        {
            _session.Close();
            _session.Dispose();
            _session = null;
        }

        public virtual void Save(object obj)
        {
            _session.SaveOrUpdate(obj);
        }

        public virtual void Delete(object obj)
        {
            _session.Delete(obj);
        }

        public virtual object GetById(Type objType, object objId)
        {
            return _session.Load(objType, objId);
        }

        public virtual bool Exists(object obj)
        {
            return _session.Contains(obj);
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                CommitTransaction();
            }
            if (_session != null)
            {
                _session.Flush();
                CloseSession();
            }
        }
    }
}