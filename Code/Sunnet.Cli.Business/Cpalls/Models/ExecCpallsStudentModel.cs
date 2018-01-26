using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/11 7:48:34
 * Description:		Please input class summary
 * Version History:	Created,2014/9/11 7:48:34
 * 
 * 
 **************************************************************************/
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using LinqKit;
using Sunnet.Cli.Business.Schools.Models;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class ExecCpallsStudentModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }

        private string _schoolName;

        public string SchoolName
        {
            get
            {
                if (string.IsNullOrEmpty(_schoolName) && Schools != null)
                {
                    _schoolName = string.Join(", ", Schools.Select(x => x.Name));
                }
                return _schoolName;
            }
            set { _schoolName = value; }
        }

        public string CommunitiesText
        {
            get
            {
                if (Schools == null) return string.Empty;
                var communities = new List<string>();
                Schools.ForEach(x => communities.AddRange(x.Communities));
                return string.Join(", ", communities.Distinct());
            }
        }

        public IEnumerable<BasicSchoolModel> Schools { get; set; }

    }
}
