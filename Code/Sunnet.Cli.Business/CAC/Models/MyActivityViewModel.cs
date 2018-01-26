using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web;
using Sunnet.Framework;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.CAC.Models
{
    public class MyActivityViewModel
    {
        #region Ctor.

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MyActivityViewModel() { }

        /// <summary>
        /// 传参构造函数
        /// </summary>
        /// <param name="id">主键，唯一标识</param>
        /// <param name="collectionType">集合类型</param>
        /// <param name="activityName">活动名称</param>
        /// <param name="domain">域名</param>
        /// <param name="subDomain">子域名</param>
        /// <param name="note">注意</param>
        /// <param name="url">链接URL</param>
        public MyActivityViewModel(int id, string collectionType, string activityName, string domain, string subDomain, string note, string url)
        {
            this.ID = id;
            this.CollectionType = collectionType;
            this.ActivityName = activityName;
            this.Domain = domain;
            this.SubDomain = subDomain;
            this.Note = note;
            this.Url = url;
        }

        #endregion

        #region Property

        /// <summary>
        /// 主键，唯一标识
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 集合类型
        /// </summary>
        [DisplayName("Collection Type")]
        public string CollectionType { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        [DisplayName("Activity Name")]
        public string ActivityName { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        [DisplayName("Domain")]
        public string Domain { get; set; }
        /// <summary>
        /// 子域名
        /// </summary>
        [DisplayName("SubDomain")]
        public string SubDomain { get; set; }
        /// <summary>
        /// 注意
        /// </summary>
        [DisplayName("Note")]
        public string Note { get; set; }
        /// <summary>
        /// 链接URL
        /// </summary>
        public string Url { get; set; }

        public string Objective { get; set; }

        public string ObjectiveHtml
        {
            get
            {
                return HttpUtility.HtmlDecode(Objective);
            }
        }

        public string AgeGroup { get; set; }

        #endregion
    }
}
