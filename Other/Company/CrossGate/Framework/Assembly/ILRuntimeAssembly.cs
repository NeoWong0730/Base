using Lib.AssetLoader;
using System;
using System.IO;
using System.Linq;

namespace Framework
{
    public class ILRuntimeAssembly : IAssembly
    {
        private ILRuntime.Runtime.Enviorment.AppDomain mAppDomain;
        private MemoryStream dllMemory;
        private MemoryStream pdbMemory;

        public IInstanceMethod CreateInstanceMethod(string typeName, string methodName, ref object _instance, int paramsCount)
        {
            return new ILInstanceMethod(mAppDomain, typeName, methodName, ref _instance, paramsCount);
        }

        public IStaticMethod CreateStaticMethod(string typeName, string methodName, int paramsCount)
        {
            return new ILStaticMethod(mAppDomain, typeName, methodName, paramsCount);
        }

        public void Load()
        {
#if !UNITY_EDITOR || FORCE_ENCRYPT
            string dir = AssetPath.sAssetLogicDir;
            string dllPath = string.Format("{0}/{1}.bytes", dir, AssetPath.sLogicDllName);
            string pdbPath = string.Format("{0}/{1}.bytes", dir, AssetPath.sLogicPdbName);
#else
            string dir = "/../Library/ScriptAssemblies";
            string dllPath = string.Format("{0}/{1}", dir, AssetPath.sLogicDllName);
            string pdbPath = string.Format("{0}/{1}", dir, AssetPath.sLogicPdbName);
#endif

            byte[] dllBytes = AssetMananger.Instance.LoadBytes(dllPath, true, Consts.sLogicPassword);
            dllMemory = new MemoryStream(dllBytes);

            mAppDomain = new ILRuntime.Runtime.Enviorment.AppDomain(ILRuntime.Runtime.ILRuntimeJITFlags.None);
#if UNITY_EDITOR && ILRUNTIME_TEST
            mAppDomain.AllowUnboundCLRMethod = false;//必须走生成的绑定代码，不走反射
#endif

#if DEBUG_MODE
            byte[] pdbBytes = AssetMananger.Instance.LoadBytes(pdbPath, true, Consts.sLogicPassword);
            pdbMemory = new MemoryStream(pdbBytes);
            mAppDomain.LoadAssembly(dllMemory, pdbMemory, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
#else
            mAppDomain.LoadAssembly(dllMemory);
#endif
//            ILHelper.InitILRuntime(mAppDomain);

#if DEBUG && !NO_PROFILER && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            mAppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        }

        public void Binding()
        {
            ILHelper.InitILRuntime(mAppDomain);
        }

        public void Unload()
        {
            mAppDomain?.Dispose();
            pdbMemory?.Dispose();
            dllMemory?.Dispose();
        }

        public object CreateInstance(string fullName)
        {
            try
            {
                return mAppDomain.Instantiate(fullName);
            }
            catch
            {
                return null;
            }
        }

        public Type[] GetTypes()
        {
            return mAppDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
        }       
    }
}