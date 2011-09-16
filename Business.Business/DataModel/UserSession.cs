using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LePont.Business
{
    public class UserSession : Entity
    {
        public virtual User User { get; set; }
        public virtual string IP { get; set; }
        public virtual DateTime? LogonDateTime { get; set; }
        public virtual DateTime? LogoffDateTime { get; set; }
        public virtual string Browser { get; set; }
    }
}