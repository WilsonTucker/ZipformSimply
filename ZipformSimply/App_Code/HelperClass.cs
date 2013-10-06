using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common
{
    public class HelperClass
    {
        public HelperClass()
        {
             
        }

        public enum ValidationType
        {
            Email, Date, Currency, RealNumber, PositiveInt, IntegerNonZero, DecimalNonZero
        }
        private Hashtable RegexLib
        {
            get
            {
                Hashtable RegexLib = new Hashtable();
                
                //RegexLib.Add(ValidationType.Email, @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
                RegexLib.Add(ValidationType.Email, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
                RegexLib.Add(ValidationType.Date, @"");
                RegexLib.Add(ValidationType.Currency, @"^\d+(?:\.\d{0,2})?$");
                RegexLib.Add(ValidationType.RealNumber, @"^[-+]?\d+(\.\d+)?$");
                RegexLib.Add(ValidationType.PositiveInt, @"^\d+$");
                RegexLib.Add(ValidationType.IntegerNonZero, @"^[1-9]+[0-9]*$");
                RegexLib.Add(ValidationType.DecimalNonZero, @"^[1-9]+[0-9]*(?:\.\\d+)?$");

                return RegexLib;
            }
        }

        public String GetValidationExpression(ValidationType type)
        {
            String strExpression = "";
            strExpression = RegexLib[type].ToString();
            return strExpression;
        }

        public Int64 parseInt64(int? value)
        {
            Int64 retVal = 0;
            if (value.HasValue)
            {
                retVal = (Int64)value;
            }
            return retVal;
        }
        public Int64 parseInt64(string value)
        {
            Int64 retVal = 0;
            Int64.TryParse(value, out retVal);
            return retVal;
        }
        public Int64 parseInt64(long? value)
        {
            Int64 retVal = 0;
            if (value.HasValue)
            {
                retVal = (Int64)value;
            }
            return retVal;
        }
        public Int64 parseInt64(JToken value)
        {
            Int64 retVal = 0;
            if (value != null)
            {
                retVal = this.parseInt64(value.Value<String>());
            }
            return retVal;
        }

        public int? parseInt(int value)
        {
            int? retVal = null;
            retVal = value;
            return retVal;
        }
        public int? parseInt(string value)
        {
            Int64 tempVal = 0;
            Int64.TryParse(value, out tempVal);
            int? retVal = null;
            retVal = (int?)tempVal;
            return retVal;
        }
        public int parseInt(int? value)
        {
            int retVal = 0;
            if (value.HasValue)
            {
                retVal = value.Value;
            }
            return retVal;
        }
        public Int32 parseInt32(string value)
        {
            Int32 retVal = 0;
            Int32.TryParse(value, out retVal);
            return retVal;
        }
        public Int32 parseInt32(JToken value)
        {
            Int32 retVal = 0;
            if (value != null)
            {
                retVal = this.parseInt32(value.Value<String>());
            }
            return retVal;
        }

        public double parseDouble(double? value)
        {
            double retVal = 0.0;
            if (value.HasValue)
            {
                retVal = (double)value;
            }
            return retVal;
        }
        public double parseDouble(decimal? value)
        {
            double retVal = 0.0;
            if (value.HasValue)
            {
                retVal = (double)value;
            }
            return retVal;
        }
        public Double parseDouble(string value)
        {
            Double retVal = 0;
            Double.TryParse(value, out retVal);
            return retVal;
        }
        public Double parseDouble(JToken value)
        {
            Double retVal = 0;
            if (value != null)
            {
                retVal = this.parseDouble(value.Value<String>());
            }
            return retVal;
        }

        public Decimal parseDecimal(decimal? value)
        {
            Decimal retVal = 0;
            if (value.HasValue)
            {
                retVal = (Decimal)value;
            }
            return retVal;
        }
        public Decimal parseDecimal(string value)
        {
            Decimal retVal = 0;
            Decimal.TryParse(value, out retVal);
            return retVal;
        }
        public Decimal parseDecimal(Double value)
        {
            Decimal retVal = 0;
            retVal = (Decimal)value;
            return retVal;
        }
        public Decimal parseDecimal(Double? value)
        {
            Decimal retVal = 0;
            if (value.HasValue)
            {
                retVal = (Decimal)value;
            }
            return retVal;
        }

        public Boolean parseBool(bool? value)
        {
            Boolean retVal = false;
            if (value.HasValue)
            {
                retVal = (Boolean)value;
            }
            return retVal;
        }
        public Boolean parseBool(String value)
        {
            Boolean retVal = false;
            if (value == "1" || value == "True" || value == "true")
            {
                retVal = true;
            }
            return retVal;
        }
        public Boolean parseBool(JToken value)
        {
            Boolean retVal = false;
            if (value != null)
            {
                retVal = this.parseBool(value.Value<String>());
            }
            return retVal;
        }

        public String parseString(int? value)
        {
            String retVal = "";
            if (value.HasValue)
            {
                retVal = value.ToString();
            }
            return retVal;
        }
        public String parseString(decimal? value)
        {
            String retVal = "";
            if (value.HasValue)
            {
                retVal = value.ToString();
            }
            return retVal;
        }
        public String parseString(double? value)
        {
            String retVal = "";
            if (value.HasValue)
            {
                retVal = value.ToString();
            }
            return retVal;
        }
        public String parseString(bool? value)
        {
            String retVal = "";
            if (value.HasValue)
            {
                retVal = value.ToString();
            }
            return retVal;
        }
        public String parseString(string value)
        {
            String retVal = "";
            if (!String.IsNullOrEmpty(value))
            {
                retVal = value.ToString();
            }
            return retVal;
        }
        public String parseString(JToken value)
        {
            String retVal = "";
            if (value != null)
            {
                retVal = value.Value<String>();
            }
            return retVal;
        }

        public DateTime parseDate(String value)
        {
            DateTime objRetVal = DateTime.MinValue;
            DateTime.TryParse(value, out objRetVal);
            return objRetVal;
        }
        public DateTime parseDate(DateTime? value)
        {
            DateTime retVal = DateTime.MinValue;
            if (value.HasValue)
            {
                retVal = value.Value;
            }
            return retVal;
        }
        public DateTime parseDate(JToken value)
        {
            DateTime retVal = DateTime.MinValue;
            if (value != null)
            {
                retVal = this.parseDate(value.Value<String>());
            }
            return retVal;
        }

        public List<Int64> toInt64List(ref IEnumerable<System.Data.SqlTypes.SqlInt64> targetList)
        {
            List<Int64> arrRetVal = new List<Int64>();
            //if (targetList != null && targetList.Count() > 0)
            //{
            foreach (Int64 intRetVal in targetList)
            {
                arrRetVal.Add(intRetVal);
            }
            //}
            return arrRetVal;
        }
        public List<Int64> toInt64List(String SourceString, char[] delimiter)
        {
            String[] arrParts = SourceString.Split(delimiter);
            List<Int64> arrRetVal = new List<Int64>();
            if (arrParts != null)
            {
                foreach (String strValue in arrParts)
                {
                    if(strValue.Length > 0)
                    {
                        arrRetVal.Add(parseInt64(strValue));
                    }
                }
            }
            return arrRetVal;
        }
        public List<String> toStringList(String SourceString, char[] delimiter)
        {
            String[] arrParts = SourceString.Split(delimiter);
            List<String> arrRetVal = new List<String>();
            if (arrParts != null)
            {
                foreach (String strValue in arrParts)
                {
                    if (strValue.Length > 0)
                    {
                        arrRetVal.Add(strValue);
                    }
                }
            }
            return arrRetVal;
        }

        public String int32ListToCSV(List<Int32> TargetArray)
        {
            String strRetVal = "";
            if (TargetArray != null)
            {
                if (TargetArray.Count > 0)
                {
                    foreach (Int32 intValue in TargetArray)
                    {
                        strRetVal += intValue.ToString() + ",";
                    }
                }
            }
            if (strRetVal.EndsWith(","))
            {
                strRetVal = strRetVal.Substring(0, strRetVal.Length - 1);
            }
            return strRetVal;
        }
        public String int64ListToCSV(List<Int64> TargetArray)
        {
            String strRetVal = "";
            if (TargetArray != null)
            {
                if (TargetArray.Count > 0)
                {
                    foreach (Int64 intValue in TargetArray)
                    {
                        strRetVal += intValue.ToString() + ",";
                    }
                }
            }
            if (strRetVal.EndsWith(","))
            {
                strRetVal = strRetVal.Substring(0, strRetVal.Length - 1);
            }
            return strRetVal;
        }

        public String uniDate(DateTime value)
        {
            return value.Year + "/" + value.Month + "/" + value.Day;
        }

        public Int32 RandomNumber(Int32 Min, Int32 Max)
        {
            Random objRandom = new Random();
            return objRandom.Next(Min, Max);
        }

        public byte[] GetEmbeddedResource(string FullAssemblyNameSpace)
        {
            Assembly objCurrentAssembly = Assembly.GetCallingAssembly();
            Stream objStream = objCurrentAssembly.GetManifestResourceStream(FullAssemblyNameSpace);
            byte[] arrResourceBytes = new byte[objStream.Length];

            objStream.Read(arrResourceBytes, 0, (int)objStream.Length);

            return arrResourceBytes;
        }
        public Object ReflectTypeInstance(string AssemblyName, string ControlTypeName)
        {
            Object objReflectedControl = null;

            if (AssemblyName.Length > 0)
            {
                Assembly objTypeAssembly = Assembly.Load(AssemblyName);
                Type objCtlType = objTypeAssembly.GetType(ControlTypeName);
                if (objCtlType != null)
                {
                    objReflectedControl = Activator.CreateInstance(objCtlType);
                }
            }
            else
            {
                foreach (Assembly objAssembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (Activator.CreateInstance(objAssembly.FullName, ControlTypeName) != null)
                        {
                            Type objCtlType = objAssembly.GetType(ControlTypeName);
                            if (objCtlType != null)
                            {
                                objReflectedControl = Activator.CreateInstance(objCtlType);
                            }
                        }
                    }
                    catch (System.Exception)
                    {
                        continue;
                    }
                }
            }
            return objReflectedControl;
        }

        public class EmailContent
        {
            private String strEmailTemplatePathLocal = "";
            private String strImageHostDomain = "";

            private String strSubject = "";
            private String strBodyText = "";

            private String strGreeting = "";
            private String strPartingComments = "";
            private String strFooterText = "";

            public String EmailTemplatePathLocal
            {
                get
                {
                    return strEmailTemplatePathLocal;
                }
                set
                {
                    strEmailTemplatePathLocal = value;
                }

            }
            public String ImageHostDomain
            {
                get
                {
                    return strImageHostDomain;
                }
                set
                {
                    strImageHostDomain = value;
                }

            }

            public String Subject
            {
                get
                {
                    return strSubject;
                }
                set
                {
                    strSubject = value;
                }

            }
            public String BodyText
            {
                get
                {
                    return strBodyText;
                }
                set
                {
                    strBodyText = value;
                }
            }

            public String Greeting
            {
                get
                {
                    return strGreeting;
                }
                set
                {
                    strGreeting = value;
                }
            }
            public String PartingComments
            {
                get
                {
                    return strPartingComments;
                }
                set
                {
                    strPartingComments = value;
                }
            }
            public String FooterText
            {
                get
                {
                    return strFooterText;
                }
                set
                {
                    strFooterText = value;
                }
            }

            public EmailContent()
            {

            }
        }
        public bool SendEmail(string ToAddress, EmailContent objContent, string CC)
        {
            try
            {
                
                String strSMTPUsername = System.Configuration.ConfigurationManager.AppSettings["MailUsername"];
                String strSMTPPassword = System.Configuration.ConfigurationManager.AppSettings["MailPassword"];

                SmtpClient objSmtpClient = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["SMTPHost"]);
                String strFromAddress = System.Configuration.ConfigurationManager.AppSettings["DefaultMailFrom"];
                String strDefaultMailRecipient = System.Configuration.ConfigurationManager.AppSettings["DebugRecipient"];
                MailMessage objMailMessage = new MailMessage();
                MailAddress objFromMailAddress = new MailAddress(strFromAddress);
                String strEmailTemplate = "";

                if (strSMTPUsername.Length > 0 && strSMTPPassword.Length > 0)
                {
                    objSmtpClient.UseDefaultCredentials = false;
                    objSmtpClient.Credentials = new NetworkCredential(strSMTPUsername, strSMTPPassword);
                }
                if (objContent.EmailTemplatePathLocal.Length > 0)
                {
                    strEmailTemplate = ReadFile(objContent.EmailTemplatePathLocal);
                    strEmailTemplate = strEmailTemplate.Replace("[ServerPath]", objContent.ImageHostDomain);
                    strEmailTemplate = strEmailTemplate.Replace("[Greeting]", objContent.Greeting);
                    strEmailTemplate = strEmailTemplate.Replace("[PartingComments]", objContent.PartingComments);
                    strEmailTemplate = strEmailTemplate.Replace("[FooterText]", objContent.FooterText);
                    strEmailTemplate = strEmailTemplate.Replace("[BodyText]", objContent.BodyText);
                }
                else
                {
                    strEmailTemplate = "<p>" + objContent.Greeting + "</p>" +
                                        "<p>" + objContent.BodyText + "</p>" +
                                        "<p>" + objContent.PartingComments + "</p>" +
                                        "<p>" + objContent.FooterText + "</p>";
                }
                
                if (ToAddress.IndexOf(",") > -1)
                {
                    string[] arrAddresses = ToAddress.Split(',');
                    foreach (string strAddress in arrAddresses)
                    {
                        if (strAddress.Trim().Length > 0)
                        {
                            objMailMessage.To.Add(strAddress.Trim());
                        }
                    }
                }
                else
                {
                    if (ToAddress.Length > 0)
                    {
                        objMailMessage.To.Add(ToAddress);
                    }
                    else
                    {
                        objMailMessage.To.Add(strDefaultMailRecipient);
                    }
                }
                if (CC.Length > 0)
                {
                    foreach (string strCCEmail in CC.Split(','))
                    {
                        objMailMessage.Bcc.Add(new MailAddress(strCCEmail));
                    }
                }
                objMailMessage.From = objFromMailAddress;
                objMailMessage.Subject = objContent.Subject;
                objMailMessage.Body = strEmailTemplate;
                objMailMessage.IsBodyHtml = true;

                objSmtpClient.Send(objMailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Stream StringToStream(String SourceString)
        {
            byte[] arrBytes = Encoding.ASCII.GetBytes(SourceString);
            MemoryStream objStream = new MemoryStream(arrBytes);

            return objStream;
        }
        public string ReadFile(String Path)
        {
            String strFileContents = "";
            FileStream objFileStream;
            StreamReader objStreamReader;

            if (File.Exists(Path))
            {
                objFileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
                objStreamReader = new StreamReader(objFileStream);

                strFileContents = objStreamReader.ReadToEnd();
                objStreamReader.Close();
                objFileStream.Close();
            }

            return strFileContents;
        }

        public void WriteEventLog(String Message, EventLogEntryType ErrorType)
        {
            EventLog objEvtLog = new EventLog();
            if (!EventLog.SourceExists(ConfigurationManager.AppSettings["ProductName"]))
            {
                EventLog.CreateEventSource(ConfigurationManager.AppSettings["ProductName"], ConfigurationManager.AppSettings["ProductName"]);
            }
            objEvtLog.Source = ConfigurationManager.AppSettings["ProductName"];
            objEvtLog.WriteEntry(Message, ErrorType);
        }

        public String GetHttpContent(String URLPath)
        {
            ServicePointManager.Expect100Continue = false;
            ASCIIEncoding objEncoding = new ASCIIEncoding();
            HttpWebRequest objWebRequest = (HttpWebRequest)WebRequest.Create(URLPath);
            WebResponse obgWebResponse;
            Stream objReceiveStream;
            StreamReader objStreamReader;

            objWebRequest.Method = "GET";
            objWebRequest.Timeout = 36000000; //10 hours...
            obgWebResponse = objWebRequest.GetResponse();
            objReceiveStream = obgWebResponse.GetResponseStream();
            objStreamReader = new StreamReader(objReceiveStream);

            return objStreamReader.ReadToEnd();
        }
    }
}