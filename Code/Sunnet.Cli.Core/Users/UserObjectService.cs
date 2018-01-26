using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.Users
{
    internal partial class UserService
    {
        #region User CommunitySchool Relations

        public OperationResult InsertUserCommunitySchoolRelations(IList<UserComSchRelationEntity> list)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                UserComSchRelationRpst.Insert(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DelUserCommunitySchoolRelations(IList<UserComSchRelationEntity> list)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                UserComSchRelationRpst.Delete(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region User Class Relations

        public OperationResult InsertUserClassRelations(IList<UserClassRelationEntity> list)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                UserClassRelationRpst.Insert(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DelUserClassRelations(IList<UserClassRelationEntity> list)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                UserClassRelationRpst.Delete(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion
    }
}
