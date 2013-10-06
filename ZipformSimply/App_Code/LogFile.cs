using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;

namespace Common
{
    public static class LogFile
    {
        public enum ReportingAction
        {
            create,
            newFile,
            lineNumber,
            processStep,
            TESTING,
            warning,
            error
        }

        public static string LogFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["logTextFilePath"];
            }
        }

        /// <summary>
        /// Check / create logfile upon initilization
        /// </summary>
        static LogFile()
        {
            try
            {
                if (!File.Exists(LogFilePath))
                {
                    try
                    {
                        Directory.CreateDirectory((Path.GetDirectoryName(LogFilePath)));
                        File.CreateText(LogFilePath);
                        FileStream objFileStream = new FileStream(LogFilePath, FileMode.Open, FileAccess.Write);
                        StreamWriter objStreamWriter = new StreamWriter(objFileStream);
                        objFileStream.Seek(0, SeekOrigin.End);
                        objStreamWriter.WriteLine("*** LogFile Created ***");
                        objStreamWriter.WriteLine();
                        objStreamWriter.Close();
                    }
                    catch (Exception ex)
                    {
                        HelperClass objHelper = new HelperClass();
                        objHelper.WriteEventLog(String.Format("Cannot create text log file. Error: {0}", ex.Message), EventLogEntryType.Warning);
                    }
                }
              
            }
            catch (Exception ex)
            {
                HelperClass objHelper = new HelperClass();
                objHelper.WriteEventLog(String.Format("Cannot create text log file. Error: {0}", ex.Message), EventLogEntryType.Warning);
            }
        }


        public static void writeToLog(string message)
        {
            try
            {
                FileStream objFileStream = new FileStream(LogFilePath, FileMode.Open, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter(objFileStream);
                objFileStream.Seek(0, SeekOrigin.End);
                objStreamWriter.WriteLine(message);
                objStreamWriter.WriteLine();
                objStreamWriter.Close();
            }
            catch (Exception ex)
            {
                HelperClass objHelper = new HelperClass();
                objHelper.WriteEventLog(String.Format("Cannot write to text log: Error: {0}", ex.Message), EventLogEntryType.Warning);
            }
        }

        public static string writeLogAsync(string path)
        {

            return "";
        }

        public static void writeToLog(string message, ReportingAction action)
        {
            string fileName;
            string fileUploadTime;
            string errorEvent = string.Empty;
            StringBuilder strBuild = new StringBuilder();
            try
            {
                FileInfo objCopiedFile = new FileInfo(LogFilePath);
                fileName = objCopiedFile.Name;
                fileUploadTime = objCopiedFile.CreationTime.ToString();
            }
            catch (Exception ex)
            {
                fileName = Path.GetFileName(LogFilePath);
                fileUploadTime = "N/A";
                errorEvent = "cannot access file";
                HelperClass objHelper = new HelperClass();
                objHelper.WriteEventLog(String.Format("Cannot write to text log: Error: {0}", ex.Message), EventLogEntryType.Warning);
            }

            switch (action)
            {
                    
                case ReportingAction.newFile:
                    strBuild.AppendLine("** File uploaded ***");
                    strBuild.AppendLine(String.Format("Name: {0}", fileName));
                    strBuild.AppendLine(String.Format("Time copied: {0}", fileUploadTime));
                    strBuild.AppendLine(String.Format("Start process time: {0}", DateTime.Now));
                    break;
                case ReportingAction.lineNumber:

                    break;
                case ReportingAction.error:
                     strBuild.AppendLine("** Error ***");
                     strBuild.AppendLine(String.Format("Encoutered error at: {0}", DateTime.Now));
                     strBuild.AppendLine(String.Format("{0}", message));
                    break;
                case ReportingAction.processStep:
                    strBuild.AppendLine("Processing---");
                    break;
                case ReportingAction.warning:
                    HelperClass objHelper = new HelperClass();
                    strBuild.AppendLine(String.Format("** WARNING: {0}", message));
                    objHelper.WriteEventLog(String.Format("WARNING: {0}", message), EventLogEntryType.Warning);
                    break;
            }

                // Output formatted string to logFile
            try
            {
                FileStream objFileStream = new FileStream(LogFilePath, FileMode.Open, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter(objFileStream);
                objFileStream.Seek(0, SeekOrigin.End);
                objStreamWriter.Write(strBuild);
                objStreamWriter.Close();
            }
            catch (Exception ex)
            {
                HelperClass objHelper = new HelperClass();
                objHelper.WriteEventLog(String.Format("Cannot write to text log: Error: {0}", ex.Message), EventLogEntryType.Warning);
            }

        }
        //public static void writeToLog(string message)
        //{
        //    string fileName;
        //    string fileUploadTime;
        //    string errorEvent = string.Empty;
        //    StringBuilder strBuild = new StringBuilder();
        //    try
        //    {
        //        FileInfo objCopiedFile = new FileInfo(LogFilePath);
        //        fileName = objCopiedFile.Name;
        //        fileUploadTime = objCopiedFile.CreationTime.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        fileName = Path.GetFileName(LogFilePath);
        //        fileUploadTime = "N/A";
        //        errorEvent = "cannot access file";
        //        HelperClass objHelper = new HelperClass();
        //        objHelper.WriteEventLog(String.Format("Cannot write to text log: Error: {0}", ex.Message), EventLogEntryType.Warning);
        //    }

        //    strBuild.AppendLine(message);
        //    try
        //    {
        //        FileStream objFileStream = new FileStream(LogFilePath, FileMode.Open, FileAccess.Write);
        //        StreamWriter objStreamWriter = new StreamWriter(objFileStream);
        //        objFileStream.Seek(0, SeekOrigin.End);
        //        objStreamWriter.Write(strBuild);
        //        objStreamWriter.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperClass objHelper = new HelperClass();
        //        objHelper.WriteEventLog(String.Format("Cannot write to text log: Error: {0}", ex.Message), EventLogEntryType.Warning);
        //    }

        //}

    }
}
