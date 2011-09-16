using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class CaseTypeBroker : BaseGenericDataBroker<CaseType, int>
    {
        public CaseType[] GetByDomain(string domain)
        {
            string queryString = "from CaseType as instance where instance.Domain = :domain and Deactivated = false ";
            IList<CaseType> resultSet = PerformQueryAction<CaseType>(queryString, query =>
            {
                query = query.SetString("domain", domain);
                return query;
            });
            if (resultSet != null && resultSet.Count > 0)
            {
                return resultSet.ToArray();
            }
            else
                return null;
        }
    }
}
