using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using LePont.Business;
using DataModel = LePont.Business;

///////////Things below are bad guys, I don't have anything to do with them!!!
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;

// TODO: Server-side authorization model

namespace LePont.Web
{
    public partial class Application : ServicePage
    {
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
                    SimpleDataBroker roleBroker = new SimpleDataBroker();
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
            SimpleDataBroker db = new SimpleDataBroker();
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
                SimpleDataBroker db = new SimpleDataBroker(ctx);
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
                SimpleDataBroker db = new SimpleDataBroker(ctx);
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
            SimpleDataBroker db = new SimpleDataBroker();
            user.Deactivated = false;
            user.CreateTime = DateTime.Now;
            db.Save<User>(user);
        }

        [ServiceMethod]
        public void ModifyUser(User user)
        {
            SimpleDataBroker db = new SimpleDataBroker();
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
            SimpleDataBroker db = new SimpleDataBroker();
            db.Save<Dossier>(caseObj);
        }

        [ServiceMethod]
        public void ModifyCase(Dossier caseObj)
        {
            SimpleDataBroker db = new SimpleDataBroker();
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
            SimpleDataBroker depBroker = new SimpleDataBroker();
            Department dep = depBroker.GetById<Department>(depId);
            CaseBroker db = new CaseBroker();
            return db.Search(dep, caseTypeId, statuses, dateFrom, dateTo, pageSize, pageIndex);
        }

        [ServiceMethod]
        public TextFileObject ExportCases(int depId, int caseTypeId, byte[] statuses, DateTime dateFrom, DateTime dateTo)
        {
            SimpleDataBroker depBroker = new SimpleDataBroker();
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
            SimpleDataBroker db = new SimpleDataBroker();
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
            SimpleDataBroker db = new SimpleDataBroker();
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
        public ForumBlockSummaryDTO[] GetForumBlockSummary()
        {
            ForumBlockBroker db = new ForumBlockBroker();
            return db.GetForumBlockSummary();
        }

        [ServiceMethod]
        public DataPage<ForumTopic> GetForumTopics(int blockID, int pageSize, int pageIndex)
        {
            ForumTopicBroker db = new ForumTopicBroker();
            return db.GetTopics(blockID, pageSize, pageIndex);
        }

        [ServiceMethod]
        public ForumPost[] GetFollowUPs(int topicID)
        {
            ForumPostBroker db = new ForumPostBroker();
            return db.GetFollowUPs(topicID);
        }

        [ServiceMethod]
        public void AddForumTopic(ForumTopic topic)
        {
            topic.Publisher = AppContext.CurrentUser;
            topic.PublishTime = DateTime.Now;
            topic.LastPostTime = topic.PublishTime;
            topic.Deactivated = false;
            topic.ListOrder = 0;
            using (SessionContext ctx = new SessionContext())
            {
                SimpleDataBroker db = new SimpleDataBroker(ctx);
                topic.Block = db.GetById<ForumBlock>(topic.Block.ID);
                topic.Block.LastPublisher = topic.Publisher;
                topic.Block.LastPostTime = topic.PublishTime;
                db.Save<ForumTopic>(topic);
            }
        }

        #region Helpers

        private T[] getAllValidItems<T>()
            where T : class
        {
            SimpleDataBroker db = new SimpleDataBroker();
            IList<T> types = db.GetAllValid<T>();
            return types != null ? types.ToArray() : null;
        }


        private void ToggleActivation<T>(int id, bool activated)
            where T : DeactivatableEntity
        {
            using (SessionContext ctx = new SessionContext())
            {
                SimpleDataBroker db = new SimpleDataBroker(ctx);
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