using Framework;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using Lib.AssetLoader;
using System.IO;
using System.Text;

public class ILRuntimeMenu
{
    private const string selectILRuntimeModeMenuPath = "Tools/ILRuntime/使用ILRuntime";
    private const string createAdapterMenuPath = "Tools/ILRuntime/生成适配器";
    private const string createCLRBindingCode = "Tools/ILRuntime/生成CLR绑定代码";
    private const string clearCLRBindingCode = "Tools/ILRuntime/清除生成的绑定代码";
    private const string AdapterGenPath = "Assets/Scripts/Framework/ILRuntime/Generated/Adapters/";
    private static List<string> whiteUserAssemblyList = new List<string>()
    {
            "Framework",
    };
    ///适配器的完整类型字符串，命名空间在内的///
    private static List<string> adapterGenList = new List<string>()
    {
        //"ydtest"
    };

    [MenuItem(selectILRuntimeModeMenuPath, priority = 0)]
    public static void SelectILRuntimeMode_Menu()
    {
        bool bSelected = Menu.GetChecked(selectILRuntimeModeMenuPath);
        EditorPrefs.SetBool(Consts.IsILRuntimeModeKey, !bSelected);
        Menu.SetChecked(selectILRuntimeModeMenuPath, !bSelected);

        string defineStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> defines = new List<string>(defineStr.Split(';'));

        if (EditorPrefs.GetBool(Consts.IsILRuntimeModeKey))
        {
            if (!defines.Contains("ILRUNTIME_MODE"))
            {
                defines.Add("ILRUNTIME_MODE");
            }
        }
        else
        {
            if (defines.Contains("ILRUNTIME_MODE"))
            {
                defines.Remove("ILRUNTIME_MODE");
            }
            if (defines.Contains("MONO_REFLECT_MODE"))
            {
                defines.Remove("MONO_REFLECT_MODE");
            }
            //取消勾选ILRUNTIME_MODE，同时默认设置 运行时的模式为Normal
        }

        string newDefineStr = string.Join(";", defines);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefineStr);
        AssetDatabase.Refresh();
    }

    [MenuItem(selectILRuntimeModeMenuPath, true)]
    public static bool SelectILRuntimeMode_Check_Menu()
    {
        Menu.SetChecked(selectILRuntimeModeMenuPath, EditorPrefs.GetBool(Consts.IsILRuntimeModeKey));
        return true;
    }

    [MenuItem(createAdapterMenuPath)]
    public static void CreateAdapter()
    {
        //1.生成跨域继承适配器
        //AdapterGenTool.CreateAdapter(Consts.GameNameSpaceStr, AdapterGenPath, whiteUserAssemblyList, adapterGenList);

        //2.官方给的案列
        //由于跨域继承特殊性太多，自动生成无法实现完全无副作用生成，所以这里提供的代码自动生成主要是给大家生成个初始模版，简化大家的工作
        //大多数情况直接使用自动生成的模版即可，如果遇到问题可以手动去修改生成后的文件，因此这里需要大家自行处理是否覆盖的问题
        //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("Assets/Scripts/Framework/ILRuntime/Generated/Adapters/UIBaseAdapter.cs"))
        //{
        //    sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(typeof(Framework.Core.UI.FUIBase), "Framework.Adaptor"));
        //}
    }

    [MenuItem(createCLRBindingCode)]
    public static void GenerateCLRBindingByAnalysis()
    {

        if (Application.isPlaying || EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            EditorUtility.DisplayDialog("错误", "游戏正在运行或者暂定，请停止运行后在操作！", "确定");
            return;
        }

        if (EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("错误", "游戏脚本正在编译，请编译完在操作", "确定");
            return;
        }
     

        //1.删除老的生成的绑定代码
        string CLRBindingCodePath = "Assets/Scripts/Framework/ILRuntime/Generated";
        if (Directory.Exists(CLRBindingCodePath))
            Directory.Delete(CLRBindingCodePath, true);
        Directory.CreateDirectory(CLRBindingCodePath + "/CLRBindings");
        AssetDatabase.Refresh();

        //2.copy from "Library/ScriptAssemblies"  to LogicDll（不用每次都编译） 删除代码并不会编译
        Startup.CopyDllFromLibraryAssembly();


        //3.根据分析dll文件 重新生成新的绑定代码
        List<string> assemblyNames = new List<string>();
        List<string> typeNames = new List<string>();
        ILRuntimeCLRBinding.GenerateCLRBindingByAnalysis(AssetPath.sAssetLogicDir, AssetPath.sLogicDllName, assemblyNames, typeNames);
    }

    [MenuItem(clearCLRBindingCode)]
    public static void ClearGenerateCLRBinding()
    {
        //使用上一层目录来生成绑定代码
        string CLRBindingCodePath = "Assets/Scripts/Framework/ILRuntime/Generated/CLRBindings";
        if (Directory.Exists(CLRBindingCodePath))
        {
            Directory.Delete(CLRBindingCodePath, true);
        }
        //        Directory.CreateDirectory(CLRBindingCodePath);
        //        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(CLRBindingCodePath + "/CLRBindings.cs", false, new UTF8Encoding(false)))
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            sb.AppendLine(@"using System;
        //using System.Collections.Generic;
        //using System.Reflection;

        //namespace ILRuntime.Runtime.Generated
        //{
        //    public partial class CLRBindings
        //    {  
        //        public static bool IsGeneratedBinding()
        //        {
        //            return false;
        //        }
        //        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        //        {
        //        }
        //    }
        //}");
        //            sw.Write(sb);
        //        }
        //        AssetDatabase.Refresh();
    }



}
