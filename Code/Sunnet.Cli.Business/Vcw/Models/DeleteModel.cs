using Sunnet.Cli.Core.Vcw.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/11/26 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/11/26 12:15:10
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Vcw.Models
{
    public class DeleteAssignmentModel
    {
        public AssignmentEntity Assignment { get; set; }

        public ICollection<AssignmentContentEntity> AssignmentContents { get; set; }

        public ICollection<AssignmentContextEntity> AssignmentContexts { get; set; }

        public ICollection<AssignmentFileEntity> AssignmentFiles { get; set; }

        public ICollection<AssignmentUploadTypeEntity> AssignmentUploadTypes { get; set; }

        public ICollection<AssignmentStrategyEntity> AssignmentStrategies { get; set; }

        public ICollection<AssignmentReportEntity> AssignmentReports { get; set; }

    }

    public class DeleteFileModel
    {
        public Vcw_FileEntity File { get; set; }

        public ICollection<FileSelectionEntity> FileSelections { get; set; }

        public ICollection<FileSharedEntity> FileShareds { get; set; }

        public ICollection<FileContentEntity> FileContents { get; set; }

        public ICollection<FileStrategyEntity> FileStrategies { get; set; }
    }
}
