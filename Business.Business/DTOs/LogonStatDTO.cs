using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.DTOs
{
    public class LogonStatDTO
    {
        public string LoginId { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public DateTime? LogonDateTime { get; set; }
        public long LoginCount { get; set; }
        public long CaseCount { get; set; }

        public LogonStatDTO(string LoginId, string Name, string IP, DateTime? LogonDateTime, long LoginCount, long CaseCount)
        {
            this.LoginId = LoginId;
            this.Name = Name;
            this.IP = IP;
            this.LogonDateTime = LogonDateTime;
            this.LoginCount = LoginCount;
            this.CaseCount = CaseCount;
        }
    }
}