using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class CaseBroker : BaseGenericDataBroker<Dossier, int>
    {
        public Dossier GetById(int id, Department dep)
        {
            string queryString = "from Dossier where ID = :id and (Department.ID = :dep_id or Department.Code like :code_pattern) ";
            IList<Dossier> resultSet = PerformQueryAction<Dossier>(queryString, query =>
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

        public Dossier[] Browse(Department dep, int pageSize, int pageIndex)
        {
            string queryString = "from Dossier where (Department.ID = :dep_id or Department.Code like :code_pattern) and Deactivated = false order by DateTime desc ";
            IList<Dossier> resultSet = PerformQueryAction<Dossier>(queryString, query =>
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

        public DataPage<Dossier> Search(Department dep, int caseTypeId, byte[] statuses, DateTime dateFrom, DateTime dateTo, int pageSize, int pageIndex)
        {
            DataPage<Dossier> result = new DataPage<Dossier>();
            string queryString =
                @"select count(*)
                  from Dossier 
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
                @"from Dossier 
                  where (Department.ID = :dep_id or Department.Code like :code_pattern)  
                  and (InternalCaseType.ID = :case_type_id or :case_type_id = 0) 
                  and Status in (:statuses) 
                  and DateTime between :date_from and :date_to
                  and Deactivated = false order by DateTime desc ";
            IList<Dossier> resultSet = PerformQueryAction<Dossier>(queryString, query =>
            {
                query
                    .SetInt32("dep_id", dep.ID)
                    .SetString("code_pattern", string.Format("{0}%", dep.Code))
                    .SetInt32("case_type_id", caseTypeId)
                    .SetParameterList("statuses", statuses)
                    .SetDateTime("date_from", dateFrom)
                    .SetDateTime("date_to", dateTo);
                if (pageSize != 0) // else ignore paging
                {
                    query
                        .SetFirstResult((pageIndex - 1) * pageSize)
                        .SetMaxResults(pageSize);
                }
                return query;
            });
            if (resultSet != null && resultSet.Count > 0)
            {
                result.Data = resultSet.ToArray();
            }
            return result;
        }

        public DataPage<Dossier> Search(Department dep, int caseTypeId, byte[] statuses, DateTime dateFrom, DateTime dateTo)
        {
            return Search(dep, caseTypeId, statuses, dateFrom, dateTo, 0, 0);
        }

        public DataPage<Dossier> GetDeactivated(Department dep, int pageSize, int pageIndex)
        {
            DataPage<Dossier> result = new DataPage<Dossier>();
            string queryString =
                @"select count(*)
                  from Dossier 
                  where (Department.ID = :dep_id or Department.Code like :code_pattern)  
                  and Deactivated = true";
            long count = PerformUniqueQueryAction<long>(queryString, query =>
            {
                query
                    .SetInt32("dep_id", dep.ID)
                    .SetString("code_pattern", string.Format("{0}%", dep.Code));
                return query;
            });
            result.TotalRecords = count;

            queryString =
                @"from Dossier 
                  where (Department.ID = :dep_id or Department.Code like :code_pattern)  
                  and Deactivated = true order by DateTime desc ";
            IList<Dossier> resultSet = PerformQueryAction<Dossier>(queryString, query =>
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
                result.Data = resultSet.ToArray();
            }
            return result;
        }

    }
}
