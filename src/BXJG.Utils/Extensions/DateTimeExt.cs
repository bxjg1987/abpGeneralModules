using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLJ.Utils.Extensions
{
    public static class DateTimeExt
    {
        public static int ToAge(this DateTime p_FirstDateTime)
        {
            return int.Parse(p_FirstDateTime.GetAgeString().Split('岁')[0]);
        }
        /// <summary>
        /// 获得年龄字符串：某个日期点到今天的年龄
        /// 默认返回：xx岁xx月xx天
        /// </summary>
        /// <param name="p_FirstDateTime">第1个日期参数</param>
        public static string GetAgeString(this DateTime p_FirstDateTime)
        {
            return CalculateAgeString(p_FirstDateTime, System.DateTime.Now, null);
        }
        /// <summary>
        /// 获得年龄字符串：某个日期点到今天的年龄
        /// 默认返回：xx岁xx月xx天
        /// </summary>
        /// <param name="p_FirstDateTime">第1个日期参数</param>
        /// <param name="p_Format">返回字符串的格式，默认为：{0}岁{1}月{2}天</param>
        public static string GetAgeString(this DateTime p_FirstDateTime, string p_ReturnFormat)
        {
            return CalculateAgeString(p_FirstDateTime, System.DateTime.Now, p_ReturnFormat);
        }
        /// <summary>
        /// 获得年龄字符串：两个日期点之间的年龄
        /// 默认返回：xx岁xx月xx天
        /// </summary>
        /// <param name="p_FirstDateTime">第1个日期参数</param>
        /// <param name="p_SecondDateTime">第2个日期参数</param>
        public static string GetAgeString(this DateTime p_FirstDateTime, System.DateTime p_SecondDateTime)
        {
            return CalculateAgeString(p_FirstDateTime, p_SecondDateTime, null);
        }
        /// <summary>
        /// 获得年龄字符串：两个日期点之间的年龄
        /// 默认返回：xx岁xx月xx天
        /// </summary>
        /// <param name="p_FirstDateTime">第1个日期参数</param>
        /// <param name="p_SecondDateTime">第2个日期参数</param>
        /// <param name="p_Format">返回字符串的格式，默认为：{0}岁{1}月{2}天</param>
        public static string GetAgeString(this DateTime p_FirstDateTime, System.DateTime p_SecondDateTime, string p_ReturnFormat)
        {
            return CalculateAgeString(p_FirstDateTime, p_SecondDateTime, p_ReturnFormat);
        }
        /// <summary>
        /// 计算年龄字符串
        /// 默认返回：xx岁xx月xx天
        /// </summary>
        /// <param name="p_FirstDateTime">第1个日期参数</param>
        /// <param name="p_SecondDateTime">第2个日期参数</param>
        /// <param name="p_Format">返回字符串的格式，默认为：{0}岁{1}月{2}天</param>
        private static string CalculateAgeString(this DateTime p_FirstDateTime, System.DateTime p_SecondDateTime, string p_ReturnFormat)
        {
            //判断时间段是否为正。若为负，调换两个时间点的位置。
            if (System.DateTime.Compare(p_FirstDateTime, p_SecondDateTime) > 0)
            {
                System.DateTime stmpDateTime = p_FirstDateTime;
                p_FirstDateTime = p_SecondDateTime;
                p_SecondDateTime = stmpDateTime;
            }

            //判断返回字符串的格式。若为空，则给默认值：{0}岁{1}月{2}天
            if (string.IsNullOrEmpty(p_ReturnFormat)) p_ReturnFormat = "{0}岁{1}月{2}天";

            //定义：年、月、日
            int year, month, day;

            //计算：天
            day = p_SecondDateTime.Day - p_FirstDateTime.Day;
            if (day < 0)
            {
                day += System.DateTime.DaysInMonth(p_FirstDateTime.Year, p_FirstDateTime.Month);
                p_FirstDateTime = p_FirstDateTime.AddMonths(1);
            }
            //计算：月
            month = p_SecondDateTime.Month - p_FirstDateTime.Month;
            if (month < 0)
            {
                month += 12;
                p_FirstDateTime = p_FirstDateTime.AddYears(1);
            }
            //计算：年
            year = p_SecondDateTime.Year - p_FirstDateTime.Year;

            //返回格式化后的结果
            return string.Format(p_ReturnFormat, year, month, day);
        }
    }
}
