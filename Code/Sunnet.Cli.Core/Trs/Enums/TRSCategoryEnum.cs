using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs
{
    //修改此枚举时，需要修改Module_TRS_Offline.js   Category
    public enum TRSCategoryEnum : byte
    {
        /// <summary>
        /// Category 1
        /// </summary>
        [Display(Name = "Category 1")]
        [Description("Director And Staff Qualifications And Training")]
        Category1 = 1,

        /// <summary>
        /// Category 2
        /// </summary>
        [Display(Name = "Category 2")]
        [Description("Caregiver-Child Interactions")]
        Category2 = 2,

        /// <summary>
        /// Category 3
        /// </summary>
        [Display(Name = "Category 3")]
        [Description("Curriculum")]
        Category3 = 3,

        /// <summary>
        /// Category 4
        /// </summary>
        [Display(Name = "Category 4")]
        [Description("Nutrition, Indoor/Outdoor Environment")]
        Category4 = 4,

        /// <summary>
        /// Category 5
        /// </summary>
        [Display(Name = "Category 5")]
        [Description("Parent Education Involvement")]
        Category5 = 5

    }

    public enum TRSSubCategoryEnum : byte
    {
        /// <summary>
        /// Category1
        /// </summary>
        [Description("Director Qualifications")]
        Director_Qualifications = 1,

        [Description("Caregiver Qualifications")]
        Caregiver_Qualifications = 2,

        /// <summary>
        /// Category2
        /// </summary>
        [Description("Staff Ratios")]
        Staff_Ratios = 11,

        [Description("Warm and Responsive Style")]
        Warm_and_Responsive_Style = 12,

        [Description("Language Faciliation and Support")]
        Language_Faciliation_and_Support = 13,

        [Description("Play-Based Interactions and Guidance")]
        Play_Based_Interactions_and_Guidance = 14,

        [Description("Support for Children's Regulation")]
        Support_for_Childrens_Regulation = 15,

        [Description("Group Size - Non-Mixed Age Classroom")]
        Group_Size_Non_Mixed_Age_Classroom = 16,

        [Description("Group Size - Mixed Age Classroom")]
        Group_Size_Mixed_Age_Classroom = 17,


        /// <summary>
        /// Category3
        /// </summary>
        [Description("Lesson Plans & Curriculum")]
        Lesson_Plans_Curriculum = 21,

        [Description("Planning for Special Needs & Respecting Diversity")]
        Planning_for_Special_Needs_Respecting_Diversity = 22,

        [Description("Instructional Formats and Approaches to Learning")]
        Instructional_Formats_and_Approaches_to_Learning = 23,

        /// <summary>
        /// Category4
        /// </summary>
        [Description("Nutrition")]
        Nutrition = 31,

        [Description("Indoor Learning Environment")]
        Indoor_Learning_Environment = 32,

        [Description("Outdoor Learning Environment")]
        Outdoor_Learning_Environment = 33,

        /// <summary>
        /// Category5
        /// </summary>
        [Description("Parent Education")]
        Parent_Education = 41,

        [Description("Parent Involvement")]
        Parent_Involvement = 42
    }
}
