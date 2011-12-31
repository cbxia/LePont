using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.Business
{
    public class Message : DeactivatableEntity
    {
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Content { get; set; }
        public virtual string AttachmentFileName { get; set; }
        public virtual byte[] AttachmentFileData { get; set; }
        public virtual DateTime? SendDateTime { get; set; }
        public virtual DateTime? ReadDateTime { get; set; }
    }
}