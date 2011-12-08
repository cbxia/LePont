using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;
using LePont.DTOs;

namespace LePont.Business
{
    public class ReportBroker : BaseDataBroker
    {
        public LogonStatDTO[] GetLogonStat(Department dep, DateTime dateFrom, DateTime dateTo)
        {
            // How NHibernate use stored procedures: 
            // http://nhforge.org/blogs/nhibernate/archive/2008/11/23/populating-entities-from-stored-procedures-with-nhibernate.aspx
            string queryString = @"
                select new LogonStatDTO(
                    u.LoginId, 
                    u.Name, 
                    s.IP, 
                    u.LastLogonTime, 
                    count(s),
                    count(distinct d.ID)
                )
                from UserSession as s
                left outer join s.User as u,
                Dossier as d
                where d.Registrar.ID = u.ID and 
                      (u.Department.ID = :dep_id or u.Department.Code like :code_pattern) and 
                      s.LogonDateTime between :date_from and :date_to 
                group by u.LoginId, u.Name, s.IP, u.LastLogonTime";
            IList<LogonStatDTO> resultSet = PerformQueryAction<LogonStatDTO>(queryString, query =>
            {
                query
                    .SetInt32("dep_id", dep.ID)
                    .SetString("code_pattern", string.Format("{0}%", dep.Code))
                    .SetDateTime("date_from", dateFrom)
                    .SetDateTime("date_to", dateTo);
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
