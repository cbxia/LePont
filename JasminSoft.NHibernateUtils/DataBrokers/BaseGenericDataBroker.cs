using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace JasminSoft.NHibernateUtils
{
    /// <summary>
    /// The BaseGenericDataBroker class is used as the base class for customized data broker classes.
    /// </summary>
    /// <typeparam name="T">The entity class handled by the data broker class.</typeparam>
    public abstract class BaseGenericDataBroker<T, KeyType> : BaseDataBroker
        where T : class, new()
    {
        public BaseGenericDataBroker()
            : base()
        {
        }

        public BaseGenericDataBroker(SessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public T GetById(KeyType id)
        {
            return GetById(id, false);
        }

        public T GetById(KeyType id, bool acquireLock)
        {
            DataAction<T> action = delegate(ISession session)
            {
                if (acquireLock)
                    return session.Get<T>(id, LockMode.Upgrade);
                else
                    return session.Get<T>(id);

            };
            return PerformDataAction<T>(action);
        }

        public virtual IList<T> GetAll()
        {
            string queryString = "from " + typeof(T).Name;
            return PerformQueryAction<T>(queryString);
        }

        public virtual IList<T> GetAllValid()
        {
            string queryString = "from " + typeof(T).Name + " as instance where instance.Deactivated = false";
            return PerformQueryAction<T>(queryString);
        }

        public T Save(T entity)
        {
            DataAction<T> action = delegate(ISession session)
            {
                return (T)session.SaveOrUpdateCopy(entity);
            };
            return PerformDataAction<T>(action);
        }

        public void Delete(T entity)
        {
            DataAction action = delegate(ISession session)
            {
                _session.Delete(entity);
            };
            PerformDataAction(action);
        }
    }
}
