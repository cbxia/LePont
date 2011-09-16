using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class CaseBroker : BaseGenericDataBroker<DisputeCase, int>
    {
        public DisputeCase GetById(int id, Department dep)
        {
            string queryString = "from DisputeCase where ID = :id and (Department.ID = :dep_id or Department.Code like :code_pattern) ";
            IList<DisputeCase> resultSet = PerformQueryAction<DisputeCase>(queryString, query =>
            {
                query
                    .SetInt32("id", id)
                    .SetInt32("dep_id", dep.ID)
                    .SetString("code_pattern", string.Format("{0}%", dep.Code));
                return query;
            });
            if (resultSet != null && resultSet.Count > 0)
            {
                return resultSet[0];
            }
            else
                return null;

        }

        public DisputeCase[] Browse(Department dep, int pageSize, int pageIndex)
        {
            string queryString = "from DisputeCase where (Department.ID = :dep_id or Department.Code like :code_pattern) and Deactivated = false order by DateTime desc ";
            IList<DisputeCase> resultSet = PerformQueryAction<DisputeCase>(queryString, query =>
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

        public DataPage<DisputeCase> Search(Department dep, int caseTypeId, byte[] statuses, DateTime dateFrom, DateTime dateTo, int pageSize, int pageIndex)
        {
            DataPage<DisputeCase> result = new DataPage<DisputeCase>();
            string queryString =
                @"select count(*)
                  from DisputeCase 
                  where (Department.ID = :dep_id or Department.Code like :code_pattern)  
                  and (InternalCaseType.ID = :case_type_id or :case_type_id = 0)
                  and Status in (:statuses) 
                  and DateTime between :date_from and :date_to
                  and Deactivated = false";
            long count = PerformUniqueQueryAction<long>(queryString, query =>
            {
                query
                    .SetInt32("dep_id", dep.ID)
                    .SetString("code_pattern", string.Format("{0}%", dep.Code))
                    .SetInt32("case_type_id", caseTypeId)
                    .SetParameterList("statuses", statuses)
                    .SetDateTime("date_from", dateFrom)
                    .SetDateTime("date_to", dateTo);
                return query;
            });
            result.TotalRecords = count;

            queryString =
                @"from DisputeCase 
                  where (Department.ID = :dep_id or Department.Code like :code_pattern)  
                  and (InternalCaseType.ID = :case_type_id or :case_type_id = 0) 
                  and Status in (:statuses) 
                  and DateTime between :date_from and :date_to
                  and Deactivated = false order by DateTime desc ";
            IList<DisputeCase> resultSet = PerformQueryAction<DisputeCase>(queryString, query =>
            {
                query
                    .SetInt32("dep_id", dep.ID)
                    .SetString("code_pattern", string.Format("{0}%", dep.Code))
                    .SetInt32("case_type_id", caseTypeId)
                    .SetParameterList("statuses", statuses)
                    .SetDateTime("date_from", dateFrom)
                    .SetDateTime("date_to", dateTo)
                    .SetFirstResult((pageIndex - 1) * pageSize)
                    .SetMaxResults(pageSize);
                return query;
            });
            if (resultSet != null && resultSet.Count > 0)
            {
                result.Data = resultSet.ToArray();
            }
            return result;
        }

    }
}
