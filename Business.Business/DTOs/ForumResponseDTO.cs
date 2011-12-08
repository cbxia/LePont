using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.DTOs
{
    public class ForumResponseDTO
    {
        public int ID { get; set; }
        public int TopicID { get; set; }
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public string Content { get; set; }
        public int PublisherID { get; set; }
        public string PublisherName { get; set; }
        public DateTime? PublishTime { get; set; }

        public ForumResponseDTO(int ID, int TopicID, int BlockID, string BlockName, 
            string Content, int PublisherID, string PublisherName, DateTime? PublishTime)
        {
            this.ID = ID;
            this.TopicID = TopicID;
            this.BlockID = BlockID;
            this.BlockName = BlockName;
            this.Content = Content;
            this.PublisherID = PublisherID;
            this.PublisherName = PublisherName;
            this.PublishTime = PublishTime;
        }
    }
}