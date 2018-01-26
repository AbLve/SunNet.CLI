using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/10/21 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/10/21 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Interfaces;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Core;

namespace Sunnet.Cli.Impl.Vcw
{
    public class AssignmentRpst : EFRepositoryBase<AssignmentEntity, Int32>, IAssignmentRpst
    {
        public AssignmentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        /// <summary>
        /// 更改Assignment状态
        /// </summary>
        /// <param name="method"></param>
        public void ChangeStatus(ChangeStatusEnum method, int assignmentId, FileStatus status)
        {
            StringBuilder sb = new StringBuilder();
            switch (method)
            {
                case ChangeStatusEnum.AddFile://上传File                                     
                    sb.AppendFormat(@"
                                if ((select count(1) from Files where AssignmentId={0} and IsDelete=0 and [Status]=2)>0)
	                                begin
	                                   update Assignments set Status=3 where ID={0} 
	                                end
                                else
                                    begin
	                                     if ((select count(1) from Files where AssignmentId={0} and IsDelete=0 and [Status]=1)>0)
                                             begin
                                               update Assignments set Status=2 where ID={0}
                                             end
                                         else
                                             begin
		                                        update Assignments set Status=4 where ID={0} 
                                             end
                                    end", assignmentId);
                    break;
                case ChangeStatusEnum.DeleteFile://删除File时更新Assignment状态
                    sb.AppendFormat(@"
                    if ((select count(1) from Files where AssignmentId={0} and IsDelete=0)>0) 
                        begin 
                            if ((select count(1) from Files where AssignmentId={0} and IsDelete=0 and [Status]=2)>0)
	                            begin
	                               update Assignments set Status=3 where ID={0} 
	                            end
                            else
                                begin
	                                 if ((select count(1) from Files where AssignmentId={0} and IsDelete=0 and [Status]=1)>0)
                                         begin
                                           update Assignments set Status=2 where ID={0}
                                         end
                                     else
                                         begin
		                                    update Assignments set Status=4 where ID={0} 
                                         end
                                end
                        end
                    else
                        begin
                           update Assignments set Status=1 where ID={0} 
                        end", assignmentId);
                    break;
                case ChangeStatusEnum.UpdateFile: //更改File状态时更新Assignment状态
                    if (status == FileStatus.Completed)
                    {
                        sb.AppendFormat(@"update Assignments set Status=3 where ID={0}", assignmentId);
                    }
                    else
                    {
                        sb.AppendFormat(@"
                        if ((select count(1) from Files where AssignmentId={0} and IsDelete=0 and [Status]=2)>0)
	                       begin
	                               update Assignments set Status=3 where ID={0} 
	                       end
                        else
                           begin
	                           if ((select count(1) from Files where AssignmentId={0} and IsDelete=0 and [Status]=1)>0)
                                  begin
                                    update Assignments set Status=2 where ID={0}
                                  end
                               else
                                  begin
		                            update Assignments set Status=4 where ID={0} 
                                  end
                            end
                        ", assignmentId);
                    }
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(sb.ToString().Trim()))
            {
                VCWUnitOfWorkContext context = this.UnitOfWork as VCWUnitOfWorkContext;
                context.DbContext.Database.ExecuteSqlCommand(sb.ToString());
            }
        }

        public void ChangeStatus(List<int> assignmentIds)
        {
            string ids = "(";
            foreach (int item in assignmentIds)
            {
                ids += item.ToString() + ",";
            }
            if (ids.EndsWith(","))
            {
                ids = ids.Remove(ids.LastIndexOf(','));
                ids += ")";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"update Assignments set Status=1 where ID in {0}", ids);
            VCWUnitOfWorkContext context = this.UnitOfWork as VCWUnitOfWorkContext;
            context.DbContext.Database.ExecuteSqlCommand(sb.ToString());
        }
    }
}
