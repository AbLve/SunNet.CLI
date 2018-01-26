using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.UpdateCluster.Models
{
    public class UpdateClusterModel
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string Description { get; set; }

        [DisplayName("Hyper Link")]
        public string HyperLink { get; set; }

        [Required]
        [DisplayName("Thumbnail Path")]
        public string ThumbnailPath { get; set; }

        public string ThumbnailName { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string ShowDate
        {
            get { return Date.ToString("MMMM dd,yyyy"); }
        }
        public string ShowTime
        {
            get { return Date.ToString("hh:mm tt"); }
            //get { return Date.GetDateTimeFormats('t')[0].ToString(); }
        }

        public string EditDate
        {
            get
            {
                return Date.ToString("MM/dd/yyyy hh:mm");
            }
        }
    }
}
