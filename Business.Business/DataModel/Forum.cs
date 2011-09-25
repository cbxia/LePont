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
        public virtual DateTime? LastPostTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }

    public class ForumTopic : DeactivatableEntity
    {
        [ScriptIgnore]
        public virtual ForumBlock Block { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual User Creator { get; set; }
        public virtual DateTime? CreateTime { get; set; }
        public virtual DateTime? LastPostTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }

    public class ForumPost : Entity
    {
        [ScriptIgnore]
        public virtual ForumBlock Block { get; set; }
        public virtual ForumTopic Topic { get; set; }
        public virtual string Content { get; set; }
        public virtual User Publisher  { get; set; }
        public virtual DateTime? PublishTime { get; set; }
        public virtual short? ListOrder { get; set; }
    }

    public class ForumBlockSummaryDTO
    {
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public long TotalTopics { get; set; }
        public long TotalPostings { get; set; }
        public string LastPublisher { get; set; }
        public DateTime? LastPostTime { get; set; }
        public string Administrator { get; set; }

        public ForumBlockSummaryDTO(int blockID, string blockName, long totalTopics, long totalPostings, string lastPublisher, DateTime? lastPostTime, string administrator)
        {
            BlockID = blockID;
            BlockName = blockName;
            TotalTopics = totalTopics;
            TotalPostings = totalPostings;
            LastPublisher = lastPublisher;
            LastPostTime = lastPostTime;
            Administrator = administrator;
        }
    }
}