using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/06/03
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/06/03
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.StatusTracking;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.StatusTracking.Models;
using System.Linq.Expressions;
using Sunnet.Framework.Core.Extensions;
using LinqKit;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Framework;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.StatusTracking
{
    public class StatusTrackingBusiness
    {
        private readonly IStatusTrackingContract _statusTrackingService;
        private EFUnitOfWorkContext Unit = null;

        public StatusTrackingBusiness(EFUnitOfWorkContext unit = null)
        {
            Unit = unit;
            _statusTrackingService = DomainFacade.CreateStatusTrackingService(unit);
        }

        public List<StatusTrackingEntity> GetTrackings()
        {
            return _statusTrackingService.StatusTrackings.ToList();
        }

        public OperationResult UpdateTracking(StatusTrackingEntity StatusStracking)
        {
            return _statusTrackingService.UpdateStatusTracking(StatusStracking);
        }

        public StatusTrackingEntity GetTracking(int id)
        {
            return _statusTrackingService.GetStatusTracking(id);
        }

        public StatusTrackingEntity GetTrackingByUrl(string url)
        {
            return _statusTrackingService.StatusTrackings.Where(r => r.ProcessAddress == url).OrderByDescending(o => o.UpdatedOn).FirstOrDefault();
        }

        public StatusTrackingEntity GetExistingTracking(int requestorId, int supposedApproveId, int communityId,
            int schoolId)
        {
            return
                _statusTrackingService.StatusTrackings.Where(
                    r =>
                        r.RequestorId == requestorId && r.SupposedApproverId == supposedApproveId &&
                        r.CommunityId == communityId && r.SchoolId == schoolId).FirstOrDefault();
        }

        public StatusTrackingEntity GetExistingTracking(int communityId, int schoolId,StatusType type)
        {
            return _statusTrackingService.StatusTrackings.OrderByDescending(o => o.UpdatedOn).FirstOrDefault(r => r.CommunityId == communityId && r.SchoolId == schoolId && r.Type == type);
        }

        public OperationResult AddTracking(StatusTrackingEntity entity)
        {
            return _statusTrackingService.AddStatusTracking(entity);
        }

        public StatusTrackingModel GetStatusTrackingModel(int id)
        {
            return _statusTrackingService.StatusTrackings.Where(r => r.ID == id)
                .Select(r => new StatusTrackingModel
                {
                    ID = r.ID,
                    ApproverId = r.ApproverId,
                    ApproverName = r.UserInfo_Approver.FirstName + " " + r.UserInfo_Approver.LastName,
                    RequestorId = r.RequestorId,
                    RequestorName = r.UserInfo_Requestor.FirstName + " " + r.UserInfo_Requestor.LastName,
                    RequestorEmail = r.UserInfo_Requestor.PrimaryEmailAddress,
                    SupposedApproverId = r.SupposedApproverId,
                    Status = r.Status,
                    RequestTime = r.RequestTime,
                    ApprovedTime = r.ApprovedTime,
                    DeniedTime = r.DeniedTime,
                    ExpiredTime = r.ExpiredTime,
                    ResendTime = r.ResendTime,
                    ResendNumber = r.ResendNumber,
                    SApproverFirstName = r.UserInfo_SupposedApprover.FirstName,
                    SApproverLastName = r.UserInfo_SupposedApprover.LastName,
                    SApproverPNumber = r.UserInfo_SupposedApprover.PrimaryPhoneNumber,
                    SApproverPType = r.UserInfo_SupposedApprover.PrimaryNumberType,
                    SApproverSNumber = r.UserInfo_SupposedApprover.SecondaryPhoneNumber,
                    SApproverSType = r.UserInfo_SupposedApprover.SecondaryNumberType,
                    SApproverFaxNumber = r.UserInfo_SupposedApprover.FaxNumber,
                    SApproverPEAddress = r.UserInfo_SupposedApprover.PrimaryEmailAddress,
                    SApproverSEAddress = r.UserInfo_SupposedApprover.SecondaryEmailAddress
                }).FirstOrDefault();
        }

        public List<StatusTrackingModel> GetTrackingList(Expression<Func<StatusTrackingEntity, bool>> trackingContition,
            UserBaseEntity user, int parentId, string sort, string order, int first, int count, out int total)
        {
            var query = _statusTrackingService.StatusTrackings.AsExpandable().Where(trackingContition)
                .Select(r => new StatusTrackingModel
                {
                    ID = r.ID,
                    ApproverId = r.ApproverId,
                    ApproverName = r.UserInfo_Approver.FirstName + " " + r.UserInfo_Approver.LastName,
                    RequestorId = r.RequestorId,
                    RequestorName = r.UserInfo_Requestor.FirstName + " " + r.UserInfo_Requestor.LastName,
                    RequestorEmail = r.UserInfo_Requestor.PrimaryEmailAddress,
                    SupposedApproverId = r.SupposedApproverId,
                    Status = r.Status,
                    RequestTime = r.RequestTime,
                    ApprovedTime = r.ApprovedTime,
                    DeniedTime = r.DeniedTime,
                    ExpiredTime = r.ExpiredTime,
                    ResendTime = r.ResendTime,
                    ResendNumber = r.ResendNumber,
                    SApproverFirstName = r.UserInfo_SupposedApprover.FirstName,
                    SApproverLastName = r.UserInfo_SupposedApprover.LastName,
                    CommunityId = r.CommunityId,
                    SchoolId = r.SchoolId,
                    Type = r.Type,
                    ProcessAddress = r.ProcessAddress,
                    IfCanOperate = (r.UserInfo_SupposedApprover.Role != user.Role
                    && r.RequestorId != user.ID && r.RequestorId != parentId || r.SupposedApproverId == user.ID)
                    && r.Status == StatusEnum.Pending && DateTime.Now < r.ExpiredTime,
                    IfCanEmail = (r.RequestorId == user.ID || r.RequestorId == parentId || user.Role < Role.Statewide)
                    && r.Status != StatusEnum.Accepted
                });
            total = query.Count();
            return query.OrderBy(sort, order).Skip(first).Take(count).ToList();
        }

        public StatusTrackingModel GetStatusTrackingModelById(int id)
        {
            return _statusTrackingService.StatusTrackings.Where(r => r.ID == id)
                .Select(r => new StatusTrackingModel()
            {
                SupposedUserCommunitySchools = r.UserInfo_SupposedApprover.UserCommunitySchools,
                UserInfo_Requestor = r.UserInfo_Requestor,
                UserInfo_SupposedApprover = r.UserInfo_SupposedApprover
            }).FirstOrDefault();
        }

        /// <summary>
        /// Approve
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <param name="approverId">Approve User ID</param>
        /// <param name="updateUserId">Update User ID</param>
        /// <returns></returns>
        public OperationResult Approve(int id, int updateUserId)
        {
            StatusTrackingEntity statusEntity = GetTracking(id);
            if (statusEntity != null)
            {
                statusEntity.Status = StatusEnum.Accepted;
                statusEntity.ApprovedTime = DateTime.Now;
                statusEntity.ApproverId = updateUserId;
                statusEntity.UpdatedBy = updateUserId;
                statusEntity.UpdatedOn = DateTime.Now;
                return UpdateTracking(statusEntity);
            }
            else
            {
                return new OperationResult(OperationResultType.Error);
            }
        }

        /// <summary>
        /// Deny
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <param name="updateUserId">Update User ID</param>
        /// <returns></returns>
        public OperationResult Deny(int id, int updateUserId)
        {
            StatusTrackingEntity statusEntity = GetTracking(id);
            if (statusEntity != null)
            {
                statusEntity.Status = StatusEnum.Denied;
                statusEntity.DeniedTime = DateTime.Now;
                statusEntity.ApproverId = updateUserId;
                statusEntity.UpdatedBy = updateUserId;
                statusEntity.UpdatedOn = DateTime.Now;
                return UpdateTracking(statusEntity);
            }
            else
            {
                return new OperationResult(OperationResultType.Error);
            }
        }

        /// <summary>
        /// Resend
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <param name="updateUserId">Update User ID</param>
        /// <returns></returns>
        public OperationResult Resend(int id, int updateUserId)
        {
            StatusTrackingEntity statusEntity = GetTracking(id);
            if (statusEntity != null)
            {
                statusEntity.Status = StatusEnum.Pending;
                statusEntity.ExpiredTime = DateTime.Now.AddDays(SFConfig.ExpirationTime);
                statusEntity.ResendTime = DateTime.Now;
                statusEntity.RequestTime = DateTime.Now;
                statusEntity.ResendNumber += 1;
                statusEntity.UpdatedBy = updateUserId;
                statusEntity.UpdatedOn = DateTime.Now;
                return UpdateTracking(statusEntity);
            }
            else
            {
                return new OperationResult(OperationResultType.Error);
            }
        }
    }
}
