using System;
using System.ComponentModel;
using System.Linq;
using Attributes;
using Networking.Utils;

namespace Utils.Extensions
{
    public static class EnumExtension {
        /// <summary>
        /// Возвращает описание перечисления по значению
        /// </summary>
        public static string GetDescription(this Enum value) 
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttribute = (DescriptionAttribute) Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
            
            return descriptionAttribute != null 
                ? descriptionAttribute.Description 
                : value.ToString();
        }

        /// <summary>
        /// Возвращает значение перечисления по его описанию
        /// </summary>
        public static T GetFromDescription<T>(string desc)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields()) 
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute) 
                {
                    if (attribute.Description == desc) return (T) field.GetValue(null);
                }
                else 
                {
                    if (field.Name == desc) return (T) field.GetValue(null);
                }
            }

            throw new ArgumentException($"[EnumExtension] Not found item by description - {desc}");
        }

        /// <summary>
        /// Возвращает список описаний всех элементов перечисления  
        /// </summary>
        public static string[] GetDescriptions<T>() where T: Enum
        {
            var values = (T[]) Enum.GetValues(typeof(T));
            return values.Select(e => e.GetDescription()).ToArray();
        }

        /// <summary>
        /// Получает из типа сообщения тип, кому предназанчено это сообщение
        /// </summary>
        public static MessageDestinationTypes GetMessageDestination(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttribute = (MessageDestination) Attribute.GetCustomAttribute(fieldInfo, typeof(MessageDestination));

            return descriptionAttribute?.Type ?? MessageDestinationTypes.Null;
        }

        /// <summary>
        /// Данный тип сообщения поддерживается сервером  
        /// </summary>
        public static bool IsServerSupported(this MessageDestinationTypes type)
            => type == MessageDestinationTypes.Server || type == MessageDestinationTypes.Both;

        /// <summary>
        /// Данный тип сообщения поддерживается клиентом  
        /// </summary>
        public static bool IsClientSupported(this MessageDestinationTypes type)
            => type == MessageDestinationTypes.Client || type == MessageDestinationTypes.Both;
    }
}