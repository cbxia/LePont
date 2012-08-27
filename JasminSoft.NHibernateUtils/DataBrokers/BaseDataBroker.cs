using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;

namespace JasminSoft.NHibernateUtils
{
    /// <summary>
    /// The BaseDataBroker class is used as the base class for customized data broker classes.
    /// 
    /// If no external SessionContext is passed in, each data access operation will
    /// automatically create an impromptu SessionContext on the fly, execute database
    /// request within this SessionContext, and close it immediately. This ensures
    /// the shortest possible calling syntax for the most common usage scenarios.
    /// 
    /// On the other hand, if the caller does pass in an external SessionContext, it is the caller's
    /// responsibility to take care of and close this SessionContext.
    /// 
    /// </summary>
    public abstract class BaseDataBroker
    {
        #region Fields

        protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(BaseDataBroker));
        protected const string NHIBERNATE_FAIL_LOGGING_MESSAGE = "Failed accessing data via NHibernate.";
        private SessionContext _sessionContext;
        private int _sessionContextRefCount = 0;

        #endregion

        #region Constructors

        public BaseDataBroker()
        {
        }

        public BaseDataBroker(SessionContext sessionContext)
        {
            if (sessionContext != null)
            {
                _sessionContext = sessionContext;
            }
        }

        #endregion

        #region Implementation helpers

        //TODO: Replace the custom delegates below with built-in generic delegates defined in System namespace.

        protected delegate void DataAction(ISession session); // Action<ISession> 
        protected delegate ResultType DataAction<ResultType>(ISession session); // Func<ISession, TResult>
        protected delegate IQuery QueryDecorator(IQuery query); // Func<IQuery, IQuery>

        protected void PerformDataAction(DataAction action)
        {
            // TODO: Retrieve the calling method info in the StackTrace, and format tracing message like below:
            ////////////////////
            //string logSource = string.Format("{0}.GetById({1}, {2}) ", this.GetType().ToString(), id, acquireLock);
            //if (_log.IsDebugEnabled)
            //  _log.Debug(">>>>Issuing call " + logSource + ":");
            ////////////////////
            try
            {
                if (_sessionContext == null || _sessionContext.Session == null)
                {
                    // Use impromptu session.
                    using (SessionContext ctx = new SessionContext())
                    {
                        action(ctx.Session);
                    }
                }
                else
                {
                    action(_sessionContext.Session);
                }

            }
            catch (Exception ex)
            {
                ////////////////////
                _log.Error(NHIBERNATE_FAIL_LOGGING_MESSAGE, ex);
                ////////////////////
                throw ex;
            }
        }

        protected ResultType PerformDataAction<ResultType>(DataAction<ResultType> action)
        {
            ResultType result = default(ResultType);
            try
            {
                if (_sessionContext == null || _sessionContext.Session == null)
                {
                    // Use impromptu session.
                    using (SessionContext ctx = new SessionContext())
                    {
                        result = action(ctx.Session);
                    }
                }
                else
                {
                    result = action(_sessionContext.Session);
                }

            }
            catch (Exception ex)
            {
                ////////////////////
                _log.Error(NHIBERNATE_FAIL_LOGGING_MESSAGE, ex);
                ////////////////////
                throw ex;
            }
            return result;
        }


        protected IList<ItemType> PerformQueryAction<ItemType>(string queryString, QueryDecorator queryDecorator)
        {
            return PerformDataAction<IList<ItemType>>(session =>
            {
                IQuery query = session.CreateQuery(queryString);
                if (queryDecorator != null)
                    query = queryDecorator(query);
                return query.List<ItemType>();
            });
            /*
            try
            {
                if (_sessionContext == null)
                {
                    // Use impromptu session.
                    using (SessionContext ctx = new SessionContext())
                    {
                        IQuery query = ctx.Session.CreateQuery(queryString);
                        if (queryDecorator != null)
                            query = queryDecorator(query);
                        result = query.List<ItemType>();
                    }
                }
                else
                {
                    IQuery query = _sessionContext.Session.CreateQuery(queryString);
                    if (queryDecorator != null)
                        query = queryDecorator(query);
                    result = query.List<ItemType>();
                }

            }
            catch (Exception ex)
            {
                ////////////////////
                _log.Error(NHIBERNATE_FAIL_LOGGING_MESSAGE, ex);
                ////////////////////
                throw ex;
            }
            
            return result;
            */
        }

        protected IList<ItemType> PerformQueryAction<ItemType>(string queryString)
        {
            return PerformQueryAction<ItemType>(queryString, null);
        }

        protected ResultType PerformUniqueQueryAction<ResultType>(string queryString, QueryDecorator queryDecorator)
        {
            return PerformDataAction<ResultType>(session =>
            {
                IQuery query = session.CreateQuery(queryString);
                if (queryDecorator != null)
                    query = queryDecorator(query);
                return query.UniqueResult<ResultType>();
            });
        }

        protected ResultType PerformUniqueQueryAction<ResultType>(string queryString)
        {
            return PerformUniqueQueryAction<ResultType>(queryString, null);
        }

        protected IList<ItemType> PerformNamedQueryAction<ItemType>(string queryName, QueryDecorator queryDecorator)
        {
            return PerformDataAction<IList<ItemType>>(session =>
            {
                IQuery query = session.GetNamedQuery(queryName);
                if (queryDecorator != null)
                    query = queryDecorator(query);
                return query.List<ItemType>();
            });
        }

        // AcquireSharedSession and ReleaseSharedSession must always be called in pair.
        protected void AcquireSharedSession()
        {
            if (_sessionContext == null || _sessionContext.Session == null)
            {
                _sessionContext = new SessionContext();
                _sessionContextRefCount++;
            }
            else if (_sessionContextRefCount > 0) // internal shared session is in use.
            {
                _sessionContextRefCount++;
            }
            //else : external shared session in use, do nothing
        }

        protected void ReleaseSharedSession()
        {
            if (_sessionContextRefCount > 0)
            {
                _sessionContextRefCount--;
                if (_sessionContextRefCount == 0 && _sessionContext != null)
                {
                    _sessionContext.Dispose();
                    _sessionContext = null;
                }
            }
            //else : external shared session in use, do nothing
        }


        #endregion
    }
}
