using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LePont.Business
{
    public class Role : DeactivatableEntity
    {
        public virtual string Name {get; set;}
        public virtual string Description {get; set;}
    }
}