using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.Business
{
    public class User : DeactivatableEntity
    {
        public virtual string LoginId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        public virtual PasswordQuestion PasswordQuestion { get; set; }
        public virtual string PasswordAnswer { get; set; }
        public virtual Department Department { get; set; }
        public virtual IList<Role> Roles { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime? LastLogonTime { get; set; }
        public virtual DateTime? CreateTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }

    public class PasswordQuestion : DeactivatableEntity
    {
        public virtual string Content { get; set; }
    }


}