using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/19 19:20:08
 * Description:		Please input class summary
 * Version History:	Created,2014/11/19 19:20:08
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class StudentAssessmentModel
    {
        #region StudentAssessmentEntity Members

        public int ID { get; set; }

        public int StudentId { get; set; }

        public int AssessmentId { get; set; }

        public CpallsStatus Status { get; set; }

        public string SchoolYear
        {
            get
            {
                if (_schoolYear < 1)
                    _schoolYear = CommonAgent.Year;
                return _schoolYear.ToString();
            }
            set { _schoolYear = int.Parse("20" + value.Substring(0, 2)); }
        }

        public Wave Wave { get; set; }

        public decimal TotalGoal { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        #endregion

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        private int _schoolYear;

        public int Age
        {
            get
            {
                int month; int day;
                CommonAgent.CalculatingAge(_schoolYear, BirthDate, out month, out day);
                return month / 12;
            }
        }

        private IEnumerable<StudentMeasureModel> _measureList;

        [JsonIgnore]
        public IEnumerable<StudentMeasureModel> Measures
        {
            get { return _measureList ?? (_measureList = new List<StudentMeasureModel>()); }
            set { _measureList = value; }
        }

        public Dictionary<int, StudentMeasureModel> Measure
        {
            get
            {
                if (Measures == null || !Measures.Any()) return null;
                Dictionary<int, StudentMeasureModel> dic = new Dictionary<int, StudentMeasureModel>();
                foreach (StudentMeasureModel item in Measures)
                {
                    if (item == null)
                        continue;
                    if (dic.ContainsKey(item.MeasureId))
                        dic[item.MeasureId] = item;
                    else
                        dic.Add(item.MeasureId, item);
                }
                return dic;
            }
        }


    }
}
