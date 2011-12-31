using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.DTOs
{
    public class MessageDTO
    {
        public int ID { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime? SendDateTime { get; set; }
        public DateTime? ReadDateTime { get; set; }
        public string AttachmentFileName { get; set; }
        public bool IsRead { get { return ReadDateTime != null; } }
        public bool HasAttachment { get { return AttachmentFileName != null; } }

        public MessageDTO(int id, string senderName, string receiverName, string subject,
            DateTime? sendDateTime, DateTime? readDateTime, string attachmentFileName)
        {
            this.ID = id;
            this.SenderName = senderName;
            this.ReceiverName = receiverName;
            this.Subject = subject;
            this.SendDateTime = sendDateTime;
            this.ReadDateTime = readDateTime;
            this.AttachmentFileName = attachmentFileName;
        }

        public MessageDTO(int id, string senderName, string receiverName, string subject, string content,
            DateTime? sendDateTime, DateTime? readDateTime, string attachmentFileName) 
            : this(id,senderName,receiverName, subject, sendDateTime, readDateTime, attachmentFileName)
        {
            this.Content = content;
        }
    }
}