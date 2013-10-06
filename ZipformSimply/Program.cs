using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace ZipformSimply
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new SimplyService() 
            };

            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            ServiceBase.Run(ServicesToRun);
        }

        //protected static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //   //LOG ERROR MESSAGE
        //}
    }
}
