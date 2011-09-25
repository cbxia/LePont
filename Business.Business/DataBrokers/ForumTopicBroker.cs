using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class ForumTopicBroker : BaseGenericDataBroker<ForumTopic, int>
    {
        public DataPage<ForumTopic> GetTopics(int blockID, int pageSize, int pageIndex)
        {
            DataPage<ForumTopic> result = new DataPage<ForumTopic>();
            string queryString =
                @"select count(*)
                  from ForumTopic 
                  where Block.ID = :block_id and Deactivated = false";
            long count = PerformUniqueQueryAction<long>(queryString, query =>
            {
                query
                    .SetInt32("block_id", blockID);
                return query;
            });
            result.TotalRecords = count;

            queryString =
                @"from ForumTopic 
                  where Block.ID = :block_id and Deactivated = false 
                  order by LastPostTime desc ";
            IList<ForumTopic> resultSet = PerformQueryAction<ForumTopic>(queryString, query =>
            {
                query
                    .SetInt32("block_id", blockID);
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
    }
}
