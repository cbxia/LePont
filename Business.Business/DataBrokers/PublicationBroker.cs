using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class PublicationBroker : BaseGenericDataBroker<Publication, int>
    {
        // This method may not be used, because publications are generally visible to the public.
        public Publication GetById(int id, Department dep)
        {
            string queryString = "from Publication where ID = :id and (Department.ID = :dep_id or Department.Code like :code_pattern)";
            Publication result = PerformUniqueQueryAction<Publication>(queryString, query =>
            {
                query
                    .SetInt32("id", id)
                    .SetInt32("dep_id", dep.ID)
                    .SetString("code_pattern", string.Format("{0}%", dep.Code));
                return query;
            });
            return result;
        }
    }
}
