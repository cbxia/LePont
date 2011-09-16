using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibernate.Cfg;

namespace JasminSoft.NHibernateUtils
{
    /// <summary>
    /// The SessionContext class serves two purposes: 
    /// [1] Creates and holds a static reference to a ready-for-use session factory.
    /// [2] Each instance of it is associated with one and only one ISession instance.
    /// </summary>
    public class SessionContext : IDisposable
    {
        private static object _lockObj = new object();
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(SessionContext));
        private static ISessionFactory _sessionFactory;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
        }

        public SessionContext()
        {
            _session = OpenSession();
        }

        public static ISessionFactory SessionFactory
        {
            get
            {
                lock (_lockObj)
                {
                    if (_sessionFactory == null)
                    {
                        try
                        {
                            Configuration config = new Configuration()
                                .SetNamingStrategy(new DbObjectsNamingStrategy())
                                .Configure();
                            _sessionFactory = config.BuildSessionFactory();
                            ////////////////////
                            _log.Debug("NHibernate session factory built.");
                            ////////////////////
                        }
                        catch (Exception ex)
                        {
                            ////////////////////
                            _log.Fatal("Failed building NHibernate session factory.", ex);
                            ////////////////////
                            throw ex;
                        }
                    }
                    return _sessionFactory;
                }
            }
        }

        public static ISession OpenSession()
        {
            ISession session = SessionFactory.OpenSession();
            ////////////////////
            _log.Debug("NHibernate session opened.");
            ////////////////////
            return session;
        }

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
                _log.Error("Error occured while executing NHibernateContext.FlushSession().", ex);
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
                _log.Error("Error occured while executing NHibernateContext.ClearSession().", ex);
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
                _log.Error("Error occured while executing NHibernateContext.CloseSession().", ex);
                ////////////////////
                throw ex;
            }
        }

        public void Dispose()
        {
            FlushSession();
            CloseSession();
        }
    }
}
