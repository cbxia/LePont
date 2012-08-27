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
    // [Note] If class Publication inherits from PublicationLite, with the same mapping document, SetMaxResults will not work properly (returning 0 results).
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
    /* Important architectural notes!!
     * When instances of PublicationLite are serialized to client side, they're already not 
     * really "lite"! entire graphs of User, Department, PublicationType objects are included,
     * rendering the data package cumbersome with useless undesired data.
     * This shows how wrong it is to naïvely expose the business model to the client side.
     */
}