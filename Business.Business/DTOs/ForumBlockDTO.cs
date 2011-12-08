using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.DTOs
{
    public class ForumBlockDTO
    {
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public long TotalTopics { get; set; }
        public long TotalPostings { get; set; }
        public string LastPublisher { get; set; }
        public DateTime? LastPublishTime { get; set; }
        public string Administrator { get; set; }

        public ForumBlockDTO(int blockID, string blockName, long totalTopics, long totalPostings, string lastPublisher, DateTime? lastPostTime, string administrator)
        {
            BlockID = blockID;
            BlockName = blockName;
            TotalTopics = totalTopics;
            TotalPostings = totalPostings;
            LastPublisher = lastPublisher;
            LastPublishTime = lastPostTime;
            Administrator = administrator;
        }
    }
}