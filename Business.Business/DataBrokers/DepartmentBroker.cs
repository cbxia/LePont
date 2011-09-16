using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class DepartmentBroker : BaseGenericDataBroker<Department, int>
    {
        public IList<Department> GetSubDepartments(int superiorId)
        {
            string queryString = "from Department as instance where instance.Superior.ID = :superiorId";
            IList<Department> resultSet = PerformQueryAction<Department>(queryString, query =>
            {
                query = query.SetInt32("superiorId", superiorId);
                return query;
            });
            return resultSet;
        }
    }
}
