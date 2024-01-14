using System.Collections.Generic;
using System.Reflection;

namespace NWFramework.Editor
{
    /// <summary>
    /// 类型相关的实用函数
    /// </summary>
    internal static class Type
    {
        private static readonly string[] RuntimeAssemblyNames =
        {
            "NWFramework.Runtime",
            "Assembly-CSharp",
        };

        private static readonly string[] RuntimeOrEditorAssemblyNames =
        {
            "NWFramework.Runtime",
            "Assembly-CSharp",
            "NWFramework.Editor",
            "Assembly-CSharp-Editor",
        };

        /// <summary>
        /// 在运行时程序集中获取指定基类的所有子类的名称
        /// </summary>
        /// <param name="typeBase">基类类型</param>
        /// <returns>指定基类的所有子类的名称</returns>
        internal static string[] GetRuntimeTypeNames(System.Type typeBase)
        {
            return GetTypeNames(typeBase, RuntimeAssemblyNames);
        }

        /// <summary>
        /// 在运行时或编辑器程序集中获取指定基类的所有子类的名称
        /// </summary>
        /// <param name="typeBase">基类类型</param>
        /// <returns>指定基类的所有子类的名称</returns>
        internal static string[] GetRuntimeOrEdiorTypeNames(System.Type typeBase)
        {
            return GetTypeNames(typeBase, RuntimeOrEditorAssemblyNames);
        }

        private static string[] GetTypeNames(System.Type typeBase, string[] assemblyNames)
        {
            List<string> typeNames = new List<string>();
            foreach (string assemblyName in assemblyNames)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch
                {
                    continue;
                }

                if (assembly == null)
                {
                    continue;
                }

                System.Type[] types = assembly.GetTypes();
                foreach (System.Type type in types)
                {
                    if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                    {
                        typeNames.Add(type.FullName);
                    }
                }
            }

            typeNames.Sort();
            return typeNames.ToArray();
        }
    }
}