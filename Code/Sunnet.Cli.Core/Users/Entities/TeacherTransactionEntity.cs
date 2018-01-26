using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class TeacherTransactionEntity : EntityBase<int>
    {
        public int TeacherId { get; set; }

        [DisplayName("Transaction Type")]
        public TransactionType TransactionType { get; set; }

        [DisplayName("Amount")]
        public decimal Amount { get; set; }

        [DisplayName("Sessions Attended")]
        public int TISessionsAttended { get; set; }

        [DisplayName("Total Sessions")]
        public int TITotalSessions { get; set; }

        [DisplayName("CLI Funding")]
        public int TICLIFundingId { get; set; }

        [Required]
        [DisplayName("Funding Year")]
        public string FundingYear { get; set; }

        public virtual TeacherEntity Teacher { get; set; }
    }
}
