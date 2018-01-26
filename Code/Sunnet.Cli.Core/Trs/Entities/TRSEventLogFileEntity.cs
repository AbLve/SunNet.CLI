using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TRSEventLogFileEntity : EntityBase<int>
    {
       public int EventLogId {get;set;}
       public string FileName   {get;set;}
       public string FilePath   {get;set;}
       public bool IsDelete   {get;set;}
       public int CreatedBy  {get;set;}
       public int UpdatedBy  {get;set;}

       public virtual TRSEventLogEntity EventLog { get; set; }
    }
}
