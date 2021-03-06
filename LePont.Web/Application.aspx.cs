﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using LePont.Business;
using DataModel = LePont.Business;
using LePont.DTOs;
///////////Things below are bad guys, I don't have anything to do with them!!!
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;

// TODO: Server-side authorization model

namespace LePont.Web
{
    public partial class Application : ServicePage
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ServicePage));

        private ApplicationContext _appContext;
        public ApplicationContext AppContext
        {
            get
            {
                if (_appContext == null)
                {
                    _appContext = new ApplicationContext();
                    if (Session["CurrentUser"] != null)
                    {
                        _appContext.CurrentUser = (User)Session["CurrentUser"];
                    }
                    else
                    {
                        UserBroker userBroker = new UserBroker();
                        _appContext.CurrentUser = userBroker.GetByLoginId(Context.User.Identity.Name);
                    }
                    DataBroker roleBroker = new DataBroker();
                    _appContext.AllRoles = roleBroker.GetAll<Role>().ToArray();

                }
                return _appContext;
            }
        }

        [ServiceMethod]
        public ApplicationContext GetAppCtx()
        {
            return AppContext;
        }

        [ServiceMethod]
        public Department GetDepartment(int id)
        {
            DataBroker db = new DataBroker();
            Department obj = db.GetById<Department>(id);
            return obj;
        }

        [ServiceMethod]
        public Department[] GetSubDepartments(int parentId)
        {
            DepartmentBroker db = new DepartmentBroker();
            IList<Department> subs = db.GetSubDepartments(parentId);
            return subs != null ? subs.ToArray() : null;
        }

        [ServiceMethod]
        public void ModifyDepartment(int id, string code, string name, short listOrder)
        {
            using (SessionContext ctx = new SessionContext())
            {
                DataBroker db = new DataBroker(ctx);
                Department dep = db.GetById<Department>(id);
                dep.Code = code;
                dep.Name = name;
                dep.ListOrder = listOrder;
                db.Save<Department>(dep);
            }
        }

        [ServiceMethod]
        public void AddDepartment(int parentId, string code, string name, short listOrder)
        {
            using (SessionContext ctx = new SessionContext())
            {
                DataBroker db = new DataBroker(ctx);
                Department parent = db.GetById<Department>(parentId);
                Department dep = new Department { Superior = parent, Code = code, Name = name, Level = ++parent.Level, ListOrder = listOrder };
                db.Save<Department>(dep);
            }
        }

        [ServiceMethod]
        public User[] GetUsersByDepartment(string depId)
        {
            UserBroker db = new UserBroker();
            IList<User> users = db.GetByDepartment(depId);
            return users != null ? users.ToArray() : null;
        }

        [ServiceMethod]
        public PasswordQuestion[] GetPasswordQuestions()
        {
            return getAllValidItems<PasswordQuestion>();
        }

        [ServiceMethod]
        public void AddUser(User user)
        {
            DataBroker db = new DataBroker();
            user.Deactivated = false;
            user.CreateTime = DateTime.Now;
            db.Save<User>(user);
        }

        [ServiceMethod]
        public void ModifyUser(User user)
        {
            DataBroker db = new DataBroker();
            db.Save<User>(user);
        }

        [ServiceMethod]
        public void DeactivateUser(int id)
        {
            ToggleActivation<User>(id, false);
        }

        [ServiceMethod]
        public void ActivateUser(int id)
        {
            ToggleActivation<User>(id, true);
        }

        [ServiceMethod]
        public CaseType[] GetCaseTypes(string domain)
        {
            CaseTypeBroker db = new CaseTypeBroker();
            return db.GetByDomain(domain);
        }

        [ServiceMethod]
        public RelationType[] GetRelationTypes()
        {
            return getAllValidItems<RelationType>();
        }

        [ServiceMethod]
        public void AddCase(Dossier caseObj)
        {
            caseObj.Registrar = AppContext.CurrentUser;
            caseObj.Department = AppContext.CurrentUser.Department;
            caseObj.DateTime = DateTime.Now;
            caseObj.Deactivated = false;
            DataBroker db = new DataBroker();
            db.Save<Dossier>(caseObj);
        }

        [ServiceMethod]
        public void ModifyCase(Dossier caseObj)
        {
            DataBroker db = new DataBroker();
            db.Save<Dossier>(caseObj);
        }

        [ServiceMethod]
        public void DeactivateCase(int id)
        {
            ToggleActivation<Dossier>(id, false);
        }

        [ServiceMethod]
        public void ActivateCase(int id)
        {
            ToggleActivation<Dossier>(id, true);
        }

        [ServiceMethod]
        public Dossier GetCase(int id)
        {
            CaseBroker db = new CaseBroker();
            Dossier obj = db.GetById(id, AppContext.CurrentUser.Department);
            return obj;
        }

        [ServiceMethod]
        public Dossier[] BrowseCases(int pageSize, int pageIndex)
        {
            CaseBroker db = new CaseBroker();
            IList<Dossier> cases = db.Browse(AppContext.CurrentUser.Department, pageSize, pageIndex);
            return cases != null ? cases.ToArray() : null;
        }

        [ServiceMethod]
        public DataPage<Dossier> SearchCases(int depId, int caseTypeId, byte[] statuses, DateTime dateFrom, DateTime dateTo, int pageSize, int pageIndex)
        {
            DataBroker depBroker = new DataBroker();
            Department dep = depBroker.GetById<Department>(depId);
            CaseBroker db = new CaseBroker();
            return db.Search(dep, caseTypeId, statuses, dateFrom, dateTo, pageSize, pageIndex);
        }

        [ServiceMethod]
        public TextFileObject ExportCases(int depId, int caseTypeId, byte[] statuses, DateTime dateFrom, DateTime dateTo)
        {
            DataBroker depBroker = new DataBroker();
            Department dep = depBroker.GetById<Department>(depId);
            CaseBroker db = new CaseBroker();
            DataPage<Dossier> resultSet = db.Search(dep, caseTypeId, statuses, dateFrom, dateTo);
            StringBuilder fileData = new StringBuilder();
            fileData.Append("案件标题,原发单位,调解类别,综治委类别,纠纷简要情况,涉及金额,涉及人数,纠纷双方关系,责任人,责任人电话\r\n");
            foreach(Dossier dossier in resultSet.Data)
            {
                fileData.Append(generateCsvRow(
                    dossier.Title,
                    dossier.Locality,
                    dossier.InternalCaseType.Name,
                    dossier.ExternalCaseType.Name,
                    dossier.Content,
                    dossier.MoneyInvolved.ToString(),
                    dossier.PeopleInvolved.ToString(),
                    dossier.PartiesRelationType.Name,
                    dossier.Responsable,
                    dossier.ResponsablePhone
                    ));
            }
            string fileName = string.Format("案件汇总({0} — {1}).csv", dateFrom.ToShortDateString(), dateTo.AddDays(-1).ToShortDateString());
            if (Context.Request.UserAgent.ToUpper().Contains("MSIE"))
            {
                fileName = Server.UrlEncode(fileName).Replace("+", "%20");
            }
            TextFileObject file = new TextFileObject(fileName, true);
            file.Data = fileData.ToString();
            file.Encoding = System.Text.Encoding.GetEncoding("GB2312");
            file.Cacheability = System.Web.HttpCacheability.NoCache;
            return file;
        }

        [ServiceMethod]
        public DataPage<Dossier> GetDeactivatedCases(int pageSize, int pageIndex)
        {
            CaseBroker db = new CaseBroker();
            return db.GetDeactivated(AppContext.CurrentUser.Department, pageSize, pageIndex);
        }

        [ServiceMethod]
        public PublicationType[] GetPublicationTypes()
        {
            return getAllValidItems<PublicationType>();
        }

        [ServiceMethod]
        public void AddPublication(Publication publication)
        {
            publication.Publisher = AppContext.CurrentUser;
            publication.Department = AppContext.CurrentUser.Department;
            publication.DateTime = DateTime.Now;
            publication.Deactivated = false;
            string fileKey = "di-Attachment-Pub";
            dynamic file = Context.Session[fileKey];
            if (file != null)
            {
                publication.AttachmentFileName = file.FileName;
                publication.AttachmentFileData = file.Data;
                Context.Session.Remove(fileKey);
            }
            DataBroker db = new DataBroker();
            db.Save<Publication>(publication);
        }

        [ServiceMethod]
        public Publication GetPublication(int id)
        {
            PublicationBroker db = new PublicationBroker();
            // Note: Publications are not subject to security screening.
            // Publication pub = db.GetById(id, AppContext.CurrentUser.Department); 
            Publication obj = db.GetById(id);
            return obj;
        }

        [ServiceMethod]
        public BinaryFileObject GetPublicationAttachment(int pubId)
        {
            BinaryFileObject file = new BinaryFileObject();
            PublicationBroker db = new PublicationBroker();
            Publication pub = db.GetById(pubId, AppContext.CurrentUser.Department);
            file.FileName = pub.AttachmentFileName;
            file.Data = pub.AttachmentFileData;
            file.SendAsAttachment = true;
            file.Cacheability = System.Web.HttpCacheability.Server;
            return file;
        }

        [ServiceMethod]
        public PublicationLite[] BrowserPublications(int typeId, int pageSize, int pageIndex)
        {
            PublicationLiteBroker db = new PublicationLiteBroker();
            IList<PublicationLite> pubs = db.Browse(typeId, pageSize, pageIndex);
            return pubs != null ? pubs.ToArray() : null;
        }

        [ServiceMethod]
        public PublicationLite[] GetLatestPublications(int totalResults)
        {
            PublicationLiteBroker db = new PublicationLiteBroker();
            IList<PublicationLite> pubs = db.GetLatestPublications(totalResults);
            return pubs != null ? pubs.ToArray() : null;
        }

        [ServiceMethod]
        public UsageStatsItem[] GetMonthlyUsageStats()
        {
            DateTime DateFrom = DateTime.Today.AddDays(-(DateTime.Today.Day - 1));
            DateTime DateTo = DateFrom.AddMonths(1);
            UsageStatsBroker db = new UsageStatsBroker();
            IList<UsageStatsItem> items = db.GetUsageStat(DateFrom, DateTo);
            return items != null ? items.ToArray() : null;
        }

        [ServiceMethod]
        public FriendlyLink[] GetFriendlyLinks()
        {
            FriendlyLinksBroker db = new FriendlyLinksBroker();
            FriendlyLink[] items = db.GetFriendlyLinks();
            return items;
        }

        [ServiceMethod]
        public void AddInstruction(Instruction instruction)
        {
            instruction.Issuer = AppContext.CurrentUser;
            instruction.Department = AppContext.CurrentUser.Department;
            instruction.DateTime = DateTime.Now;
            instruction.Deactivated = false;
            string fileKey = "di-Attachment-Instr";
            dynamic file = Context.Session[fileKey];
            if (file != null)
            {
                instruction.AttachmentFileName = file.FileName;
                instruction.AttachmentFileData = file.Data;
                Context.Session.Remove(fileKey);
            }
            DataBroker db = new DataBroker();
            db.Save<Instruction>(instruction);
        }

        [ServiceMethod]
        public Instruction GetInstruction(int id)
        {
            InstructionBroker db = new InstructionBroker();
            Instruction obj = db.GetById(id, AppContext.CurrentUser.Department);
            return obj;
        }

        [ServiceMethod]
        public Instruction GetInstructionByCase(int caseId)
        {
            InstructionBroker db = new InstructionBroker();
            Instruction obj = db.GetByCase(caseId, AppContext.CurrentUser.Department);
            return obj;
        }

        [ServiceMethod]
        public BinaryFileObject GetInstructionAttachment(int pubId)
        {
            BinaryFileObject file = new BinaryFileObject();
            InstructionBroker db = new InstructionBroker();
            Instruction pub = db.GetById(pubId, AppContext.CurrentUser.Department);
            file.FileName = pub.AttachmentFileName;
            file.Data = pub.AttachmentFileData;
            file.SendAsAttachment = true;
            file.Cacheability = System.Web.HttpCacheability.Server;
            return file;
        }

        [ServiceMethod]
        public ForumBlock[] GetForumBlocks()
        {
            return getAllValidItems<ForumBlock>();
        }

        [ServiceMethod]
        public ForumBlockDTO[] GetForumBlockSummary()
        {
            ForumBroker db = new ForumBroker();
            return db.GetForumBlockSummary();
        }

        [ServiceMethod]
        public DataPage<ForumTopicDTO> GetForumTopics(int blockID, int pageSize, int pageIndex)
        {
            ForumBroker db = new ForumBroker();
            return db.GetTopics(blockID, pageSize, pageIndex);
        }

        [ServiceMethod]
        public ForumResponseDTO[] GetForumResponses(int topicID)
        {
            ForumBroker db = new ForumBroker();
            return db.GetFollowUps(topicID);
        }

        [ServiceMethod]
        public void AddForumTopic(ForumTopic post)
        {
            post.Publisher = AppContext.CurrentUser;
            post.PublishTime = DateTime.Now;
            post.LastPublisher = post.Publisher;
            post.LastPublishTime = post.PublishTime;
            post.ListOrder = 0;
            using (SessionContext ctx = new SessionContext())
            {
                DataBroker db = new DataBroker(ctx);
                /*The Block property is not fully populated at client-side, so we need to populate it from DB*/
                post.Block = db.GetById<ForumBlock>(post.Block.ID);
                post.Block.LastPublisher = post.Publisher;
                post.Block.LastPublishTime = post.PublishTime;
                db.Save<ForumTopic>(post);
            }
        }

        [ServiceMethod]
        public void DeleteForumTopic(int postId)
        {
            DataBroker db = new DataBroker();
            db.Delete<ForumTopic>(postId);
        }

        [ServiceMethod]
        public void DeleteForumResponse(int postId)
        {
            DataBroker db = new DataBroker();
            db.Delete<ForumResponse>(postId);
        }

        [ServiceMethod]
        public void AddForumResponse(ForumResponse post)
        {
            post.Publisher = AppContext.CurrentUser;
            post.PublishTime = DateTime.Now;
            post.ListOrder = 0;
            using (SessionContext ctx = new SessionContext())
            {
                DataBroker db = new DataBroker(ctx);
                post.Topic = db.GetById<ForumTopic>(post.Topic.ID);
                post.Topic.LastPublisher = post.Publisher;
                post.Topic.LastPublishTime = post.PublishTime;
                post.Block = db.GetById<ForumBlock>(post.Block.ID);
                post.Block.LastPublisher = post.Publisher;
                post.Block.LastPublishTime = post.PublishTime;
                db.Save<ForumResponse>(post);
            }
        }

        [ServiceMethod]
        public LogonStatDTO[] GetLogonStat(int depId, DateTime dateFrom, DateTime dateTo)
        {
            DataBroker depBroker = new DataBroker();
            Department dep = depBroker.GetById<Department>(depId);

            ReportBroker db = new ReportBroker();
            return db.GetLogonStat(dep, dateFrom, dateTo);
        }

        [ServiceMethod]
        public MessageDTO GetMessage(int id, bool markAsRead)
        {
            MessageBroker db = new MessageBroker();
            return db.GetMessage(id, markAsRead);
        }

        [ServiceMethod] 
        public void SendMessage(Message message, string recipient)
        {
            UserBroker ub = new UserBroker();
            User receiver = ub.GetByEmailAddress(recipient);
            if (receiver == null)
            {
                receiver = ub.GetByLoginId(recipient);
            }
            if (receiver == null)
            {
                string error = string.Format("邮件发送失败，原因是没有找到收件人\"{0}\"", recipient);
                if (_log != null)
                    _log.Error(error);
                throw new InvalidOperationException(error); // See how this message could be caught by client side code.
            }
            else
            {
                message.Receiver = receiver;
                message.Sender = AppContext.CurrentUser;
                message.SendDateTime = DateTime.Now;
                message.Deactivated = false;
                string fileKey = "di-Attachment-Mail";
                dynamic file = Context.Session[fileKey];
                if (file != null)
                {
                    message.AttachmentFileName = file.FileName;
                    message.AttachmentFileData = file.Data;
                    Context.Session.Remove(fileKey);
                }
                MessageBroker mb = new MessageBroker();
                mb.Save(message);
            }
        }

        [ServiceMethod]
        public BinaryFileObject GetMessageAttachment(int MessageID)
        {
            BinaryFileObject file = new BinaryFileObject();
            MessageBroker db = new MessageBroker();
            Message message = db.GetById(MessageID);
            file.FileName = message.AttachmentFileName;
            file.Data = message.AttachmentFileData;
            file.SendAsAttachment = true;
            file.Cacheability = System.Web.HttpCacheability.Server;
            return file;
        }

        [ServiceMethod]
        public DataPage<MessageDTO> GetInbox(int pageSize, int pageIndex)
        {
            MessageBroker db = new MessageBroker();
            return db.GetInbox(AppContext.CurrentUser, pageSize, pageIndex);
        }

        [ServiceMethod]
        public DataPage<MessageDTO> GetOutbox(int pageSize, int pageIndex)
        {
            MessageBroker db = new MessageBroker();
            return db.GetOutbox(AppContext.CurrentUser, pageSize, pageIndex);
        }

        [ServiceMethod]
        public DataPage<MessageDTO> GetTrashcan(int pageSize, int pageIndex)
        {
            MessageBroker db = new MessageBroker();
            return db.GetTrashcan(AppContext.CurrentUser, pageSize, pageIndex);
        }


        [ServiceMethod]
        public ReportItem[] GenerateReport_1(DateTime dateFrom, DateTime dateTo)
        {
            ReportBroker db = new ReportBroker();
            return db.GenerateReport_1(dateFrom, dateTo);
        }

        #region Helpers

        private T[] getAllValidItems<T>()
            where T : class
        {
            DataBroker db = new DataBroker();
            IList<T> types = db.GetAllValid<T>();
            return types != null ? types.ToArray() : null;
        }


        private void ToggleActivation<T>(int id, bool activated)
            where T : DeactivatableEntity
        {
            using (SessionContext ctx = new SessionContext())
            {
                DataBroker db = new DataBroker(ctx);
                T entity = db.GetById<T>(id);
                entity.Deactivated = !activated;
                db.Save(entity);
            }
        }

        private string escapeCsvField(string raw)
        {
            // Ref for CSV format: http://www.csvreader.com/csv_format.php
            return "\"" + raw.Replace("\"", "\"\"") + "\"";
        }

        private string generateCsvRow(params string[] fields)
        {
            string result = null;
            for (int i = 0; i < fields.Length; i++)
            {
                string field = fields[i];
                if (i < fields.Length - 1)
                    result += escapeCsvField(field) + ",";
                else
                    result += escapeCsvField(field) + "\r\n";
            }
            return result;
        }

        #endregion
    }
}