using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LePont.Business;

namespace LePont.Web
{
    public class ApplicationContext
    {
        public User CurrentUser { get; set; }
        public Role[] AllRoles { get; set; }
    }
}