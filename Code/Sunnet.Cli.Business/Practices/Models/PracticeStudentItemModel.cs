using Sunnet.Cli.Core.Cpalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		-
 * Description:		Please input class summary
 * Version History:	Created,-
 * Change:          Delete old class 2014-11-20
 * Create:          new Class
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Business.Practices.Models
{
    public class PracticeStudentItemModel
    {
        public int ID { get; set; }
        /// <summary>
        /// StudentMeasureEntity ID
        /// </summary>
        public int SMId { get; set; }

        public int ItemId { get; set; }

        public CpallsStatus Status { get; set; }
        /// <summary>
        /// 是否回答正确
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// 选 择的答案，有1,2,3这种格式
        /// </summary>
        public string SelectedAnswers { get; set; }

        /// <summary>
        /// 暂停时间
        /// </summary>
        public int PauseTime { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public decimal Goal { get; set; }

        /// <summary>
        /// item的分数 PA 要计算所有 answer 的分数
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 是否计分
        /// </summary>
        public bool Scored { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Response 填写的详细信息以JSON序列化格式保存
        /// </summary>
        /// Author : JackZhang
        /// Date   : 7/6/2015 09:21:27
        public string Details { get; set; }

        /// <summary>
        /// 表示是执行过的 Item, 因为有Item 跳转逻辑，有些Item 是不会执行的
        /// 已执行的为 True ，跳过的 Item 的为 False
        /// </summary>
        public bool Executed { get; set; }

        /// <summary>
        /// 记录上一个Item 的index 顺序。主要为 Item 上的后退按钮服务
        /// </summary>        
        public int LastItemIndex { get; set; }

        public int ResultIndex { get; set; }

        public ItemType Type { get; set; }
    }
}