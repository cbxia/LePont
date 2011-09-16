using System;
using System.Text;

using NHibernate.Cfg;
using NHibernate.Util;

namespace JasminSoft.NHibernateUtils
{

    public sealed class DbObjectsNamingStrategy : INamingStrategy
    {
        public static readonly INamingStrategy DefaultInstance = new DbObjectsNamingStrategy();

        public string ClassToTableName(string className)
        {
            return AddUnderscoresAndCapitalize(StringHelper.Unqualify(className));
        }

        public string PropertyToColumnName(string propertyName)
        {
            return AddUnderscoresAndCapitalize(StringHelper.Unqualify(propertyName));
        }

        public string PropertyToTableName(string className, string propertyName)
        {
            return
                AddUnderscoresAndCapitalize(StringHelper.Unqualify(className))
                + "_X_"
                + AddUnderscoresAndCapitalize(StringHelper.Unqualify(propertyName));
        }

        public string TableName(string tableName)
        {
            //return AddUnderscoresAndCapitalize(StringHelper.Unqualify(tableName));
            return tableName;
        }

        public string ColumnName(string columnName)
        {
            // The value of the 'column' attribute below is subject to processing by this method.
            // <many-to-one name="User" column="UserId" /> 
            // return AddUnderscoresAndCapitalize(StringHelper.Unqualify(columnName));
            return columnName;
        }

        public string LogicalColumnName(string columnName, string propertyName)
        {
            return StringHelper.IsNotEmpty(columnName) ? columnName : StringHelper.Unqualify(propertyName);
        }

        /// <summary>
        /// Can convert given strings such as "UsersOnline" into "USERS_ONLINE".
        /// It handles special cases satisfactorily, e.g., US -> US; USA - > USA; theUS -> THE_US; U -> U; USAAndCanada -> USA_AND_CANADA; IsVIPOrAdmin -> IS_VIP_OR_ADMIN
        /// </summary>
        public string AddUnderscoresAndCapitalize(string name)
        {
            StringBuilder buf = new StringBuilder(name.Replace('.', '_'));

            for (int i = 1; i < buf.Length - 1; i++)
            {
                if (buf[i - 1] != '_' && Char.IsUpper(buf[i]) && (Char.IsLower(buf[i-1]) || Char.IsLower(buf[i + 1])))
                {
                    buf.Insert(i++, '_');
                }
            }
            return buf.ToString().ToUpper();
        }
    }
}



