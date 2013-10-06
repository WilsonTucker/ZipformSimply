using System;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

//using System.Configuration.ApplicationSettingsGroup;
//using System.Configuration.ClientSettingsSection;


using Common;

namespace ZipformSimply
{
    enum ProcessStep
    {
        step1 = 1,
        step2 = 2,
        step3 = 3,
        step4 = 4,
        step5 = 5,
        step6 = 6
    }
    enum ProcessStatus
    {
        running,
        finished

    }
    enum ExecutableTypes
    {
        DOC1,
        Xharbour,
        Dataflow,
        NA
    }


    class ProcessTask
    {
        private bool errorWhileProcessing;
        private string currentPath;     //current system dir path for process job
        private string pathToCopyTo;
        private string pathtoCopyFrom;
        private string HourValue;
        private string NewDir;
        private bool isRunning;
        private ProcessStep currentStep;

        ProcessFile objFile;
        HelperClass objHelper = new HelperClass();

        public string CURRENT_PATH
        {
            get { return currentPath; }
            set { currentPath = value; }
        }
        public string DATE_FORMAT
        {
            get
            {
              return ConfigurationManager.AppSettings["DateFormat"];
            } 
        }

        public string TIME_FORMAT
        {
            get
            {
                return ConfigurationManager.AppSettings["TimeFormat"];
            }
        }

        public ProcessTask(string filePath)
        {
            isRunning = true;
            errorWhileProcessing = false;
            currentPath = Path.GetDirectoryName(filePath);
            HourValue = DateTime.Now.ToString("hh:mm:ss:ff");
            NewDir = "";    //MC larch...
            currentStep = ProcessStep.step1;
            objFile = new ProcessFile(filePath);

            runTask(currentStep);
        }

        public bool runTask(ProcessStep currentStep)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            //  while (isRunning)
            // {
            try
            {
                //          switch (currentStep)
                //          {
                //              case ProcessStep.step1:
                ClientSettingsSection config = (ClientSettingsSection)ConfigurationManager.GetSection("applicationSettings/ZipformSimply.ProcessSteps");
                if (config != null)
                {
                    LogFile.writeToLog("Processing Started....");

                    // Loop through processing steps...
                    foreach (System.Configuration.SettingElement entry in config.Settings)
                    {
                        //TODO: throw custom error
                        //         while (!errorWhileProcessing)
                        //        {
                        LogFile.writeToLog(String.Format("**** Processing: {0} ", entry.Name));

                        xmlDoc.LoadXml(String.Format("<root>{0}</root>", entry.Value.ValueXml.InnerXml));
                        foreach (XmlElement node in xmlDoc.ChildNodes)
                        {
                            foreach (XmlElement step in node.ChildNodes)
                            {

                                string childFilePath = step.FirstChild.InnerText.Trim();
                                switch (step.Name)
                                {
                                    case "goToDirectory":
                                        gotoDirectory(childFilePath);
                                        LogFile.writeToLog(String.Format("SET current directory to:{0} ", childFilePath));
                                        break;
                                    case "deleteFiles":
                                        LogFile.writeToLog(String.Format("DELETE files matching: {0} ", childFilePath));
                                        deleteFiles(step);
                                        break;
                                    case "copyFiles":
                                        LogFile.writeToLog(String.Format("COPY files matching: {0} ", childFilePath));
                                        copyFiles(step);
                                        break;
                                    case "runBatchFile":
                                        LogFile.writeToLog(String.Format("RUN Batch file with name {0} ", childFilePath));
                                        runBatchFile(step);
                                        break;
                                    case "writeToLog":
                                        string strMessage = childFilePath;
                                        strMessage = strMessage.Replace("%time%", DateTime.Now.ToString(TIME_FORMAT));
                                        strMessage = strMessage.Replace("%date%", DateTime.Now.ToString(DATE_FORMAT));
                                        LogFile.writeToLog(strMessage);
                                        LogFile.writeToLog(" ----->>");

                                        break;
                                    default:
                                        //NOT defined
                                        LogFile.writeToLog(String.Format("appConfig step: {0} is not formatted correctly, step ignored ", step.Name),LogFile.ReportingAction.warning);
                                        break;
                                }
                            }
                        }

                        //            }//whileNoError
                    }//for steps 
                    LogFile.writeToLog(String.Format("COMPLETED all steps"));

                } //not null

                return false;
                //                   break;
                //            }//switch
            }
            catch (ProcessingException ex)
            {
                LogFile.writeToLog(" --- TERMINATING PROCESS JOB --- ");
                //TODO email log
            }
            //    isRunning = false;
            return false; //TESTING
            //  }//while

