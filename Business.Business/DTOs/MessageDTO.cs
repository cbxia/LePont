using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.DTOs
{
    public class MessageDTO
    {
        public int ID { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Subject { get; set; }
        public DateTime? SendDateTime { get; set; }
        public DateTime? ReceiveDateTime { get; set; }
        public bool IsRead { get; set; }
        public bool HasAttachment { get; set; }
    }
}