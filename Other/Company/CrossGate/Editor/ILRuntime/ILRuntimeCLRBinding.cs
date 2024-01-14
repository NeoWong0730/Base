using ILRuntime.Runtime.Enviorment;
using Lib.AssetLoader;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{    
    public static void GenerateCLRBindingByAnalysis(string sAssetLogicDir, string sLogicDllName, List<string> assemblyNames, List<string> typeNames)
    {
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();

        string path = string.Format("Assets/{0}/{1}.bytes", sAssetLogicDir, sLogicDllName);
        using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            domain.LoadAssembly(fs);

            //Crossbind Adapter is needed to generate the correct binding code
            InitILRuntime(domain, assemblyNames, typeNames);
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/Scripts/Framework/ILRuntime/Generated/CLRBindings");
        }
    }


    public static void GenerateCLRBindingByAnalysis()
    {
        string CLRBindingCodePath = "Assets/Scripts/Framework/ILRuntime/Generated/CLRBindings";
        if (Directory.Exists(CLRBindingCodePath))
        {
            Directory.Delete(CLRBindingCodePath, true);
        }
        Directory.CreateDirectory(CLRBindingCodePath);
        AssetDatabase.Refresh();

        List<string> assemblyNames = new List<string>();
        List<string> typeNames = new List<string>();
        GenerateCLRBindingByAnalysis(AssetPath.sAssetLogicDir, AssetPath.sLogicDllName, assemblyNames, typeNames);
        AssetDatabase.Refresh();
    }

    
    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain, List<string> assemblyNames, List<string> typeNames)
    {
        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        Framework.ILHelper.RegisterILRuntime(appdomain);
    }
}
