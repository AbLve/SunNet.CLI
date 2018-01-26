using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sunnet.Cli.Business.Common
{
    /// <summary>
    /// 公共 方法/属性 代理类
    /// </summary>
    public static class CommonAgent
    {
        /// <summary>
        /// It should be 8, Aguest
        /// To be changed back 08/1/2017
        /// </summary>
        static int _yearSeparate = 8;
        static string _schoolYear = string.Empty;
        /// <summary>
        /// 获取当前 Shool Year  04-05
        /// </summary>
        public static string SchoolYear
        {
            get
            {
                if (_schoolYear == string.Empty)
                {
                    if (DateTime.Now.Month >= _yearSeparate)
                        return string.Format("{0}-{1}", DateTime.Now.Year.ToString().Substring(2, 2), DateTime.Now.AddYears(1).Year.ToString().Substring(2, 2));
                    else
                        return string.Format("{0}-{1}", DateTime.Now.AddYears(-1).Year.ToString().Substring(2, 2), DateTime.Now.Year.ToString().Substring(2, 2));
                }
                return _schoolYear;
            }
        }

        /// <summary>
        /// 判断是否当前年 根据 学年的分隔月（一般是8月）来取年份 ; 如 2014年6月，则为2013 ; 如 2014年9月，则为2014
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static bool IsCurrentSchoolYear(int year)
        {
            return year == Year;
        }

        /// <summary>
        /// 获取schoolYear 中较大的那个年份
        /// David:Safe for School year Rollover
        /// </summary>
        public static string SchoolYearMax
        {
            get
            {
                if (DateTime.Now.Month < _yearSeparate)
                    return DateTime.Now.Year.ToString("0000");
                else
                {
                    return DateTime.Now.AddYears(1).Year.ToString("0000");
                }
            }
        }
        static int _year = 0;
        /// <summary>
        /// 当前年度  根据 学年的分隔月（一般是8月）来取年份 ; 如 2014年6月，则为2013 ; 如 2014年9月，则为2014
        /// </summary>
        public static int Year
        {
            get
            {
                if (_year == 0)
                {
                    _year = DateTime.Now.Year;
                    if (DateTime.Now.Month < _yearSeparate)
                        _year = _year - 1;
                }
                return _year;
            }
        }

        /// <summary>
        /// 系统最小日期
        /// </summary>
        public static DateTime MinDate
        {
            get { return CommonHelper.MinDate; }
        }

        /// <summary>
        /// TRS 最小日期: 2015-9-1
        /// </summary>
        /// Author : JackZhang
        /// Date   : 6/2/2015 12:00:08
        public static DateTime TrsMinDate
        {
            get
            {
                return new DateTime(2016, 9, 1);
            }
        }

        /// <summary>
        /// Gets the TRS receritification date.
        /// </summary>
        /// <returns></returns>
        /// Author : JackZhang
        /// Date   : 6/2/2015 12:00:04
        public static DateTime GetTrsReceritificationDate()
        {
            var date = DateTime.Now;
            if (date < TrsMinDate)
                date = TrsMinDate;
            return date.AddYears(3);
        }

        /// <summary>
        /// 确保日期能够存入数据库.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="minDate">The minimum date.</param>
        /// <returns></returns>
        public static DateTime EnsureMinDate(this DateTime date, string minDate = "")
        {
            DateTime md;
            if (!DateTime.TryParse(minDate, out md))
            {
                md = MinDate;
            }
            if (date <= md)
            {
                return md;
            }
            return date;
        }



        public static string GetInformation(string key)
        {
            var r = ResourceHelper.GetRM().RmInformation.GetObject(key);
            if (r != null)
                return r.ToString();
            return "";
        }

        /// <summary>
        /// 年度下拉框选项
        /// </summary>
        /// <returns></returns>
        public static List<SelectItemModel> GetYears()
        {
            List<SelectItemModel> list = new List<SelectItemModel>();
            //for (int i = 5; i > 0; i--)
            //{
            //    int year = DateTime.Now.AddYears(-i).Year;
            //    list.Add(new SelectItemModel() { ID = year, Name = string.Format("{0}-{1}", year.ToString().Substring(2, 2), (year + 1).ToString().Substring(2, 2)) });
            //}
            //if (DateTime.Now.Month >= YearSeparate)
            //    list.Add(new SelectItemModel()
            //    {
            //        ID = DateTime.Now.Year
            //        ,
            //        Name = string.Format("{0}-{1}", DateTime.Now.Year.ToString().Substring(2, 2), DateTime.Now.AddYears(1).Year.ToString().Substring(2, 2))
            //    });
            if (DateTime.Now.Month >= _yearSeparate)
            {
                list.Add(new SelectItemModel()
                {
                    ID = DateTime.Now.Year,
                    Name = string.Format("{0}-{1}", DateTime.Now.Year.ToString().Substring(2, 2), DateTime.Now.AddYears(1).Year.ToString().Substring(2, 2))
                });
            }
            else
            {
                list.Add(new SelectItemModel()
                {
                    ID = DateTime.Now.Year - 1,
                    Name = string.Format("{0}-{1}", DateTime.Now.AddYears(-1).Year.ToString().Substring(2, 2), DateTime.Now.Year.ToString().Substring(2, 2))
                });
            }

            return list;
        }

        public static List<SelectItemModel> GetYearsForCOT()
        {
            List<SelectItemModel> list = new List<SelectItemModel>();
            for (int i = 5; i > 0; i--)
            {
                int year = DateTime.Now.AddYears(-i).Year;
                list.Add(new SelectItemModel() { ID = year, Name = string.Format("{0}-{1}", year.ToString().Substring(2, 2), (year + 1).ToString().Substring(2, 2)) });
            }
            if (DateTime.Now.Month >= _yearSeparate)
                list.Add(new SelectItemModel()
                {
                    ID = DateTime.Now.Year
                    ,
                    Name = string.Format("{0}-{1}", DateTime.Now.Year.ToString().Substring(2, 2), DateTime.Now.AddYears(1).Year.ToString().Substring(2, 2))
                });
            return list;
        }

        public static List<SelectItemModel> GetWave()
        {
            List<SelectItemModel> list = new List<SelectItemModel>();
            list.Add(new SelectItemModel { ID = 1, Name = "BOY" });
            list.Add(new SelectItemModel { ID = 2, Name = "MOY" });
            list.Add(new SelectItemModel { ID = 3, Name = "EOY" });
            return list;
        }


        /// <summary>
        /// 当前 Wave David 01/15/2015
        /// </summary>
        public static Wave CurrentWave
        {
            get
            {
                if (DateTime.Now >= DateTime.Parse(string.Format("1/15/{0}", DateTime.Now.Year)))
                {
                    if (DateTime.Now < DateTime.Parse(string.Format("3/15/{0}", DateTime.Now.Year)))
                        return Wave.MOY;
                    else
                    {
                        if (DateTime.Now < DateTime.Parse(string.Format("8/1/{0}", DateTime.Now.Year)))
                            return Wave.EOY;
                        else
                            return Wave.BOY;
                    }
                }
                else
                    return Wave.BOY;
            }
        }

        /// <summary>
        /// Age 年龄计算 ,返回总月数与额外天数
        /// </summary>
        /// <param name="currentDate">当前时间</param>
        /// <param name="dateofBirth">生日</param>
        /// <param name="finalMonth">Age中的月数</param>
        /// <param name="finalDay">Age 中的天数</param>
        /// <returns>计算是否成功</returns>
        public static bool CalculatingAge(int year, DateTime dateofBirth, out int finalMonth, out int finalDay)
        {
            DateTime currentDate = GetStartDateForAge(year);
            finalMonth = 0;
            finalDay = 0;
            if (dateofBirth.Date > currentDate.Date) return false;

            int CurrentYear = currentDate.Year;
            int CurrentMonth = currentDate.Month;
            int CurrentDay = currentDate.Day;

            int Year1 = dateofBirth.Year;
            int Month1 = dateofBirth.Month;
            int Day1 = dateofBirth.Day;

            //1.先计算天数
            finalDay = CurrentDay - Day1;
            while (finalDay < 0)//天数存在借了一个月，也不够减的情况，例如借来的月份是2月，就只有28天或29天
            {
                CurrentMonth = CurrentMonth - 1; //向月份借1
                if (CurrentMonth == 0)//月份也不够，则要向年份借1
                {
                    CurrentYear = CurrentYear - 1;
                    CurrentMonth = 12;
                }
                CurrentDay = DateTime.DaysInMonth(CurrentYear, CurrentMonth) + CurrentDay;
                finalDay = CurrentDay - Day1;
            }
            //2. 计算月数(月份比较)
            finalMonth = CurrentMonth - Month1;
            if (finalMonth < 0)//不存在借了也不够的情况，所以判断一次即可
            {
                CurrentYear = CurrentYear - 1;
                finalMonth = 12 + CurrentMonth - Month1;
            }

            //3. 计算月数(年份比较，即年份转换为月份)
            finalMonth = (CurrentYear - Year1) * 12 + finalMonth;

            if (finalMonth < 0)//生日不合法，即生日在将来时间
            {
                return false;
            }
            return true;
        }

        public static string GetAge(DateTime dtBirthday, DateTime dtNow)
        {
            string strAge = string.Empty;                         // 年龄的字符串表示
            int intYear = 0;                                    // 岁
            int intMonth = 0;                                    // 月
            int intDay = 0;                                    // 天
            // 如果没有设定出生日期, 返回空
            if (dtBirthday > dtNow)
            {
                return string.Empty;
            }

            // 计算天数
            intDay = dtNow.Day - dtBirthday.Day;
            if (intDay < 0)
            {
                dtNow = dtNow.AddMonths(-1);
                intDay += DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
            }

            // 计算月数
            intMonth = dtNow.Month - dtBirthday.Month;
            if (intMonth < 0)
            {
                intMonth += 12;
                dtNow = dtNow.AddYears(-1);
            }

            // 计算年数
            intYear = dtNow.Year - dtBirthday.Year;

            // 格式化年龄输出
            if (intYear >= 1)
            {
                if (intYear == 1)
                    strAge = intYear + " year";
                else
                    strAge = intYear + " years";
            }
            if (intMonth > 0)
            {
                if (intMonth == 1)
                    strAge += intMonth + " month";
                else
                    strAge += intMonth + " months";
            }
            return strAge;
        }
        /// <summary>
        /// For Student Age calculation, hardcode with Septemper.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GetStartDateForAge(int year)
        {
            if (DateTime.Now.Year == year && DateTime.Now.Month < _yearSeparate)
            {
                return new DateTime(year - 1, 9, 1);
            }
            return new DateTime(year, 9, 1);
        }

        /// <summary>
        /// For Student Age calculation, hardcode with Septemper.
        /// </summary>
        /// <param name="schoolYear">The school year:14-15.</param>
        /// <returns></returns>
        public static DateTime GetStartDateForAge(string schoolYear)
        {
            var year = int.Parse(schoolYear.Substring(0, 2)) + 2000;
            return new DateTime(year, 9, 1);
        }
        /// <summary>
        /// For Assessment: School View, Class View, Student View. Hardcode with 8
        /// David 08/01/2017
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartDateOfSchoolYear()
        {
            return new DateTime(CommonAgent.Year, 8, 1);
            //return new DateTime(CommonAgent.Year, CommonAgent.YearSeparate, 1);
        }

        /// <summary>
        /// For Cot Report ,不同年份开始时间
        /// </summary>
        /// <param name="year">四位数的年份</param>
        /// <returns></returns>
        public static DateTime GetStartDateOfYear(int year)
        {
            return new DateTime(year, _yearSeparate, 1);
        }
        /// <summary>
        /// 转化SchoolYear.返回14-15格式
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public static string ToSchoolYearString(this int year)
        {
            if (year == DateTime.Now.Year && DateTime.Now.Month < _yearSeparate)
                year--;
            return string.Format("{0}-{1}", year.ToString().Substring(2, 2),
                (year + 1).ToString().Substring(2, 2));
        }

        /// <summary>
        /// 返回2014-2015格式
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static string ToFullSchoolYearString(this int year)
        {
            if (year == DateTime.Now.Year && DateTime.Now.Month < _yearSeparate)
                year--;
            return string.Format("{0}-{1}", year, year + 1);
        }

        /// <summary>
        /// 返回2014-2015格式
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static string ToFullSchoolYearString(this string year)
        {
            if (string.IsNullOrEmpty(year))
            {
                return string.Empty;
            }
            if (year.Length == 5 && year.IndexOf("-") >= 0)
            {
                var ys = year.Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                return string.Join("-", ys.Select(y => "20" + y));
            }
            return string.Empty;
        }

        /// <summary>
        /// 验证邮箱格式是否正确(只验证大致格式，没有验证正确性)
        /// </summary>
        /// <param name="str_Email"></param>
        /// <returns></returns>
        public static bool IsEmail(string str_Email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_Email, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }
    }
}
