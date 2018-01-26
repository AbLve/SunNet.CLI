using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManagement.App;
using StructureMap;
using Sunnet.Cli.Business.BUP;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Framework.Log;

namespace DataManagement.Business
{
    public class TaskManager
    {
        private readonly BUPTaskBusiness _taskBusiness;

        public TaskManager()
        {
            _taskBusiness = new BUPTaskBusiness();
        }

        public bool ExecuteTask()
        {
            try
            {
                LogManager.Info("Start the execution of all the waiting tasks");

                var allPendingTasks = _taskBusiness.GetPendingList();
                int taskCount = allPendingTasks.Count;

                LogManager.Info("There are " + taskCount + " sub tasks in this task.");

                for (int i = 0; i < taskCount; i++)
                {
                    LogManager.Info("#" + (i + 1) + " sub task(ID=" + allPendingTasks[i].ID + ")start");

                    ExecuteOneTask(allPendingTasks[i], i);

                    LogManager.Info("#" + (i + 1) + " sub task(ID=" + allPendingTasks[i].ID + ")end。");
                }

                LogManager.Info("Perform all the waiting tasks to complete"); 
            }
            catch (Exception e)
            {
                LogManager.Info("TaskManager.ExecuteTask() exception：" + e.Message);
                return false;
            }
            return true;
        }

        private bool ExecuteOneTask(BUPTaskEntity taskEntity, long taskIndex)
        {
            try
            {
                BUPTaskBusiness.Start(taskEntity.ID, taskEntity.CreatedBy);
            }
            catch (Exception e)
            {
                LogManager.Info("#" + (taskIndex + 1) + " sub task(" + "ID=" + taskEntity.ID + " OriginFileName=" + taskEntity.OriginFileName + " CreatedBy=" + taskEntity.CreatedBy + ")exception：" + e.Message);
                return false;
            }

            return true;
        }
    }
}
