using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;
using LePont.DTOs;

namespace LePont.Business
{
    public class MessageBroker : BaseGenericDataBroker<Message, int>
    {
        public Message ReadMessage(int messageID)
        {
            return PerformDataAction<Message>(session =>
            {
                Message result = session.Get<Message>(messageID);
                result.ReadDateTime = DateTime.Now;
                session.Update(result);
                return result;
            });
        }

        public DataPage<MessageDTO> GetInbox(User user, int pageSize, int pageIndex)
        {
            DataPage<MessageDTO> result = new DataPage<MessageDTO>();
            string queryString =
                @"select count(*)
                  from Message 
                  where (Receiver.ID = :user_id) 
                  and Deactivated = false";
            long count = PerformUniqueQueryAction<long>(queryString, query =>
            {
                query
                    .SetInt32("user_id", user.ID);
                return query;
            });
            result.TotalRecords = count;

            queryString =
                @"from Message 
                  where (Receiver.ID = :user_id) 
                  and Deactivated = false order by DateTime desc ";
            IList<MessageDTO> resultSet = PerformQueryAction<MessageDTO>(queryString, query =>
            {
                query
                    .SetInt32("user_id", user.ID)
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
