using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Logic
{
    public static class CodeHelper
    {
        public static List<ValueTuple<Type, T>> GetAllAttributeType<T>(bool includeBuiltinType, bool inherit = false) where T : Attribute
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            Type[] allTypes = includeBuiltinType ? assembly.GetTypes() : assembly.GetExportedTypes();
            List<ValueTuple<Type, T>> alist = new List<(Type, T)>();

            foreach (Type currentType in allTypes)
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(currentType, inherit);
                foreach (Attribute a in attributes)
                {
                    T instance = a as T;
                    if (instance != null)
                    {
                        alist.Add(new ValueTuple<Type, T>(currentType, instance));
                        break;
                    }
                }
            }
            return alist;
        }

        public static bool IsIndexValid(int index, int arrayCount)
        {
            return 0 <= index && index < arrayCount;
        }
        public static bool IsIndexValid<T>(IList<T> list, int index)
        {
            return 0 < index && index < list.Count;
        }

        public static T_Field GetField<T, T_Field>(T target, string name) where T : class
        {
            T_Field ret = default;
            FieldInfo fieldInfo = typeof(T).GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                ret = (T_Field)(fieldInfo.GetValue(target));
            }
            return ret;
        }
        public static bool SetField<T>(T target, string name, System.Object value) where T : class
        {
            bool ret = false;
            FieldInfo fieldInfo = typeof(T).GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(target, value);
                ret = true;
            }
            return ret;
        }
        public static T_Property GetProperty<T, T_Property>(T target, string name) where T : class
        {
            T_Property ret = default;
            PropertyInfo propertyInfo = typeof(T).GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo != null)
            {
                ret = (T_Property)(propertyInfo.GetValue(target));
            }
            return ret;
        }
        public static bool SetProperty<T>(T target, string name, System.Object value) where T : class
        {
            bool ret = false;
            PropertyInfo propertyInfo = typeof(T).GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(target, value);
                ret = true;
            }
            return ret;
        }
    }
}
