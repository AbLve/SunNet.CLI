using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.Permission
{
    public enum Authority : int
    {
        /// <summary>
        /// 浏览权限
        /// </summary>
        [Description("Index")]
        Index = 1,

        /// <summary>
        /// 添加权限
        /// </summary>
        [Description("Add")]
        Add = 2,

        /// <summary>
        /// 编辑权限
        /// </summary>
        [Description("Edit")]
        Edit = 4,

        /// <summary>
        /// 删除权限
        /// </summary>
        [Description("Delete")]
        Delete = 8,


        /// <summary>
        /// 查看详细
        /// </summary>
        [Description("View")]
        View = 32,

        /// <summary>
        /// 分配功能
        /// </summary>
        [Description("Assign")]
        Assign = 64,

        /// <summary>
        /// 所有权限
        /// </summary>
        [Description("All")]
        All = 128,

        /// <summary>
        /// 
        /// </summary>
        [Description("Custom Notifications")]
        Notes = 256,

        /// <summary>
        /// 邀请家长
        /// </summary>
        [Description("Parent Invitation")]
        ParentInvitation = 512,

        /// <summary>
        /// 会计业务
        /// </summary>
        [Description("Transaction")]
        Transaction = 1024,

        /// <summary>
        /// Assessment Equipment
        /// </summary>
        [Description("Assessment Equipment")]
        AssessmentEquipment = 2048,

        /// <summary>
        /// Offline
        /// </summary>
        [Description("Offline")]
        Offline = 4096,

        /// <summary>
        /// Bes
        /// </summary>
        [Description("Bes")]
        Bes = 8192,

        /// <summary>
        /// Bes
        /// </summary>
        [Description("Assessment Practice Area")]
        AssessmentPracticeArea = 16384
    }
}
