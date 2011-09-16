using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace LePont.Web
{
    public class UploadHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            foreach(string key in context.Request.Files)
            {
                HttpPostedFile file = context.Request.Files[key];
                byte[] buffer = new byte[file.ContentLength];
                file.InputStream.Read(buffer, 0, file.ContentLength);
                context.Session[key] = new { FileName = file.FileName, Data = buffer };
                // TODO: message is not complete.
                context.Response.Write("{msg: 'ok.'}");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}