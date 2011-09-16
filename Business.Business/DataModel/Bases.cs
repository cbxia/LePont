using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.Business
{
    public class Entity
    {
        public virtual int ID { get; set; }
    }

    public class DeactivatableEntity : Entity
    {
        public virtual bool? Deactivated { get; set; }
    }
}