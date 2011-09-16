using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.Web
{
    public abstract class ServicePage : System.Web.UI.Page
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ServicePage));
        public override void ProcessRequest(HttpContext context)
        {
            if (context.Request["invokemode"] == "service") 
            {
                DispatchInvoke(context);
            }
            else 
            {
                base.ProcessRequest(context);
            }
        }

        protected virtual void DispatchInvoke(HttpContext context)
        {
            string methodName = context.Request["method"];
            MethodInfo mi = this.GetType().GetMethod(methodName);
            if (mi != null && mi.GetCustomAttributes(typeof(ServiceMethodAttribute), true).Length > 0)
            {
                ServiceMethodAttribute invAttr = (ServiceMethodAttribute)mi.GetCustomAttributes(typeof(ServiceMethodAttribute), true)[0];
                ParameterInfo[] pars = mi.GetParameters();
                List<object> paramValues = new List<object>();
                bool paramParsingSuccess = true;
                JavaScriptSerializer jsonSer = new JavaScriptSerializer();
                foreach (ParameterInfo p in pars)
                {
                    string paramName = p.Name;
                    if (context.Request[paramName] != null)
                    {
                        object paramValue = null;
                        string typeName = p.ParameterType.FullName;
                        try
                        {
                            paramValue = jsonSer.Deserialize(context.Request[paramName], p.ParameterType);
                            paramValues.Add(paramValue);
                        }
                        catch (FormatException err)
                        {
                            paramParsingSuccess = false;
                            context.Response.StatusCode = HTTPStatus.BAD_REQUEST;
                            context.Response.StatusDescription = string.Format("Parameter \"{0}\" of invokable method \"{1}\" has an invalid value: \"{2}\". ", methodName, paramName, context.Request[paramName]);
                            if (_log != null)
                                _log.Error(context.Response.StatusDescription, err);
                            break;
                        }
                    }
                    else
                    {
                        paramParsingSuccess = false;
                        context.Response.StatusCode = HTTPStatus.BAD_REQUEST;
                        context.Response.StatusDescription = string.Format("Requested invokable method \"{0}\" requires parameter \"{1}\", which is not supplied.", methodName, paramName);
                        break;
                    }
                }
                if (paramParsingSuccess)
                {
                    try
                    {
                        object result = mi.Invoke(this, paramValues.ToArray()); // Call the target method
                        context.Response.ContentType = invAttr.MimeType;
                        if ((mi.ReturnType.IsSubclassOf(typeof(FileObject))))
                        {
                            if (result != null)
                            {
                                if (((FileObject)result).SendAsAttachment)
                                {
                                    string mimeType = ((FileObject)result).MimeType;
                                    System.Text.Encoding encoding = ((FileObject)result).Encoding;
                                    System.Web.HttpCacheability cacheability = ((FileObject)result).Cacheability;
                                    if (mimeType != null)
                                        context.Response.ContentType = mimeType;
                                    if (encoding != null)
                                        context.Response.ContentEncoding = encoding;
                                    context.Response.Cache.SetCacheability(cacheability);
                                    context.Response.AddHeader("Content-Disposition", "attachment;filename=" + ((FileObject)result).FileName);
                                }
                                if (result is TextFileObject)
                                {
                                    context.Response.Write(((TextFileObject)result).Data);
                                }
                                else
                                {
                                    byte[] data = ((BinaryFileObject)result).Data;
                                    context.Response.OutputStream.Write(data, 0, data.Length);
                                }
                            }
                            else
                            {
                                context.Response.StatusCode = HTTPStatus.RESOURCE_NOT_FOUND;
                                context.Response.StatusDescription = "Requested document not found.";
                                context.Response.Flush();
                            }
                        }
                        else if (invAttr.MimeType == "application/json")
                        {
                            try
                            {
                                //JavaScriptSerializer jsonSer = new JavaScriptSerializer();
                                /* Note that scalar values (which include strings) will be serialized as "direct values", 
                                 * for example: jsonSer.Serialize(100) => "100";
                                 *              jsonSer.Serialize(true) => "true";
                                 */
                                context.Response.Write(jsonSer.Serialize(result));
                            }
                            catch (InvalidOperationException err)
                            {
                                context.Response.StatusCode = HTTPStatus.SERVER_ERROR;
                                context.Response.StatusDescription = string.Format("JavaScript serialization error occured on requesting invokable method \"{0}\".", methodName);
                                if (_log != null)
                                    _log.Error(context.Response.StatusDescription, err);
                            }
                        }
                        else if (invAttr.MimeType.StartsWith("text"))
                        {
                            context.Response.Write(result);
                        }
                        else if (invAttr.MimeType.StartsWith("image")
                            || invAttr.MimeType.StartsWith("audio")
                            || invAttr.MimeType.StartsWith("video")
                            || invAttr.MimeType.StartsWith("application")
                            || invAttr.MimeType.StartsWith("multipart")) // binary types
                        {
                            try
                            {
                                if (result is byte[]) // binary types
                                {
                                    byte[] data = (byte[])result;
                                    context.Response.OutputStream.Write(data, 0, data.Length);
                                }
                                else if (result is string)
                                {
                                    context.Response.Write((string)result);
                                }
                                else
                                {
                                    context.Response.Write(result.ToString());
                                }
                            }
                            catch (InvalidCastException err)
                            {
                                context.Response.StatusCode = HTTPStatus.SERVER_ERROR;
                                context.Response.StatusDescription = string.Format("Requested invokable method \"{0}\" is expected to return a byte array.", methodName);
                                if (_log != null)
                                    _log.Error(context.Response.StatusDescription, err);
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        context.Response.StatusCode = HTTPStatus.SERVER_ERROR;
                        context.Response.StatusDescription = string.Format("Server error. Method : {0}", methodName);
                        if (_log != null)
                            _log.Error(context.Response.StatusDescription, err);

                    }
                }
            }
            else
            {
                context.Response.StatusCode = HTTPStatus.BAD_REQUEST;
                context.Response.StatusDescription = string.Format("Requested invokable method \"{0}\" not defined.", methodName);
            }
        }
    }

    public struct HTTPStatus
    {
        public static int OK = 200;
        public static int BAD_REQUEST = 400;
        public static int RESOURCE_NOT_FOUND = 404;
        public static int SERVER_ERROR = 500;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ServiceMethodAttribute : Attribute
    {
        private string _mimeType = "application/json";
        public string MimeType
        {
            get { return _mimeType; }
            set { _mimeType = value; }
        }
    }

    /// <summary>
    /// The FileObject class is the base class for derived classes TextFileObject, BinaryFileObject, which are used as return types for invokable methods,
    /// which need to return named user-downloadable files.
    /// These types are only meant for user-downloadable files. For files that are used inline, for simplicity,
    /// specifying the Mimetype is still the preferred approach, unless file names are explictly needed in that case.
    /// </summary>
    public abstract class FileObject
    {
        public string _fileName;
        /// <summary>
        /// File name 
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public System.Text.Encoding _encoding;
        /// <summary>
        /// File encoding 
        /// </summary>
        public System.Text.Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        public string _mimeType;

        /// <summary>
        /// Mime type. If set, this overrides the MimeType setting at the attribute level.
        /// </summary>
        public string MimeType
        {
            get { return _mimeType; }
            set { _mimeType = value; }
        }

        private bool _sendAsAttachment = false;
        /// <summary>
        /// True if the file is transmitted in the response as an attachment. If false, it will be transmitted in-line.
        /// </summary>
        public bool SendAsAttachment
        {
            get { return _sendAsAttachment; }
            set { _sendAsAttachment = value; }
        }

        private HttpCacheability _cacheability = System.Web.HttpCacheability.NoCache;
        /// <summary>
        /// HttpCacheability
        /// </summary>
        public HttpCacheability Cacheability
        {
            get { return _cacheability; }
            set { _cacheability = value; }
        }

        /// <summary>
        /// Default contructor. The file will be transmitted in-line in the response.
        /// </summary>
        public FileObject()
        {
        }

        /// <summary>
        /// Constructor allowing the user of this class to specify the file name and whether it will be send as an attachment in the response.
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="sendAsAttachment">True if the file is transmitted in the response as an attachment. If false, it will be transmitted in-line.</param>
        public FileObject(string fileName, bool sendAsAttachment)
        {
            _fileName = fileName;
            _sendAsAttachment = sendAsAttachment;
        }
    }

    /// <summary>
    /// FileObject descendant to handle text files.
    /// </summary>
    public class TextFileObject : FileObject
    {
        /// <summary>
        /// The payload data of the file.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TextFileObject()
            : base()
        {
        }

        /// <summary>
        /// Constructor allowing the user of this class to specify the file name and whether it will be send as an attachment in the response.
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="sendAsAttachment">True if the file is transmitted in the response as an attachment. If false, it will be transmitted in-line.</param>
        public TextFileObject(string fileName, bool sendAsAttachment)
            : base(fileName, sendAsAttachment)
        {
        }
    }

    /// <summary>
    /// FileObject descendant to handle binary files.
    /// </summary>
    public class BinaryFileObject : FileObject
    {
        /// <summary>
        /// The payload data of the file.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BinaryFileObject()
            : base()
        {
        }

        /// <summary>
        /// Constructor allowing the user of this class to specify the file name and whether it will be send as an attachment in the response.
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="sendAsAttachment">True if the file is transmitted in the response as an attachment. If false, it will be transmitted in-line.</param>
        public BinaryFileObject(string fileName, bool sendAsAttachment)
            : base(fileName, sendAsAttachment)
        {
        }
    }
}
