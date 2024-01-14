using Lib.AssetLoader;
using System;
using System.Reflection;

namespace Framework
{
    public class MonoAssembly : IAssembly
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
#if UNITY_EDITOR
            string dir = "/../Library/ScriptAssemblies";
            string dllPath = string.Format("{0}/{1}", dir, AssetPath.sLogicDllName);
#else
        string dir = AssetPath.sAssetLogicDir;
        string dllPath = string.Format("{0}/{1}.bytes", dir, AssetPath.sLogicDllName);
#endif

            byte[] dllBytes = AssetMananger.Instance.LoadBytes(dllPath, true, Consts.sLogicPassword);

#if UNITY_EDITOR
            string pdbPath = string.Format("{0}/{1}", dir, AssetPath.sLogicPdbName);
            byte[] pdbBytes = AssetMananger.Instance.LoadBytes(pdbPath, true, Consts.sLogicPassword);

            mAssembly = Assembly.Load(dllBytes, pdbBytes);
#else
        mAssembly = Assembly.Load(dllBytes);
#endif
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