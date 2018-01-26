using Sunnet.Cli.Core.DataProcess.Entities;
using Sunnet.Cli.Core.DataProcess.Models;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.DataProcess
{
    public interface IDataProcessContract
    {
        #region  属性
        
        IQueryable<DataGroupEntity> Groups { get; }

        IQueryable<DataCommunityEntity> Communities { get; }

        IQueryable<DataSchoolEntity> Schools { get; }

        IQueryable<DataStudentEntity> Students { get; }


        #endregion


        OperationResult InsertGroup(DataGroupEntity entity);

        OperationResult InsertCommunity(DataCommunityEntity entity);

        OperationResult InsertSchool(DataSchoolEntity entity);

        OperationResult InsertStudent(DataStudentEntity entity);

        OperationResult UpdateGroup(DataGroupEntity entity);

        bool DeleteGroup(int id);

        string ImportData(string sql);

        void Start(int id, int createdBy);

        List<RecordRemarkModel> GetRemarks(string sql);
    }
}
