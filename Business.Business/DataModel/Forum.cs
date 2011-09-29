using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.Business
{
    public class ForumBlock : DeactivatableEntity
    {
        public virtual string Name { get; set; }
        public virtual User Administrator { get; set; }
        public virtual User LastPublisher { get; set; }
        public virtual DateTime? LastPublishTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }

    public abstract class ForumPost : Entity
    {
        [ScriptIgnore]
        public virtual ForumBlock Block { get; set; }
        public virtual string Content { get; set; }
        public virtual User Publisher { get; set; }
        public virtual DateTime? PublishTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }

    public class ForumTopic : ForumPost
    {
        public virtual string Title { get; set; }
        public virtual User LastPublisher { get; set; }
        public virtual DateTime? LastPublishTime { get; set; }
    }

    public class ForumFollowUp : ForumPost
    {
        public virtual ForumTopic Topic { get; set; }
    }
}