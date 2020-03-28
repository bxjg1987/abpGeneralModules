using Abp.Localization;
using Abp.Localization.Sources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Utils.Localization
{
    public static class BXJGUtilsLocalizationExt
    {
        #region 获取枚举的本地化列表

        public static IDictionary<int, string> GetEnum(this ILocalizationSource localizationSource, Type enumType)
        {
            if (!enumType.IsEnum)
                throw new InvalidEnumArgumentException();

            var fields = enumType.GetFields();
            if (fields == null || fields.Length <= 1)
                throw new InvalidEnumArgumentException();

            //这种方式要反射很多次
            //var ary = Enum.GetValues(enumType);
            //foreach (var item in ary)
            //{
            //    dic.Add((int)item, localizationSource.GetString(item.ToString()));
            //}

            var dic = new Dictionary<int, string>();
            for (int i = 1; i < fields.Length; i++)
            {
                var field = fields[i];
                var fieldName = field.Name;
                var localizationKey = enumType.FullName + "." + fieldName;
                var attr = field.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;
                if (attr != null)
                    localizationKey = attr.Description;
                var val = Enum.Parse(enumType, fieldName);
                dic.Add((int)val, localizationSource.GetString(localizationKey));
            }
            return dic;
        }
        public static IDictionary<int, string> GetEnum<T>(this ILocalizationSource localizationSource) where T : Enum
        {
            return localizationSource.GetEnum(typeof(T));
        }
        //public static IList<KeyValuePair<int?, string>> GetNullableEnum(this ILocalizationSource localizationSource, Type enumType)
        //{
        //    return localizationSource.GetUnknown<int>(localizationSource.GetEnum(enumType));
        //}
        //public static IList<KeyValuePair<int?, string>> GetNullableEnum<T>(this ILocalizationSource localizationSource) where T : Enum
        //{
        //    return localizationSource.GetUnknown<int>(localizationSource.GetEnum<T>());
        //}

        public static IDictionary<int, string> GetEnum(this ILocalizationManager localizationManager, string sourceName, Type enumType)
        {
            return localizationManager.GetSource(sourceName).GetEnum(enumType);
        }
        public static IDictionary<int, string> GetEnum<T>(this ILocalizationManager localizationManager, string sourceName) where T : Enum
        {
            return localizationManager.GetSource(sourceName).GetEnum<T>();
        }
        //public static IList<KeyValuePair<int?, string>> GetNullableEnum(this ILocalizationManager localizationManager, string sourceName, Type enumType)
        //{
        //    return localizationManager.GetSource(sourceName).GetNullableEnum(enumType);
        //}
        //public static IList<KeyValuePair<int?, string>> GetNullableEnum<T>(this ILocalizationManager localizationManager, string sourceName) where T : Enum
        //{
        //    return localizationManager.GetSource(sourceName).GetNullableEnum<T>();
        //}

        #endregion

        #region 获取指定枚举值对应的本地化文本
        /// <summary>
        /// 获取指定枚举值对应的本地化文本
        /// 如：Gender.Man  则获取 “男”
        /// </summary>
        /// <param name="localizationSource"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnum(this ILocalizationSource localizationSource, Enum obj)
        {
            if (obj == null)
                return null;


            Type t = obj.GetType();
            string objName = obj.ToString();

            FieldInfo fi = t.GetField(objName);

            var localizationKey = t.FullName + "." + objName;

            var attr = fi.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;
            if (attr != null)
                localizationKey = attr.Description;

            return localizationSource.GetString(localizationKey);
        }
        /// <summary>
        /// 获取指定枚举值对应的本地化文本
        /// 如：Gender.Man  则获取 “男”
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="localizationSource"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnum<T>(this ILocalizationSource localizationSource, T obj) where T : Enum
        {
            return localizationSource.GetEnum(obj as Enum);
        }
        /// <summary>
        /// 获取指定枚举值对应的本地化文本
        /// 如：Gender.Man  则获取 “男”
        /// </summary>
        /// <param name="localizationManager"></param>
        /// <param name="sourceName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnum(this ILocalizationManager localizationManager, string sourceName, Enum obj)
        {
            return localizationManager.GetSource(sourceName).GetEnum(obj);
        }
        /// <summary>
        /// 获取指定枚举值对应的本地化文本
        /// 如：Gender.Man  则获取 “男”
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="localizationManager"></param>
        /// <param name="sourceName">本地化资源名</param>
        /// <param name="obj">枚举值</param>
        /// <returns></returns>
        public static string GetEnum<T>(this ILocalizationManager localizationManager, string sourceName, T obj) where T : Enum
        {
            return localizationManager.GetSource(sourceName).GetEnum<T>(obj);
        }
        #endregion

        #region 为指定类型本身获取本地化字符串
        private static string GetLocalizationKeyForType(Type type)
        {
            var localizationKey = type.FullName;
            var attr = type.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;
            if (attr != null)
                localizationKey = attr.Description;
            return localizationKey;
        }
        #region MyRegion
        /// <summary>
        /// 为指定类型本身获取本地化字符串
        /// 如：.GetForType(typeof(Gender))  返回"性别"
        /// </summary>
        /// <param name="localizationSource"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetForType(this ILocalizationSource localizationSource, Type type)
        {
            //var localizationKey = type.FullName;
            //var attr = type.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;
            //if (attr != null)
            //    localizationKey = attr.Description;
            return localizationSource.GetString(GetLocalizationKeyForType(type));
        }
        /// <summary>
        /// 为指定类型本身获取本地化字符串
        /// 如：.GetForType(typeof(Gender))  返回"性别"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="localizationSource"></param>
        /// <returns></returns>
        public static string GetForType<T>(this ILocalizationSource localizationSource)
        {
            return localizationSource.GetForType(typeof(T));
        }
        /// <summary>
        /// 为指定类型本身获取本地化字符串
        /// 如：.GetForType(typeof(Gender))  返回"性别"
        /// </summary>
        /// <param name="localizationManager"></param>
        /// <param name="sourceName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetForType(this ILocalizationManager localizationManager, string sourceName, Type type)
        {
            return localizationManager.GetSource(sourceName).GetForType(type);
        }
        /// <summary>
        /// 为指定类型本身获取本地化字符串
        /// 如：.GetForType(typeof(Gender))  返回"性别"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="localizationManager"></param>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        public static string GetForType<T>(this ILocalizationManager localizationManager, string sourceName)
        {
            return localizationManager.GetSource(sourceName).GetForType<T>();
        }
        #endregion
        #region MyRegion
        /// <summary>
        /// 为指定类型本身获取本地化字符串
        /// 如：.GetForType(typeof(Gender))  返回"性别"
        /// </summary>
        /// <param name="localizationSource"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetForTypeOrNull(this ILocalizationSource localizationSource, Type type)
        {
            return localizationSource.GetStringOrNull(GetLocalizationKeyForType(type));
        }
        /// <summary>
        /// 为指定类型本身获取本地化字符串
        /// 如：.GetForType(typeof(Gender))  返回"性别"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="localizationSource"></param>
        /// <returns></returns>
        public static string GetForTypeOrNull<T>(this ILocalizationSource localizationSource)
        {
            return localizationSource.GetForTypeOrNull(typeof(T));
        }
        /// <summary>
        /// 为指定类型本身获取本地化字符串
        /// 如：.GetForType(typeof(Gender))  返回"性别"
        /// </summary>
        /// <param name="localizationManager"></param>
        /// <param name="sourceName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetForTypeOrNull(this ILocalizationManager localizationManager, string sourceName, Type type)
        {
            return localizationManager.GetSource(sourceName).GetForTypeOrNull(type);
        }
        /// <summary>
        /// 为指定类型本身获取本地化字符串
        /// 如：.GetForType(typeof(Gender))  返回"性别"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="localizationManager"></param>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        public static string GetForTypeOrNull<T>(this ILocalizationManager localizationManager, string sourceName)
        {
            return localizationManager.GetSource(sourceName).GetForTypeOrNull<T>();
        }
        #endregion
        #endregion

        #region 获取boo值的本地化数据
        public static string GetTrue(this ILocalizationSource localizationSource)
        {
            return localizationSource.GetString(true.ToString());
        }
        public static string GetFalse(this ILocalizationSource localizationSource)
        {
            return localizationSource.GetString(false.ToString());
        }
        public static string GetBool(this ILocalizationSource localizationSource, bool value)
        {
            return localizationSource.GetString(value.ToString());
        }
        public static IDictionary<bool, string> GetBool(this ILocalizationSource localizationSource)
        {
            var dic = new Dictionary<bool, string>();
            dic.Add(false, localizationSource.GetBool(false));
            dic.Add(true, localizationSource.GetBool(true));
            return dic;
        }

        public static string GetTrue(this ILocalizationManager localizationManager, string sourceName)
        {
            return localizationManager.GetSource(sourceName).GetTrue();
        }
        public static string GetFalse(this ILocalizationManager localizationManager, string sourceName)
        {
            return localizationManager.GetSource(sourceName).GetFalse();
        }
        public static string GetBool(this ILocalizationManager localizationManager, string sourceName, bool value)
        {
            return localizationManager.GetSource(sourceName).GetBool(value);
        }
        public static IDictionary<bool, string> GetBool(this ILocalizationManager localizationManager, string sourceName)
        {
            return localizationManager.GetSource(sourceName).GetBool();
        }
        #endregion

        #region MyRegion
        //public static string GetUnknown(this ILocalizationSource localizationSource)
        //{
        //    return localizationSource.GetString("Unknown");
        //}
        //public static KeyValuePair<T?, string> GetUnknown<T>(this ILocalizationSource localizationSource) where T : struct
        //{
        //    return new KeyValuePair<T?, string>(default(T?), localizationSource.GetUnknown());
        //}
        //public static IList<KeyValuePair<T?, string>> GetUnknown<T>(this ILocalizationSource localizationSource, IDictionary<T, string> dic) where T : struct
        //{
        //    var dic1 = new List<KeyValuePair<T?, string>>();
        //    foreach (var item in dic)
        //    {
        //        dic1.Add(new KeyValuePair<T?, string>(item.Key, item.Value));
        //    }
        //    var unknown = new KeyValuePair<T?, string>(default(T?), localizationSource.GetUnknown());
        //    dic1.Add(new KeyValuePair<T?, string>(unknown.Key, unknown.Value));
        //    return dic1;
        //}
        #endregion

        #region MyRegion
        /// <summary>
        /// 获取Utils模块中的指定键的本地化字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string UtilsL(this string key)
        {
            return Abp.Localization.LocalizationHelper.GetString(BXJGUtilsConsts.LocalizationSourceName, key);
        }
        /// <summary>
        /// 获取Utils模块中的指定键的本地化ILocalizableString
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ILocalizableString UtilsLI(this string key)
        {
            return new LocalizableString(key, BXJGUtilsConsts.LocalizationSourceName);
        }
        #endregion
    }
}
