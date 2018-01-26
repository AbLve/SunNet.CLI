using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.TRSClasses
{
    public interface ITRSClassContract
    {
        #region Class
        IQueryable<TRSClassEntity> TRSClasses { get; }

        OperationResult InsertTRSClass(TRSClassEntity entity, bool isSave = true);

        OperationResult DeleteTRSClass(int id);

        OperationResult UpdateTRSClass(TRSClassEntity entity, bool isSave = true);

        TRSClassEntity GetTRSClass(int id);

        OperationResult UpdateTRSClassPlayground(int playgroundId = 0, int[] classIds = null);
        #endregion

        #region TRS
        IQueryable<CHChildrenEntity> CHChildrens { get; }

        IQueryable<CHChildrenResultEntity> CHChildrenResults { get; }

        OperationResult InsertResult(List<CHChildrenResultEntity> list, bool isSave = true);

        OperationResult DeleteResult(int classId, bool isSave = true);

        OperationResult DeleteResultBySchoolId(int schoolId, bool isSave = true);
        #endregion
    }
}
