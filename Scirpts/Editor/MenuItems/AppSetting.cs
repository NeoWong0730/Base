using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using System.IO;
using UnityEditor.AddressableAssets.Build;

public class AppSetting : EditorWindow
{
    [MenuItem("__Tools__/GC")]
    public static void GC()
    {
        System.GC.Collect();
    }

    public enum ERuntimeMode
    {
        Normal = 0,
        MonoReflect = 1,
        ILRuntime = 2,
    }




    //bool FORCE_AB = false;
    bool FORCE_ENCRYPT = false;
    bool DEBUG_MODE = false;
    bool BEHAVIAC_RELEASE = false;
    ERuntimeMode eRuntimeMode = ERuntimeMode.Normal;//ERuntimeMode.ILRuntime;
    bool RecordUseAddressable = false;
    bool ILRUNTIME_TEST = false;
    bool USE_SPLIT_FRAME = false;
    bool RESOLUTION_SCALE = false;
    bool USE_HOTFIX_EDITOR = false;
    bool USE_ATLAS_COMBINE = false;
    bool CLOSE_ILRUNTIME_DEBUG = false;
    bool ADDRESSABLES_LOG_ALL = false;
    bool SKIP_SDK_Login = false;
    bool USE_PCSDK = false;
    bool OPEN_PC_KEYCODE_FUN = false;
    bool PROTOCOL_NOENCRYPT = false;
    bool USE_SDK_QRCODE = false;
    bool HAVE_PROFILER = false;
    bool APP_SCORE = false;

    private void OnEnable()
    {
#if FORCE_ENCRYPT
        FORCE_ENCRYPT = true;
#else
        FORCE_ENCRYPT = false;
#endif

#if PROTOCOL_NOENCRYPT
        PROTOCOL_NOENCRYPT = true;
#else
        PROTOCOL_NOENCRYPT = false;
#endif


#if DEBUG_MODE
        DEBUG_MODE = true;
#else
        DEBUG_MODE = false;
#endif

#if BEHAVIAC_RELEASE
        BEHAVIAC_RELEASE = true;
#else
        BEHAVIAC_RELEASE = false;
#endif

#if ILRUNTIME_MODE
        eRuntimeMode = ERuntimeMode.ILRuntime;
#elif MONO_REFLECT_MODE
        eRuntimeMode = ERuntimeMode.MonoReflect;
#else
        eRuntimeMode = ERuntimeMode.Normal;
#endif


#if USE_HOTFIX_EDITOR
        USE_HOTFIX_EDITOR = true;
#else
        USE_HOTFIX_EDITOR = false;
#endif

//#if FORCE_AB
//        FORCE_AB = true;
//#else
//        FORCE_AB = false;
//#endif

#if ILRUNTIME_TEST
        ILRUNTIME_TEST = true;
#else
        ILRUNTIME_TEST = false;
#endif

#if USE_SPLIT_FRAME
        USE_SPLIT_FRAME = true;
#else
        USE_SPLIT_FRAME = false;
#endif


#if RESOLUTION_SCALE
        RESOLUTION_SCALE = true;
#else
        RESOLUTION_SCALE = false;
#endif


#if USE_ATLAS_COMBINE
        USE_ATLAS_COMBINE = true;
#else
        USE_ATLAS_COMBINE = false;
#endif

#if DISABLE_ILRUNTIME_DEBUG
        CLOSE_ILRUNTIME_DEBUG = true;
#else
        CLOSE_ILRUNTIME_DEBUG = false;
#endif


#if ADDRESSABLES_LOG_ALL
        ADDRESSABLES_LOG_ALL = true;
#else
        ADDRESSABLES_LOG_ALL = false;
#endif


#if SKIP_SDK_Login
     SKIP_SDK_Login = true;
#else
     SKIP_SDK_Login = false;
#endif

#if USE_PCSDK
        USE_PCSDK = true;
#else
        USE_PCSDK = false;
#endif

#if OPEN_PC_KEYCODE_FUN
        OPEN_PC_KEYCODE_FUN = true;
#else
        OPEN_PC_KEYCODE_FUN = false;
#endif


#if USE_SDK_QRCODE
        USE_SDK_QRCODE = true;
#else
        USE_SDK_QRCODE = false;
#endif


#if DEBUG && !NO_PROFILER 
        HAVE_PROFILER = true;
#else
        HAVE_PROFILER = false;
#endif

#if APP_SCORE
        APP_SCORE = true;
#else
        APP_SCORE = false;
#endif
    }

