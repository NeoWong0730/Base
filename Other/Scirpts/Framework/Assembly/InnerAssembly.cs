using System;
using System.Reflection;

namespace Framework
{
    public class InnerAssembly : IAssembly
    {
        private Assembly mAssembly;

        public IInstanceMethod CreateInstanceMethod(string typeName, string methodName, ref object _instance, int paramsCount)
        {
            return new MonoInstanceMethod(mAssembly, typeName, methodName, ref _instance);
        }

        public IStaticMethod CreateStaticMethod(string typeName, string methodName, int paramsCount)
        {
            return new MonoStaticMethod(mAssembly, typeName, methodName);
        }

        public void Load()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.FullName.StartsWith("Logic,", StringComparison.Ordinal))
                {
                    mAssembly = assembly;
                    break;
                }
            }
        }

        public void Unload()
        {

        }

        public object CreateInstance(string fullName)
        {
            try
            {
                return mAssembly.CreateInstance(fullName);
            }
            catch
            {
                return null;
            }
        }

        public Type[] GetTypes()
        {
            return mAssembly.GetTypes();
        }
    }
}
