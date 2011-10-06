using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;
using LePont.DTOs;

namespace LePont.Business
{
    public class ForumBroker : BaseDataBroker
    {
        public ForumBlockDTO[] GetForumBlockSummary()
        {
            string queryString = @"
                select new ForumBlockDTO(b.ID, b.Name, 
                    (
                      select count(ID) 
                      from ForumTopic
                      where Block.ID = b.ID
                    ),
                    (
                      select count(ID) 
                      from ForumPost
                      where Block.ID = b.ID
                    ),
                    u.Name, b.LastPublishTime, a.Name)    
                from ForumBlock as b 
                left outer join b.LastPublisher as u
                left outer join b.Administrator as a
                where b.Deactivated = false";
            IList<ForumBlockDTO> resultSet = PerformQueryAction<ForumBlockDTO>(queryString);
            if (resultSet != null && resultSet.Count > 0)
            {
                return resultSet.ToArray();
            }
            else
                return null;
        }

        public ForumResponseDTO[] GetFollowUps(int topicID)
        {
            string queryString = @"
                select new ForumResponseDTO (p.ID, t.ID, b.ID, b.Name, p.Content, u.ID, u.Name, p.PublishTime)  
                from ForumResponse as p
                inner join p.Topic as t
                inner join p.Block as b
                inner join p.Publisher as u
                where p.Topic.ID = :topic_id  
                order by p.PublishTime";
            IList<ForumResponseDTO> resultSet = PerformQueryAction<ForumResponseDTO>(queryString, query =>
            {
                query
                    .SetInt32("topic_id", topicID);
                return query;
            });
            if (resultSet != null && resultSet.Count > 0)
            {
                return resultSet.ToArray();
            }
            else
                return null;
        }

        public DataPage<ForumTopicDTO> GetTopics(int blockID, int pageSize, int pageIndex)
        {
            DataPage<ForumTopicDTO> result = new DataPage<ForumTopicDTO>();
            string queryString = @"
                select count(*)
                from ForumTopic 
                where Block.ID = :block_id";
            long count = PerformUniqueQueryAction<long>(queryString, query =>
            {
                query
                    .SetInt32("block_id", blockID);
                return query;
            });
            result.TotalRecords = count;

            queryString = @"
                select new ForumTopicDTO(p.ID, b.ID, b.Name, p.Title, p.Content, u.ID, u.Name, p.PublishTime, l.ID, l.Name, p.LastPublishTime)
                from ForumTopic as p
                inner join p.Block as b
                inner join p.Publisher as u
                left outer join p.LastPublisher as l
                where p.Block.ID = :block_id  
                order by p.LastPublishTime desc";
            IList<ForumTopicDTO> resultSet = PerformQueryAction<ForumTopicDTO>(queryString, query =>
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
