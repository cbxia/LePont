using System;
using System.Collections.Generic;
using System.Linq;

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
            SimpleDataBroker db = new SimpleDataBroker();
            IList<PasswordQuestion> questions = db.GetAllValid<PasswordQuestion>();
            return questions != null ? questions.ToArray() : null;
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

        [ServiceMethod]
        public void DeactivateUser(int userID)
        {
            ToggleActivation<User>(userID, false);
        }

        [ServiceMethod]
        public void ActivateUser(int userID)
        {
            ToggleActivation<User>(userID, true);
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
            SimpleDataBroker db = new SimpleDataBroker();
            IList<RelationType> types = db.GetAllValid<RelationType>();
            return types != null ? types.ToArray() : null;
        }

        [ServiceMethod]
        public void AddCase(DisputeCase caseObj)
        {
            caseObj.Registrar = AppContext.CurrentUser;
            caseObj.Department = AppContext.CurrentUser.Department;
            caseObj.DateTime = DateTime.Now;
            caseObj.Deactivated = false;
            SimpleDataBroker db = new SimpleDataBroker();
            db.Save<DisputeCase>(caseObj);
        }

        [ServiceMethod]
        public DisputeCase GetCase(int id)
        {
            CaseBroker db = new CaseBroker();
            DisputeCase obj = db.GetById(id, AppContext.CurrentUser.Department);
            return obj;
        }

        [ServiceMethod]
        public DisputeCase[] BrowseCases(int pageSize, int pageIndex)
        {
            CaseBroker db = new CaseBroker();
            IList<DisputeCase> cases = db.Browse(AppContext.CurrentUser.Department, pageSize, pageIndex);
            return cases != null ? cases.ToArray() : null;
        }

        [ServiceMethod]
        public DataPage<DisputeCase> SearchCases(int depId, int caseTypeId, byte[] statuses, DateTime dateFrom, DateTime dateTo, int pageSize, int pageIndex)
        {
            SimpleDataBroker depBroker = new SimpleDataBroker();
            Department dep = depBroker.GetById<Department>(depId);
            CaseBroker db = new CaseBroker();
            return db.Search(dep, caseTypeId, statuses, dateFrom, dateTo, pageSize, pageIndex);
        }

        [ServiceMethod]
        public PublicationType[] GetPublicationTypes()
        {
            SimpleDataBroker db = new SimpleDataBroker();
            IList<PublicationType> types = db.GetAllValid<PublicationType>();
            return types != null ? types.ToArray() : null;
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
    }
}