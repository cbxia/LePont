using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class InstructionLiteBroker : BaseGenericDataBroker<InstructionLite, int>
    {
        public InstructionLite[] Browse(int pageSize, int pageIndex, Department dep)
        {
            string queryString = "from InstructionLite where (TargetCase.Department.ID = :dep_id or TargetCase.Department.Code like :code_pattern) and Deactivated = false order by DateTime desc ";
            IList<InstructionLite> resultSet = PerformQueryAction<InstructionLite>(queryString, query =>
            {
                query
                    .SetInt32("dep_id", dep.ID)
                    .SetString("code_pattern", string.Format("{0}%", dep.Code))
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
