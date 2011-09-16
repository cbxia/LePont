using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.Business
{
    public class PublicationType : DeactivatableEntity
    {
        public virtual string Name { get; set; }
    }

    public class Publication : DeactivatableEntity
    {
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual string AttachmentFileName { get; set; }
        [ScriptIgnore] // Attachment content is fetched by dedicated file download request.
        public virtual byte[] AttachmentFileData { get; set; }
        public virtual PublicationType Type { get; set; }
        public virtual Department Department { get; set; }
        public virtual User Publisher { get; set; }
        public virtual DateTime? DateTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }

    // For list-loading only
    public class PublicationLite : DeactivatableEntity
    {
        public virtual string Title { get; set; }
        public virtual string AttachmentFileName { get; set; }
        public virtual PublicationType Type { get; set; }
        public virtual Department Department { get; set; }
        public virtual User Publisher { get; set; }
        public virtual DateTime? DateTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }

    // [Note] If class Publication inherits from PublicationLite, with the same mapping document, SetMaxResults will not work properly (returning 0 results).
}