    private void OnGUI()
    {
        ILRUNTIME_TEST = EditorGUILayout.Toggle("ILRuntime走绑定(不走反射)", ILRUNTIME_TEST);
        DEBUG_MODE = EditorGUILayout.Toggle("调试模式", DEBUG_MODE);
        //FORCE_AB = EditorGUILayout.Toggle("AB包测试(内网打包)", FORCE_AB);
        SKIP_SDK_Login = EditorGUILayout.Toggle("跳过SDK(正式服登录)", SKIP_SDK_Login);
        USE_HOTFIX_EDITOR = EditorGUILayout.Toggle("热更模式打开(编辑器下)", USE_HOTFIX_EDITOR);
        BEHAVIAC_RELEASE = !EditorGUILayout.Toggle("AI调试模式", !BEHAVIAC_RELEASE);
        FORCE_ENCRYPT = EditorGUILayout.Toggle("文本资源加密", FORCE_ENCRYPT);
        eRuntimeMode = (ERuntimeMode)EditorGUILayout.EnumPopup("运行模式", eRuntimeMode);
        USE_SPLIT_FRAME = EditorGUILayout.Toggle("逻辑限帧分帧计算", USE_SPLIT_FRAME);
        RESOLUTION_SCALE = EditorGUILayout.Toggle("分辨率缩放", RESOLUTION_SCALE);
        USE_ATLAS_COMBINE = EditorGUILayout.Toggle("精灵合并图集", USE_ATLAS_COMBINE);
        CLOSE_ILRUNTIME_DEBUG = EditorGUILayout.Toggle("打开ILRuntime Release模式", CLOSE_ILRUNTIME_DEBUG);
        ADDRESSABLES_LOG_ALL = EditorGUILayout.Toggle("开启Addressable日志", !ADDRESSABLES_LOG_ALL);
        //USE_MTP = EditorGUILayout.Toggle("开启MTP反外挂", !USE_MTP);//Android平台默认开启MTP反外挂
        USE_PCSDK = EditorGUILayout.Toggle("使用PCSDK模式", USE_PCSDK);
        OPEN_PC_KEYCODE_FUN = EditorGUILayout.Toggle("开启PC自定义热键功能", OPEN_PC_KEYCODE_FUN);
        PROTOCOL_NOENCRYPT = EditorGUILayout.Toggle("协议不加密", PROTOCOL_NOENCRYPT);
        USE_SDK_QRCODE = EditorGUILayout.Toggle("使用二维码登录", USE_SDK_QRCODE);
        HAVE_PROFILER = EditorGUILayout.Toggle("连接Profiler", HAVE_PROFILER);
        APP_SCORE = EditorGUILayout.Toggle("App评分测试", APP_SCORE);

        if (GUILayout.Button("Apply"))
        {            
            DoChange();
        }
    }
    

    private void Check(List<string> add, List<string> remove, string symbol, bool bAdd)
    {
        if(bAdd)
        {
            add.Add(symbol);
        }
        else
        {
            remove.Add(symbol);
        }
    }

    private void DoChange()
    {
        List<string> adds = new List<string>();
        List<string> removes = new List<string>();

        //DEBUG_MODE 调试模式的绑定代码生成后，当取消调试模式，会导致编译不通过，此时需要手动删除绑定代码，方可编译成功！！！        

        //Check(adds, removes, "FORCE_AB", FORCE_AB);
        Check(adds, removes, "SKIP_SDK_Login", SKIP_SDK_Login);
        Check(adds, removes, "ILRUNTIME_TEST", ILRUNTIME_TEST);
        Check(adds, removes, "USE_HOTFIX_EDITOR", USE_HOTFIX_EDITOR);
        Check(adds, removes, "FORCE_ENCRYPT", FORCE_ENCRYPT);
        Check(adds, removes, "ILRUNTIME_MODE", eRuntimeMode == ERuntimeMode.ILRuntime);
        Check(adds, removes, "MONO_REFLECT_MODE", eRuntimeMode == ERuntimeMode.MonoReflect);
        Check(adds, removes, "USE_SPLIT_FRAME", USE_SPLIT_FRAME);
        Check(adds, removes, "RESOLUTION_SCALE", RESOLUTION_SCALE);
        Check(adds, removes, "USE_ATLAS_COMBINE", USE_ATLAS_COMBINE);
        Check(adds, removes, "DEBUG_MODE", DEBUG_MODE);
        Check(adds, removes, "DISABLE_ILRUNTIME_DEBUG", CLOSE_ILRUNTIME_DEBUG);
        Check(adds, removes, "BEHAVIAC_RELEASE",  BEHAVIAC_RELEASE);
        Check(adds, removes, "ADDRESSABLES_LOG_ALL", ADDRESSABLES_LOG_ALL);
        Check(adds, removes, "USE_PCSDK", USE_PCSDK);
        Check(adds, removes, "OPEN_PC_KEYCODE_FUN", OPEN_PC_KEYCODE_FUN);
        Check(adds, removes, "PROTOCOL_NOENCRYPT", PROTOCOL_NOENCRYPT);
        Check(adds, removes, "USE_SDK_QRCODE", USE_SDK_QRCODE);
        Check(adds, removes, "NO_PROFILER", !HAVE_PROFILER);
        Check(adds, removes, "APP_SCORE", APP_SCORE);

        string sDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
        string[] defines = sDefines.Split(';');

        List<string> defs = new List<string>(defines.Length + adds.Count);

        for (int i = 0; i < defines.Length; ++i)
        {
            string item = defines[i];
            if (!removes.Contains(item))
            {
                defs.Add(item);
            }
        }

        for (int i = 0; i < adds.Count; ++i)
        {
            string item = adds[i];
            if (!defs.Contains(item))
            {
                defs.Add(item);
            }
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), string.Join(";", defs));
    }




}
