
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:22:23
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:23
 * 更改属性名必须注意客户端JS里面有写死的属性名
 * 
 **************************************************************************/


namespace Sunnet.Cli.Core.Cpalls.Entities
{
    public class StudentItemEntity : EntityBase<int>
    {
        public StudentItemEntity()
        {
            Details = string.Empty;
            Executed = true;
        }
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
        public decimal? Goal { get; set; }

        /// <summary>
        /// item的分数 PA 要计算所有 answer 的分数
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 是否计分
        /// </summary>
        public bool Scored { get; set; }

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

        /// <summary>
        /// 生成结果页面中显示的索引（Branching Skip时需要特殊处理）
        /// </summary>
        public int ResultIndex { get; set; }

        public virtual StudentMeasureEntity Measure { get; set; }

        public virtual ItemBaseEntity Item { get; set; }
    }
}
