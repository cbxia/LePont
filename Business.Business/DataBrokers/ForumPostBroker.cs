using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class ForumPostBroker : BaseGenericDataBroker<ForumPost, int>
    {
        public ForumPost[] GetFollowUPs(int topicID)
        {
            string queryString = @"
                  from ForumPost 
                  where Topic.ID = :topic_id  
                  order by PublishTime desc";
            IList<ForumPost> resultSet = PerformQueryAction<ForumPost>(queryString, query =>
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

    }
}
