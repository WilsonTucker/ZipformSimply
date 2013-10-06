using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Threading;

using Common;

namespace ZipformSimply
{
    public partial class SimplyService : ServiceBase
    {

        HelperClass objHelper = new HelperClass();
        System.Timers.Timer objTimedTask = new System.Timers.Timer();
        //private delegate string AsyncRunTask(string filePath);
        //private delegate string AsyncWriteToLog(string filePath);
        //AsyncWriteToLog writeLog; 
        //AsyncCallback writeCallBack; 

        public SimplyService()
        {
            
            InitializeComponent();

            objTimedTask.Interval = objHelper.parseDouble(ConfigurationManager.AppSettings["maxTimeForProcessedTask"]);
            objTimedTask.AutoReset = false;
            objTimedTask.Elapsed += objTimedTask_Elapsed;

            //writeCallBack = new AsyncCallback(logWriteCallBack);
            //writeLog = new AsyncWriteToLog(LogFile.writeLogAsync);
            ftpFolderWatch.Path = ConfigurationManager.AppSettings["fileWatchPath"];
            ftpFolderWatch.Filter = "*.csv";
            ftpFolderWatch.Renamed += ftpFolderWatch_Renamed;
            ftpFolderWatch.Deleted += ftpFolderWatch_Deleted;
            ftpFolderWatch.Changed += ftpFolderWatch_Changed;
            ftpFolderWatch.Created += ftpFolderWatch_Created;
            objHelper.WriteEventLog("Watching ftp directory...", EventLogEntryType.Information);
            ftpFolderWatch.EnableRaisingEvents = true;

        }

        /// <summary>
        /// Maximum processing time on task reached
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void objTimedTask_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
          
        }

        private void runTask(string filePath)
        {
            //IAsyncResult aResult = writeLog.BeginInvoke(filePath,writeCallBack,null);
           // private AsyncRunTask(filePath);
            ProcessTask objTask = new ProcessTask(filePath);
        }

        //protected void logWriteCallBack(IAsyncResult result)
        //{

        //}


        protected void ftpFolderWatch_Created(object sender, FileSystemEventArgs e)
        {
            LogFile.writeToLog(e.FullPath, LogFile.ReportingAction.newFile);
                //TODO: overload or refactor to allow for e.Fullpath which is now message
            runTask(e.FullPath);
            
        }

        protected void ftpFolderWatch_Changed(object sender, FileSystemEventArgs e)
        {
           
        }

        protected void ftpFolderWatch_Deleted(object sender, FileSystemEventArgs e)
        {
            objHelper.WriteEventLog(String.Format("Watched ftp file has been deleted. Path: {0}",e.FullPath), EventLogEntryType.Information); // TODO: remove 
            //TODO: send notifcation email
        }

        protected void ftpFolderWatch_Renamed(object sender, RenamedEventArgs e)
        {
            objHelper.WriteEventLog(String.Format("Watched ftp directory has been renamed! Path: {0}", e.FullPath), EventLogEntryType.Information);
            //TODO: send notifcation email
        }

        protected override void OnStart(string[] args)
        {
            objHelper.WriteEventLog("Started",EventLogEntryType.Information);
            
        }

        protected override void OnStop()
        {
            ftpFolderWatch.EnableRaisingEvents = false;
            ftpFolderWatch.Dispose();
            objTimedTask.Stop();
            objHelper.WriteEventLog("Stopped", EventLogEntryType.Information);
        }
    }
}