            //TODO: total process time
            return true;
        }

        protected bool gotoDirectory(string pathToSet)
        {
            try
            {
                if (Directory.Exists(pathToSet))
                {
                    currentPath = pathToSet;
                }
                else
                {
                    LogFile.writeToLog(String.Format("Directory doesn't exist: {0}", pathToSet));
                    LogFile.writeToLog("    Try to create...");
                    Directory.CreateDirectory(pathToSet);
                }
            }
            catch (Exception ex)
            {
                LogFile.writeToLog(String.Format("Error trying to create directory: {0}", ex.Message), LogFile.ReportingAction.warning);
                LogFile.writeToLog("    Retrying...");

                Directory.CreateDirectory(pathToSet);
            }

            if (!Directory.Exists(pathToSet))
            {
                errorWhileProcessing = true;
                throw new ProcessingException(String.Format("Error: Failed to create directory: {0}", pathToSet));
            }


            return true;
        }

        protected bool runBatchFile(XmlElement processSection)
        {

            int maxProcessTime = (Int32)objHelper.parseInt(ConfigurationManager.AppSettings["maxTimeForProcessedTask"]);
            string strBatchFilePath = String.Empty;
            string batchName = String.Empty;
            string outputLogFile = String.Empty;
            string batchParameter = String.Empty;
            string logfileSuccessCode = String.Empty;
            string strExecType = String.Empty;
            ExecutableTypes objType = ExecutableTypes.NA;

            foreach (XmlElement childNode in processSection.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "name":
                        batchName = childNode.InnerText;
                        break;
                    case "outputFileName":
                        outputLogFile = childNode.InnerText;
                        break;
                    case "parameter":
                        batchParameter = "/" + childNode.InnerText;
                        break;
                    case "successCode":
                        logfileSuccessCode = childNode.InnerText;
                        break;
                }
            }

            objType = getBatchType(batchName);
            strBatchFilePath = Path.Combine(CURRENT_PATH, batchName);
            Process myProcess;
            if (File.Exists(strBatchFilePath))
            {
                try
                {
                    LogFile.writeToLog(String.Format("    Starting batch call for file: {0}", strBatchFilePath));

                    myProcess = new Process();
                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    myProcess.StartInfo.RedirectStandardError = true;
                    myProcess.StartInfo.RedirectStandardOutput = true;
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.FileName = @"C:\WINDOWS\SYSTEM32\CMD.EXE";
                    myProcess.StartInfo.Arguments = @"/c " + strBatchFilePath;// +batchParameter;
                    myProcess.EnableRaisingEvents = true;
                    myProcess.Start();
                    myProcess.WaitForExit(maxProcessTime);
                    string output = myProcess.StandardOutput.ReadToEnd();       //might not need
                    LogFile.writeToLog(String.Format("    finished processing", strBatchFilePath));

                }
                catch (Exception ex)
                {
                    errorWhileProcessing = true;
                    throw new ProcessingException(String.Format("Error encountered while calling {0}: {1}", batchName, ex.Message));
                }

                if (myProcess.ExitCode != 0)
                {
                    errorWhileProcessing = true;
                    throw new ProcessingException(String.Format("Error encountered while calling {0}. The program did not exit cleanly, Exit code: {1}", batchName, myProcess.ExitCode));
                }
                else
                {
                    string strvalidationError = "";
                    if (!validateLogFileOutput(outputLogFile, logfileSuccessCode, ref strvalidationError))
                    {
                        errorWhileProcessing = true;
                        throw new ProcessingException(String.Format("Error during " + objType.ToString() + " execution. Logfile validation: " + strvalidationError, strBatchFilePath));
                    }
                }
            }
            else
            {
                throw new ProcessingException(String.Format("Error during " + objType.ToString() + " execution. File: {0} doesn't exist "), strBatchFilePath);
            }

