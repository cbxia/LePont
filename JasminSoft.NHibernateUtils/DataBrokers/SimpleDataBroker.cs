﻿using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;

namespace JasminSoft.NHibernateUtils
{
    /// <summary>
    /// The SimpleDataBroker class is intended to be used as a common-purpose data broker for the simplest CRUD operations.
    /// </summary>
    public class SimpleDataBroker : BaseDataBroker
    {
        public SimpleDataBroker()
            : base()
        {
        }

        public SimpleDataBroker(SessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public T GetById<T>(object id)
            where T : class
        {
            return GetById<T>(id, false);
        }

        public T GetById<T>(object id, bool acquireLock)
            where T : class
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

        public IList<T> GetAll<T>()
            where T : class
        {
            string queryString = "from " + typeof(T).Name;
            return PerformQueryAction<T>(queryString);
        }

        public virtual IList<T> GetAllValid<T>()
            where T : class
        {
            string queryString = "from " + typeof(T).Name + " as instance where instance.Deactivated = false";
            return PerformQueryAction<T>(queryString);
        }

        public T Save<T>(T entity)
            where T : class
        {
            DataAction<T> action = delegate(ISession session)
            {
                return (T)session.SaveOrUpdateCopy(entity);
            };
            return PerformDataAction<T>(action);
        }

        public void Delete<T>(T entity)
            where T : class
        {
            DataAction action = delegate(ISession session)
            {
                session.Delete(entity);
            };
            PerformDataAction(action);
        }
    }
}