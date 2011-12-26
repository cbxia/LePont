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
        /*
         * Lessons learned:
         * [1] Never, ever leak your domain model to the client layer in a true distributed application! Always stick to
         * strict loose coupling.
         * [2] There should be a distinct service layer, with distinct service data models. Even there could be significant
         * duplication in properties in domain models and service models, it is NO good architectual practice trying to reuse 
         * properties across layers. The above means the DTOs and DataPages here should better be moved to a separate service layer.
         */

        public MessageDTO GetMessage(int messageID, bool markAsRead)
        {
            MessageDTO result = null;
            try
            {
                EnsureSharedSession();
                Message message = GetById(messageID);
                if (markAsRead)
                {
                    message.ReadDateTime = DateTime.Now;
                    SaveAttached(message);
                }
                result = new MessageDTO(
                    message.ID,
                    message.Sender.Name,
                    message.Receiver.Name,
                    message.Subject,
                    message.SendDateTime,
                    message.ReadDateTime,
                    message.AttachmentFileName);
            }
            finally
            {
                EndEnsureSharedSession();
            }
            return result;
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
                @"select new MessageDTO(m.ID, s.Name, r.Name, m.Subject, m.SendDateTime, m.ReadDateTime, m.AttachmentFileName)  
                  from Message as m        
                  left outer join m.Sender as s
                  left outer join m.Receiver as r
                  where (m.Receiver.ID = :user_id) 
                  and m.Deactivated = false order by m.SendDateTime desc ";
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

        public DataPage<MessageDTO> GetOutbox(User user, int pageSize, int pageIndex)
        {
            DataPage<MessageDTO> result = new DataPage<MessageDTO>();
            string queryString =
                @"select count(*)
                  from Message 
                  where (Sender.ID = :user_id) 
                  and Deactivated = false";
            long count = PerformUniqueQueryAction<long>(queryString, query =>
            {
                query
                    .SetInt32("user_id", user.ID);
                return query;
            });
            result.TotalRecords = count;

            queryString =
                @"select new MessageDTO(m.ID, s.Name, r.Name, m.Subject, m.SendDateTime, m.ReadDateTime, m.AttachmentFileName)  
                  from Message as m        
                  left outer join m.Sender as s
                  left outer join m.Receiver as r
                  where (m.Sender.ID = :user_id) 
                  and m.Deactivated = false order by m.SendDateTime desc ";
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

        public DataPage<MessageDTO> GetTrashcan(User user, int pageSize, int pageIndex)
        {
            return null;
        }
    }
}
