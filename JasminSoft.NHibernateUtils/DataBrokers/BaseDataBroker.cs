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
    public abstract class BaseDataBroker : IDisposable
    {
        #region Fields

        protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(BaseDataBroker));
        protected const string NHIBERNATE_FAIL_LOGGING_MESSAGE = "Failed accessing data via NHibernate.";
        protected SessionContext _sessionContext;
        protected ISession _session;

        #endregion

        #region Properties

        public SessionContext SessionContext
        {
            get
            {
                return _sessionContext;
            }
            set
            {
                _sessionContext = value;
                _session = _sessionContext.Session;
            }
        }

        public ISession Session
        {
            get
            {
                if (_sessionContext != null)
                    return _sessionContext.Session;
                else
                    return null;
            }
        }

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
                _session = _sessionContext.Session;
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
                if (_session == null)
                {
                    // Use impromptu session.
                    using (SessionContext ctx = new SessionContext())
                    {
                        ISession session = ctx.Session;
                        action(session);
                    }
                }
                else
                {
                    action(_session);
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
                if (_session == null)
                {
                    // Use impromptu session.
                    using (SessionContext ctx = new SessionContext())
                    {
                        ISession session = ctx.Session;
                        result = action(session);
                    }
                }
                else
                {
                    result = action(_session);
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
            IList<ItemType> result = null;
            try
            {
                if (_session == null)
                {
                    // Use impromptu session.
                    using (SessionContext ctx = new SessionContext())
                    {
                        ISession session = ctx.Session;
                        IQuery query = session.CreateQuery(queryString);
                        if (queryDecorator != null)
                            query = queryDecorator(query);
                        result = query.List<ItemType>();
                    }
                }
                else
                {
                    IQuery query = _session.CreateQuery(queryString);
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
        }

        protected ResultType PerformUniqueQueryAction<ResultType>(string queryString)
        {
            return PerformUniqueQueryAction<ResultType>(queryString, null);
        }

        protected ResultType PerformUniqueQueryAction<ResultType>(string queryString, QueryDecorator queryDecorator)
        {
            ResultType result = default(ResultType);
            try
            {
                if (_session == null)
                {
                    // Use impromptu session.
                    using (SessionContext ctx = new SessionContext())
                    {
                        ISession session = ctx.Session;
                        IQuery query = session.CreateQuery(queryString);
                        if (queryDecorator != null)
                            query = queryDecorator(query);
                        result = query.UniqueResult<ResultType>();
                    }
                }
                else
                {
                    IQuery query = _session.CreateQuery(queryString);
                    if (queryDecorator != null)
                        query = queryDecorator(query);
                    result = query.UniqueResult<ResultType>();
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

        protected IList<ItemType> PerformQueryAction<ItemType>(string queryString)
        {
            return PerformQueryAction<ItemType>(queryString, null);
        }

        #endregion

        #region Session management methods

        public void FlushSession()
        {
            try
            {
                if (_session != null)
                {
                    _session.Flush();
                    ////////////////////
                    _log.Debug("Session flushed.");
                    ////////////////////
                }
            }
            catch (Exception ex)
            {
                ////////////////////
                _log.Error("Error occured while executing BaseDataBroker.FlushSession().", ex);
                ////////////////////
                throw ex;
            }
        }

        public void ClearSession()
        {
            try
            {
                if (_session != null)
                {
                    _session.Clear();
                    ////////////////////
                    _log.Debug("Session cleared.");
                    ////////////////////
                }
            }
            catch (Exception ex)
            {
                ////////////////////
                _log.Error("Error occured while executing BaseDataBroker.ClearSession().", ex);
                ////////////////////
                throw ex;
            }
        }

        public void CloseSession()
        {
            try
            {
                if (_session != null)
                {
                    _session.Close();
                    _session.Dispose();
                    _session = null;
                    ////////////////////
                    _log.Debug("Session closed.");
                    ////////////////////
                }
            }
            catch (Exception ex)
            {
                ////////////////////
                _log.Error("Error occured while executing BaseDataBroker.CloseSession().", ex);
                ////////////////////
                throw ex;
            }
        }

        #endregion

        #region IDispose

        public void Dispose()
        {
            CloseSession();
        }

        #endregion
    }
}
