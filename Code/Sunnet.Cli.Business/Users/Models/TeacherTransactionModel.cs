using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Users.Models
{
    public class TeacherTransactionModel
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

        public DateTime CreatedOn { get; set; }
    }
}
