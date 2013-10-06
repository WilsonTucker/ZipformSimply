using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipformSimply
{
    class ServiceStatus
    {

     //   public static AvailableStatus ServiceStatus = AvailableStatus.Idle;

        public enum AvailableStatus
        {
            Idle,
            ThreadWait,
            Fail
        }

        //public static AvailableStatus GetServiceStatus()
        //{
        //    return ServiceStatus;
        //}

    }
}
