using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class PublicationLiteBroker : BaseGenericDataBroker<PublicationLite, int>
    {
        public PublicationLite[] Browse(int typeId, int pageSize, int pageIndex)
        {
            string queryString = "from PublicationLite where Type.ID = :typeid and Deactivated = false order by DateTime desc ";
            IList<PublicationLite> resultSet = PerformQueryAction<PublicationLite>(queryString, query =>
            {
                query
                    .SetInt32("typeid", typeId)
                    .SetFirstResult((pageIndex - 1) * pageSize)
                    .SetMaxResults(pageSize);
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
