using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class UserBroker : BaseGenericDataBroker<User, int>
    {
        public UserBroker()
        {
        }

        public UserBroker(SessionContext ctx)
            : base(ctx)
        {
        }

        public User GetByLoginId(string loginId)
        {
            string queryString = "from User as instance where instance.LoginId = :LoginId and Deactivated = false ";
            IList<User> resultSet = PerformQueryAction<User>(queryString, query =>
            {
                query = query.SetString("LoginId", loginId);
                return query;
            });
            if (resultSet != null && resultSet.Count > 0)
            {
                return resultSet[0];
            }
            else
                return null;
        }

        public IList<User> GetByDepartment(string depId)
        {
            string queryString = "from User as instance where instance.Department.ID = :depId";
            IList<User> resultSet = PerformQueryAction<User>(queryString, query =>
            {
                query = query.SetString("depId", depId);
                return query;
            });
            return resultSet;
        }
    }
}
