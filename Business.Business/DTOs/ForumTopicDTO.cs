using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.DTOs
{
    public class ForumTopicDTO
    {
        public int ID { get; set; }
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int PublisherID { get; set; }
        public string PublisherName { get; set; }
        public DateTime? PublishTime { get; set; }
        public int? LastPublisherID { get; set; }
        public string LastPublisherName { get; set; }
        public DateTime? LastPublishTime { get; set; }

        public ForumTopicDTO(int ID, int BlockID, string BlockName, string Title,
            string Content, int PublisherID, string PublisherName, DateTime? PublishTime, int? LastPublisherID,
            string LastPublisherName, DateTime? LastPublishTime)
        {
            this.ID = ID;
            this.BlockID = BlockID;
            this.BlockName = BlockName;
            this.Title = Title;
            this.Content = Content;
            this.PublisherID = PublisherID;
            this.PublisherName = PublisherName;
            this.PublishTime = PublishTime;
            this.LastPublisherID = LastPublisherID;
            this.LastPublisherName = LastPublisherName;
            this.LastPublishTime = LastPublishTime;
        }
    }
}