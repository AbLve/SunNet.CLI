using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 1:01:46
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 1:01:46
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade.Entities
{
    /// <summary>
    /// ������Links��ʵ��
    /// </summary>
    public interface IAdeLinkProperties
    {
    }

    public class AdeLinkEntity : EntityBase<int>
    {
        /// <summary>
        /// ��������.
        /// </summary>
        [StringLength(100)]
        [Required]
        public string HostType { get; set; }
        /// <summary>
        /// ����ID.
        /// </summary>
        public int HostId { get; set; }

        /// <summary>
        /// ��������(Activity|Course|File...).
        /// </summary>
        public byte LinkType { get; set; }

        /// <summary>
        /// ���ӵ�ַ.
        /// </summary>
        [StringLength(500)]
        [Required]
        public string Link { get; set; }

        /// <summary>
        /// Display Text
        /// </summary>
        [StringLength(500)]
        public string DisplayText { get; set; }

        public EntityStatus Status { get; set; }

        public bool MeasureWave1 { get; set; }
        public bool MeasureWave2 { get; set; }
        public bool MeasureWave3 { get; set; }
        public bool StudentWave1 { get; set; }
        public bool StudentWave2 { get; set; }
        public bool StudentWave3 { get; set; }
    }
}
