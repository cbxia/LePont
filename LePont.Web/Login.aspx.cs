using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using JasminSoft.NHibernateUtils;
using DataModel = LePont.Business;
using LePont.Business;

namespace LePont.Web
{
    public partial class Login : System.Web.UI.Page
    {
        private bool AuthenticateUser(string loginId, string password)
        {
            bool result = false;
            using (SessionContext ctx = new SessionContext())
            {
                UserBroker broker_user = new UserBroker(ctx);
                DataModel.User user = broker_user.GetByLoginId(loginId);
                if (user != null && user.Password == password)
                {
                    // Session log
                    DataModel.UserSession session = new DataModel.UserSession();
                    session.Browser = Request.Browser.Browser;
                    session.IP = Request.UserHostAddress;
                    session.LogonDateTime = DateTime.Now;
                    session.User = user;
                    DataBroker broker_session = new DataBroker(ctx);
                    session = broker_session.Save<DataModel.UserSession>(session);
                    result = true;
                    Session["CurrentUser"] = user;
                    Session["CurrentSession"] = session;
                }
            }
            return result;
        }

        protected void Button_Login_Click(object sender, EventArgs e)
        {
            string userID = TextBox_UserID.Text;
            string password = TextBox_Password.Text;
            if (AuthenticateUser(userID, password))
            {
                FormsAuthentication.RedirectFromLoginPage(userID, false); // false means not create cross-session cookie.
            }
            else
            {
                string errorMessage = "登录失败，用户名或密码无效，请重试。";
                ClientScript.RegisterStartupScript(this.GetType(), "error", string.Format("<script type='text/javascript'>alert('{0}')</script>", errorMessage));
            }

        }
    }
}