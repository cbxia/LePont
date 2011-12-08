using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.Business
{
    public class Message : DeactivatableEntity
    {
        public virtual int ID { get; set; }
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Content { get; set; }
        public virtual string AttachmentFileName { get; set; }
        [ScriptIgnore]
        public virtual byte[] AttachmentFileData { get; set; }
        public virtual DateTime? SendDateTime { get; set; }
        public virtual DateTime? ReceiveDateTime { get; set; }
        public virtual bool IsRead { get; set; }
    }
}