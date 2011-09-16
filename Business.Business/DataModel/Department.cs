using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.Business
{
    public class Department : DeactivatableEntity
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual Department Superior { get; set; }
        //[ScriptIgnore] //Circular ref. will cause json serialization error.
        //public virtual IList<Department> Subordinates { get; set; }
        public virtual short? Level { get; set; }
        public virtual short? ListOrder { get; set; }
    }
}