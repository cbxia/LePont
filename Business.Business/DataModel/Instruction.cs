using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.Business
{
    public class Instruction : InstructionLite
    {
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual DisputeCase TargetCase { get; set; }
        public virtual string AttachmentFileName { get; set; }
        [ScriptIgnore] // Attachment content is fetched by dedicated file download request.
        public virtual byte[] AttachmentFileData { get; set; }
        public virtual Department Department { get; set; }
        public virtual User Issuer { get; set; }
        public virtual DateTime? DateTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }

    // For list-loading only
    public class InstructionLite : DeactivatableEntity
    {
        public virtual string Title { get; set; }
        public virtual DisputeCase TargetCase { get; set; }
        public virtual string AttachmentFileName { get; set; }
        public virtual Department Department { get; set; }
        public virtual User Issuer { get; set; }
        public virtual DateTime? DateTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }
}