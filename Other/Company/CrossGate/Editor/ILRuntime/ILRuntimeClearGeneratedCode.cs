using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

//[InitializeOnLoad]//嘿嘿，很特别，自动生成
public class ILRuntimeClearGeneratedCode 
{
    static ILRuntimeClearGeneratedCode()
    {
        CompilationPipeline.compilationStarted += ClearGenerateCLRBinding;
    }
    
    private static void ClearGenerateCLRBinding(object str)
    {
        string CLRBindingCodePath = "Assets/Scripts/Framework/ILRuntime/Generated/CLRBindings";

        if (Directory.Exists(CLRBindingCodePath))
        {
            if (File.Exists(CLRBindingCodePath + "/CLRBindings.cs"))
            {
                return;
            }
            else
            {
                GeneratedCode(CLRBindingCodePath);
            }
        }
        else
        {
            Directory.CreateDirectory(CLRBindingCodePath);
            GeneratedCode(CLRBindingCodePath);
        }
        
        AssetDatabase.Refresh();
    }


    private static void GeneratedCode(string CLRBindingCodePath)
    {
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(CLRBindingCodePath + "/CLRBindings.cs", false, new UTF8Encoding(false)))
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    public class CLRBindings
    {  
        ILRuntime.Runtime.Generated.CLRBindings.isGeneratedBinding = false;
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}");
            sw.Write(sb);
        }
    }





}
