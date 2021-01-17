using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace CommentProject.CommentExtention.GeneralExtention
{
    public static class EnumActionExtentions
    {
        /// <summary>
		/// 从枚举中获取Description		
		/// </summary>
		/// <param name="enumName">需要获取枚举描述的枚举</param>
		/// <returns>描述内容</returns>
		public static string ToDescription(this Enum enumName)
        {
            string _description = string.Empty;
            FieldInfo _fieldInfo = enumName.GetType().GetField(enumName.ToString());
            DescriptionAttribute[] _attributes = _fieldInfo.GetDescriptAttr();
            if (_attributes != null && _attributes.Length > 0)
                _description = _attributes[0].Description;
            else
                _description = enumName.ToString();
            return _description;
        }

        /// <summary>
		/// 获取字段Description
		/// </summary>
		/// <param name="fieldInfo">FieldInfo</param>
		/// <returns>DescriptionAttribute[] </returns>
		public static DescriptionAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
        {
            if (fieldInfo != null)
            {
                return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            }
            return null;
        }

        /// <summary>
        /// 获取枚举指定的显示内容
        /// </summary>
        public static object Display(this Enum val, DisplayProperty property)
        {
            var enumType = val.GetType();
            var text = val.ToString();
            var field = enumType.GetField(text);

            if (field != null)
            {
                return field.Display(property);
            }

            return null;
        }

        /// <summary>
        /// 获取枚举指定的显示内容
        /// </summary>
        public static object Display(this MemberInfo memberInfo, DisplayProperty property)
        {
            if (memberInfo == null) return null;

            var display = memberInfo.GetAttribute<DisplayAttribute>();

            if (display != null)
            {
                switch (property)
                {
                    case DisplayProperty.Name:
                        return display.GetName();
                    case DisplayProperty.ShortName:
                        return display.GetShortName();
                    case DisplayProperty.GroupName:
                        return display.GetGroupName();
                    case DisplayProperty.Description:
                        return display.GetDescription();
                    case DisplayProperty.Order:
                        return display.GetOrder();
                    case DisplayProperty.Prompt:
                        return display.GetPrompt();
                }
            }

            return null;
        }

        /// <summary>
        /// 获取成员信息的Attribute
        /// </summary>
        public static TAttr GetAttribute<TAttr>(this MemberInfo member) where TAttr : Attribute
        {
            return Attribute.GetCustomAttribute(member, typeof(TAttr)) as TAttr;
        }

        /// <summary>
        /// 获取枚举说明
        /// </summary>
        public static string DisplayName(this Enum val)
        {
            return val.Display(DisplayProperty.Name) as string;
        }

        /// <summary>
        /// 获取枚举说明
        /// </summary>
        public static string DisplayShortName(this Enum val)
        {
            return val.Display(DisplayProperty.ShortName) as string;
        }

        /// <summary>
        /// 获取枚举分组名称
        /// </summary>
        public static string DisplayGroupName(this Enum val)
        {
            return val.Display(DisplayProperty.GroupName) as string;
        }

        /// <summary>
        /// 获取枚举水印信息
        /// </summary>
        public static string DisplayPrompt(this Enum val)
        {
            return val.Display(DisplayProperty.Prompt) as string;
        }

        /// <summary>
        /// 获取枚举备注
        /// </summary>
        public static string DisplayDescription(this Enum val)
        {
            return val.Display(DisplayProperty.Description) as string;
        }

        /// <summary>
        /// 获取枚举显示排序
        /// </summary>
        public static int? DisplayOrder(this Enum val)
        {
            return val.Display(DisplayProperty.Order) as int?;
        }

        /// <summary>
        /// 列举所有枚举值和描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<EnumDescriptionDto> GetDescriptionToList<T>() where T : struct
        {
            Type type = typeof(T);
            List<EnumDescriptionDto> list = new List<EnumDescriptionDto>();
            foreach (int item in System.Enum.GetValues(type))
            {
                string description = string.Empty;
                try
                {
                    FieldInfo fieldInfo = type.GetField(System.Enum.GetName(type, item));
                    if (fieldInfo == null)
                    {
                        continue;
                    }
                    DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
                    if (da == null)
                    {
                        continue;
                    }
                    description = da.Description;
                }
                catch { }
                list.Add(new EnumDescriptionDto { name = description, type = item });
            }
            return list;
        }

        /// <summary>
        /// 遍历整个枚举
        /// </summary>
        public static void ForEach<T>(Action<int, T, string, string> eachDo) where T : struct
        {
            Type type = typeof(T);
            foreach (object item in System.Enum.GetValues(type))
            {
                string name = string.Empty;
                string description = string.Empty;
                try
                {
                    name = System.Enum.GetName(type, item);
                    FieldInfo fieldInfo = type.GetField(name);
                    if (fieldInfo == null)
                    {
                        continue;
                    }
                    DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
                    if (da == null)
                    {
                        continue;
                    }
                    description = da.Description;
                    eachDo((int)item, (T)item, name, description);
                }
                catch { }
            }
        }

        /// <summary>
        /// 列举所有枚举值和描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<EnumDescriptionDto> GetDescriptionToList2<T>() where T : struct
        {
            Type type = typeof(T);
            List<EnumDescriptionDto> list = new List<EnumDescriptionDto>();
            foreach (int item in System.Enum.GetValues(type))
            {
                string description = string.Empty;
                try
                {
                    FieldInfo fieldInfo = type.GetField(System.Enum.GetName(type, item));
                    if (fieldInfo == null)
                    {
                        continue;
                    }
                    DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
                    if (da == null)
                    {
                        continue;
                    }
                    description = da.Description;
                }
                catch { }
                list.Add(new EnumDescriptionDto { name = description, type = item });
            }
            return list;
        }
        /// <summary>
        /// 列举所有枚举值和索引
        /// </summary>
        /// <param name="typeParam"></param>
        /// <returns></returns>
        public static Dictionary<int, string> EnumToFieldDictionary(Type typeParam)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            foreach (int i in System.Enum.GetValues(typeParam))
            {
                string name = System.Enum.GetName(typeParam, i);
                dictionary.Add(i, name);
            }
            return dictionary;
        }
        /// <summary>
        /// 获取指定枚举值的描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetDescription<T>(T request) where T : struct
        {
            try
            {
                Type type = request.GetType();
                FieldInfo fieldInfo = type.GetField(request.ToString());

                if (fieldInfo == null) { return string.Empty; }

                DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));

                if (da != null)
                {
                    return da.Description;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

    }

    /// <summary>
    /// 定义 <see cref="System.ComponentModel.DataAnnotations.DisplayAttribute"/> 的属性
    /// </summary>
    public enum DisplayProperty
    {
        /// <summary>
        /// 名称
        /// </summary>
        Name,

        /// <summary>
        /// 短名称
        /// </summary>
        ShortName,

        /// <summary>
        /// 分组名称
        /// </summary>
        GroupName,

        /// <summary>
        /// 说明
        /// </summary>
        Description,

        /// <summary>
        /// 排序
        /// </summary>
        Order,

        /// <summary>
        /// 水印信息
        /// </summary>
        Prompt,
    }

    /// <summary>
    /// 枚举信息
    /// </summary>
    public class EnumDescriptionDto
    {
        /// <summary>
        /// 枚举的名称(即：Description)
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 枚举的值
        /// </summary>
        public int type { get; set; }
    }
}