            return true;
        }

        protected bool copyFiles(XmlElement processSection)
        {
            string extensionFilter = String.Empty;
            string strCopyfromDir = String.Empty;
            string strOverwriteFileName = String.Empty;
            string strMergeFromFile = String.Empty;
            string strCopyToDir = CURRENT_PATH;
            IEnumerable<FileInfo> fileArray = Enumerable.Empty<FileInfo>();
            DirectoryInfo dir;

            try
            {
                dir = new DirectoryInfo(currentPath);
                foreach (XmlElement childNode in processSection.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "filter":
                            extensionFilter = childNode.InnerText;
                            break;
                        case "directory":
                            strCopyfromDir = childNode.InnerText;
                            break;
                        case "overwriteFileName":
                            strOverwriteFileName = childNode.InnerText;
                            break;
                        case "mergeFrom":
                            strMergeFromFile = childNode.InnerText;
                            break;
                    }
                }

                // Get all files with filter, read to string, write to file 
                string strAppendedText = string.Empty;
                int arrayLength = 0;
                string[] strFileArray;
                string strFileCopyPath = Path.Combine(strCopyToDir, strOverwriteFileName);

                //TODO refactor to use Process.copy
                try
                {
                    strFileArray = GetFilesByExtension(strCopyfromDir, extensionFilter, SearchOption.TopDirectoryOnly);
                    FileInfo objDestionationFile = new FileInfo(strFileCopyPath);
                    checkCreateFile(ref objDestionationFile);

                    if (objDestionationFile != null)
                    {

                        foreach (string strFile in strFileArray)
                        {
                            try
                            {
                                using (StreamReader reader = new StreamReader(strFile))
                                {
                                    strAppendedText += reader.ReadToEnd();
                                }

                                LogFile.writeToLog(String.Format("    copy file: {0}", strFile));
                            }
                            catch (Exception ex)
                            {
                                //TODO: logfile warning
                                LogFile.writeToLog(String.Format("Error: could not copy file: {0}", strFile));
                            }

                        }
                    }
                    else
                    {
                        //TODO throw custom
                        // throw new ProcessingException(String.Format("Cannot find file at {0}", strFileCopyPath)); - not sure if Should thow here

                    }
                    arrayLength = strFileArray.Length; // dont need
                    using (TextWriter tw = new StreamWriter(objDestionationFile.Open(FileMode.Create)))
                    {
                        //append header textfile
                        if (!String.IsNullOrEmpty(strMergeFromFile))
                        {
                            string strHeaderFilePath = Path.Combine(CURRENT_PATH, strMergeFromFile);
                            if (File.Exists(strHeaderFilePath))
                            {
                                tw.Write(objHelper.ReadFile(strHeaderFilePath));
                            }
                        }
                        tw.Write(strAppendedText);

                    }
                }
                catch (Exception ex)
                {
                    //TODO: throw custom error
                    throw new ProcessingException(ex.Message); //TESTING
                }
                //  File.AppendAllText(Path.Combine(strCopyToDir, strOverwriteFileName), strAppendedText);

                if (arrayLength == 0)
                {
                    LogFile.writeToLog("0 files copied", LogFile.ReportingAction.warning);
                }
                else
                {
                    LogFile.writeToLog(String.Format("{0} file/s copied", Convert.ToString(arrayLength)));
                }
                //       throw new ProcessingException("testing"); //TESTING

            }
            catch (Exception ex)
            {
                //TODO: error
                objHelper.WriteEventLog("Error during delete operation", EventLogEntryType.Error);
                LogFile.writeToLog(String.Format("Error encountered during delete operation: {0}", ex.Message));
                errorWhileProcessing = true;
                throw new ProcessingException(ex.Message);
                // return false;
            }

            return true;
        }

        protected bool deleteFiles(XmlElement processSection)
        {
            string extensionFilter = String.Empty;
            IEnumerable<FileInfo> fileArray = Enumerable.Empty<FileInfo>();
            // DirectoryInfo dir;
            try
            {
                //  dir = new DirectoryInfo(currentPath);
                foreach (XmlElement childNode in processSection.ChildNodes)
                {
                    if (childNode.Name == "filter")
                    {
                        extensionFilter = childNode.InnerText;
                    }
                }

                // Get all files with extensions
                //TODO: fix delete function!! not returning anything
                string[] strFileArray = GetFilesByExtension(currentPath, extensionFilter, SearchOption.TopDirectoryOnly);
                foreach (string strFile in strFileArray)
                {
                    try
                    {
                        File.Delete(strFile);
                        LogFile.writeToLog(String.Format("    deleted file: {0}", strFile));
                    }
                    catch (Exception ex)
                    {
                        //TODO: logfile warning
                        LogFile.writeToLog(String.Format("Error: could not delete file: {0}", strFile));
                    }
                }

                LogFile.writeToLog(String.Format("{0} file/s deleted", strFileArray.Length));


            }
            catch (Exception ex)
            {

                errorWhileProcessing = true;
                throw new ProcessingException(String.Format("Error encountered during delete operation: {0}" + ex.Message));
                // return false;
            }

            return true;
        }

        private static string[] GetFilesByExtension(string sourceFolder, string filters, System.IO.SearchOption searchOption)
        {
            return filters.Split(',').SelectMany(filter => System.IO.Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
            // List<String> lstArray = new List<string>();
            // foreach (string filter in filters.Split(','))
            // {
            //     foreach (FileInfo f in new DirectoryInfo(sourceFolder).GetFiles(filter))
            //     {
            //         lstArray.Add(f.FullName);
            //     }
            // }
            //return lstArray.ToArray();
        }

        protected void checkCreateFile(ref FileInfo objFile)
        {
            if (!objFile.Exists)
            {
                try
                {
                    LogFile.writeToLog(String.Format("file doesn't exist at path: {0}", objFile.FullName));
                    LogFile.writeToLog("Creating file...");
                    objFile.Create().Close();
                }
                catch (Exception ex)
                {
                    LogFile.writeToLog(String.Format("Error could not create file {0}", ex.Message));
                    errorWhileProcessing = true;
                    throw new ProcessingException(ex.Message);
                }
            }
        }

        protected bool validateLogFileOutput(string strOutputLog, string strSuccessCode, ref string strvalidationError)
        {
            string strFileContents = string.Empty;
            string strLogFileFullPath = Path.Combine(CURRENT_PATH, strOutputLog);
            if (File.Exists(strLogFileFullPath))
            {
                strFileContents = objHelper.ReadFile(strLogFileFullPath);
                if (strFileContents.Contains(strSuccessCode))
                {
                    LogFile.writeToLog("Success code: '" + strSuccessCode + "' found");
                    return true;
                }
                else
                {
                    strvalidationError = "Validation fail for {0}. Success code: '" + strSuccessCode + "' not found";
                    return false;
                }
            }
            else
            {
                strvalidationError = "Could not find logfile for {0} at path: " + strLogFileFullPath;
                return false;
            }

            return false;
        }

        protected ExecutableTypes getBatchType(string strName)
        {

            if (strName.ToLower().Contains("doc"))
            {
                return ExecutableTypes.DOC1;
            }
            else if (strName.ToLower().Contains("xharb"))
            {
                return ExecutableTypes.Xharbour;
            }
            else if (strName.ToLower().Contains("dataf"))
            {
                return ExecutableTypes.Dataflow;
            }
            else
            {
                return ExecutableTypes.NA;
            }
        }



    }//endClass

    public class ProcessFile
    {
        string name;
        string fullPath;
        string currentDirectory;
        string extension;
        FileInfo objFile;

        public ProcessFile(string filePath)
        {
            try
            {
                objFile = new FileInfo(filePath);
                name = objFile.Name;
                fullPath = objFile.FullName;
                currentDirectory = Path.GetDirectoryName(objFile.FullName);
                extension = objFile.Extension;
            }
            catch (Exception ex)
            {
                //TODO: logfile error
            }
        }

        //TODO : breakup functions with subclass functions
        public void copyFiles(string[] sourceFileArray, string destinationPath)
        {

            // if (objDestionationFile != null)
            string strAppendedText = string.Empty;
            int arrayLength = 0;
            //{
            try
            {
                foreach (string strFile in sourceFileArray)
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(strFile))
                        {
                            strAppendedText += reader.ReadToEnd();
                        }

                        LogFile.writeToLog(String.Format("    copy file: {0}", strFile));
                    }
                    catch (Exception ex)
                    {
                        //TODO: logfile warning
                        LogFile.writeToLog(String.Format("Error: could not copy file: {0}", strFile));
                    }

                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                arrayLength = sourceFileArray.Length;
                using (TextWriter tw = new StreamWriter(objFile.Open(FileMode.Truncate)))
                {
                    tw.Write(strAppendedText);
                }
            }
            catch (Exception ex)
            {
                //TODO: throw custom error
            }


            LogFile.writeToLog(String.Format("{0} file/s copied", Convert.ToString(arrayLength)));

        }

    }

    [Serializable]
    public class ProcessingException : Exception
    {
        HelperClass objHelper = new HelperClass();
        public string ErrorMessage
        {
            get { return base.Message.ToString(); }
        }

        public ProcessingException() : base() { }

        public ProcessingException(string message)
            : base(message)
        {
            //TODO: email output.
            LogFile.writeToLog(message, LogFile.ReportingAction.error);
            objHelper.WriteEventLog(message, EventLogEntryType.Error);
        }

        public ProcessingException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public ProcessingException(string message, Exception innerException)
            : base(message, innerException) { }

        public ProcessingException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        // protected ProcessingException(SerializationInfo info, StreamingContext context)
        //    : base(info, context) { }

    }




}
