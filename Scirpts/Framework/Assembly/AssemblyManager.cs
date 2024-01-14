using System;

namespace Framework
{
    public interface IAssembly
    {
        IInstanceMethod CreateInstanceMethod(string typeName, string methodName, ref object _instance, int paramsCount);
        IStaticMethod CreateStaticMethod(string typeName, string methodName, int paramsCount);
        void Load();
        void Unload();
        object CreateInstance(string fullName);
        Type[] GetTypes();
    }

    public class AssemblyManager : TSingleton<AssemblyManager>
    {
        private IAssembly mAssembly;

        public void RegisterAssembly(IAssembly assembly)
        {
            mAssembly = assembly;
        }

        public IInstanceMethod CreateInstanceMethod(string typeName, string methodName, ref object _instance, int paramsCount)
        {
            return mAssembly.CreateInstanceMethod(typeName, methodName, ref _instance, paramsCount);
        }

        public IStaticMethod CreateStaticMethod(string typeName, string methodName, int paramsCount)
        {
            return mAssembly.CreateStaticMethod(typeName, methodName, paramsCount);
        }

        public object CreateInstacen(string fullName)
        {
            return mAssembly.CreateInstance(fullName);
        }

        public Type[] GetTypes()
        {
            return mAssembly.GetTypes();
        }
    }
}
