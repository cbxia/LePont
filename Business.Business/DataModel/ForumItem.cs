using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LePont.Business
{
    public class ForumItem : DeactivatableEntity
    {
        public virtual ForumItem Parent { get; set; }
        public virtual IList<ForumItem> Children { get; set; }
        public virtual string Topic { get; set; }
        public virtual string Content { get; set; }
        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
        public virtual DateTime? DateTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }
}