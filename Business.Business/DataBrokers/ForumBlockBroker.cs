using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class ForumBlockBroker : BaseGenericDataBroker<ForumBlock, int>
    {
        public ForumBlockSummaryDTO[] GetForumBlockSummary()
        {
            string queryString = @"
                select new ForumBlockSummaryDTO(b.ID, b.Name, 
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
                    u.Name, b.LastPostTime, a.Name)    
                from ForumBlock as b 
                left outer join b.LastPublisher as u
                left outer join b.Administrator as a
                where b.Deactivated = false";
            IList<ForumBlockSummaryDTO> resultSet = PerformQueryAction<ForumBlockSummaryDTO>(queryString);
            if (resultSet != null && resultSet.Count > 0)
            {
                return resultSet.ToArray();
            }
            else
                return null;
        }
    }
}
