using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManagement.Business;

namespace DataManagement
{
    public static class BusinessFacade
    {
        public static SingleRosterManager GetRosterManager()
        {
            return new SingleRosterManager();
        }

        public static TaskManager GetTaskManager()
        {
            return new TaskManager();
        }

        
    }
}
