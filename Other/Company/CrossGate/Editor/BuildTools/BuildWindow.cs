using Lib.AssetLoader;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


//#if USE_ADDRESSABLE_ASSET
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using UnityEditor.Build.Pipeline.Utilities;
//#endif

#pragma warning disable 0414

public class BuildWindow : EditorWindow
{
    private ChannelConfigs mVersionConfigs;
    private string[] Urls;
    private int selectUrl = 0;
    private int selectProfileId = 0;

    private VersionSetting mVersionSetting;
    private BuildSetting mBuildSetting;

    private Vector2 mAssetNamePos = Vector2.zero;
    private bool lockEdit = true;
    private bool lockEditName = true;
    private bool backup = true;

    private string productName = "魔力宝贝：旅人";
    private string currentTime = string.Empty;
    private bool IsAdvanced = false;

    private bool IsAssetDetails = false;

    private string sDefine = string.Empty;

    private ReorderableList mABSettingList;

    private bool Protocol_NoEncrypt;
    private bool Debug_Mode;
    private bool Use_SDK_QRCode;
    private bool Have_Profiler;
    private bool Gm_PropagateVersion;

    //int buildToolBarIndex = 0;
    //GUIContent[] buildToolBarContents = new GUIContent[]
    //{
    //    new UnityEngine.GUIContent("发布版"),
    //    new UnityEngine.GUIContent("测试版"),
    //    new UnityEngine.GUIContent("自定义"),
    //    new UnityEngine.GUIContent("热更资源"),
    //    //new UnityEngine.GUIContent("热更Apk"),
    //};

    int toolBarIndex = 0;
    GUIContent[] toolBarContents = new GUIContent[]
    {
        new UnityEngine.GUIContent("打包"),
        new UnityEngine.GUIContent("资源配置"),
        new UnityEngine.GUIContent("内网包资源"),
        new UnityEngine.GUIContent("打包(程序)"),
    };

    [MenuItem("__Tools__/打包工具")]
    public static BuildWindow ShowWindow()
    {
        return GetWindow<BuildWindow>("Build Tool");
    }

    private void OnEnable()
    {
        GetDefineStatus();

        //===
        string versionPath = string.Format("{0}/Resources/{1}", Application.dataPath, "VersionSetting.asset");
        if (File.Exists(versionPath))
        {
            mVersionSetting = AssetDatabase.LoadAssetAtPath<VersionSetting>("Assets/Resources/VersionSetting.asset");
        }
        if (mVersionSetting == null)
        {
            Debug.Log(versionPath);
            mVersionSetting = ScriptableObject.CreateInstance<VersionSetting>();

            //mVersionSetting.eChannelType = EChannelType.InternalTest;
            mVersionSetting.eHotFixType = EHotFixMode.Normal;
            mVersionSetting.AssetVersion = "0.0.0";
            mVersionSetting.AppVersion = "0.0.0";

            AssetDatabase.CreateAsset(mVersionSetting, "Assets/Resources/VersionSetting.asset");
        }

        //===
        string versionConfigPath = string.Format("{0}/{1}", Application.dataPath, "VersionConfigs.asset");
        if (File.Exists(versionConfigPath))
        {
            mVersionConfigs = AssetDatabase.LoadAssetAtPath<ChannelConfigs>("Assets/VersionConfigs.asset");
        }
        if (mVersionConfigs == null)
        {
            Debug.Log(versionConfigPath);
            mVersionConfigs = ScriptableObject.CreateInstance<ChannelConfigs>();

            ChannelConfig config = new ChannelConfig();
            config.sId = 1;
            config.sName = "内网测试(主干)";
            config.sVersionUrl = ChannelConfigs.GetDefaultVersionUrl(1);
            config.eChannelFlags = EChannelFlags.None;
            mVersionConfigs.Urls.Add(config);

            config = new ChannelConfig();
            config.sId = 2;
            config.sName = "外网测试(测试2服 legence)";
            config.sVersionUrl = ChannelConfigs.GetDefaultVersionUrl(2);
            config.eChannelFlags = EChannelFlags.None;
            mVersionConfigs.Urls.Add(config);

            config = new ChannelConfig();
            config.sId = 3;
            config.sName = "外网测试(测试4服 edg)";
            config.sVersionUrl = ChannelConfigs.GetDefaultVersionUrl(3);
            config.eChannelFlags = EChannelFlags.None;
            mVersionConfigs.Urls.Add(config);

            config = new ChannelConfig();
            config.sId = 4;
            config.sName = "外网测试(法兰城 主干)";
            config.sVersionUrl = ChannelConfigs.GetDefaultVersionUrl(4);
            config.eChannelFlags = EChannelFlags.None;
            mVersionConfigs.Urls.Add(config);

            //config = new ChannelConfig();
            //config.sId = 3;
            //config.sName = "快手(测试 废弃)";
            //config.sVersionUrl = ChannelConfigs.GetDefaultVersionUrl(5);
            //config.eChannelFlags = EChannelFlags.None;
            //mVersionConfigs.Urls.Add(config);


            config = new ChannelConfig();
            config.sId = 5;
            config.sName = "快手(正式)";
            config.sVersionUrl = ChannelConfigs.GetDefaultVersionUrl(5);
            config.eChannelFlags = EChannelFlags.None;
            mVersionConfigs.Urls.Add(config);


            config = new ChannelConfig();
            config.sId = 6;
            config.sName = "快手(渠道)";
            config.sVersionUrl = ChannelConfigs.GetDefaultVersionUrl(6);
            config.eChannelFlags = EChannelFlags.None;
            mVersionConfigs.Urls.Add(config);

            config = new ChannelConfig();
            config.sId = 7;
            config.sName = "快手(不加密)";
            config.sVersionUrl = ChannelConfigs.GetDefaultVersionUrl(7);
            config.eChannelFlags = EChannelFlags.DisplayName;
            mVersionConfigs.Urls.Add(config);


            config = new ChannelConfig();
            config.sId = 8;
            config.sName = "快手(二维码登录)";
            config.sVersionUrl = ChannelConfigs.GetDefaultVersionUrl(8);
            config.eChannelFlags = EChannelFlags.None;
            mVersionConfigs.Urls.Add(config);

            AssetDatabase.CreateAsset(mVersionConfigs, "Assets/VersionConfigs.asset");
        }
        Urls = mVersionConfigs.GetDisplay();

        for (int i = 0; i < Urls.Length; ++i)
        {
            if (string.Equals(Urls[i], mVersionSetting.ChannelName, StringComparison.Ordinal))
            {
                selectUrl = i;
                break;
            }
        }

        //===
        string settingPath = string.Format("{0}/{1}", Application.dataPath, "DefaultBuildSetting.asset");
        if (File.Exists(settingPath))
        {
            mBuildSetting = AssetDatabase.LoadAssetAtPath<BuildSetting>("Assets/DefaultBuildSetting.asset");
        }
        if (mBuildSetting == null)
        {
            mBuildSetting = BuildSetting.CreateBuildSetting("Assets", "DefaultBuildSetting");
        }

        mBuildSetting.mBuildTarget = EditorUserBuildSettings.activeBuildTarget;

        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(mBuildSetting.mBuildTarget);
        sDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);


        //mBuildSetting.Reset();

        CreateAssetBundleSettingList();
    }

    private void OnDisable()
    {
        mVersionSetting = null;
        mBuildSetting = null;
        GC.Collect();
    }

    private void GetDefineStatus()
    {
#if DEBUG_MODE
        Debug_Mode = true;
#else
        Debug_Mode = false;
#endif

#if PROTOCOL_NOENCRYPT
        Protocol_NoEncrypt = true;
#else
        Protocol_NoEncrypt = false;
#endif


#if USE_SDK_QRCODE
        Use_SDK_QRCode = true;
#else
        Use_SDK_QRCode = false;
#endif


#if DEBUG && !NO_PROFILER 
        Have_Profiler = true;
#else
        Have_Profiler = false;
#endif

#if GM_PROPAGATE_VERSION
        Gm_PropagateVersion=true;
#else
        Gm_PropagateVersion = false;
#endif
    }


    public void Gen()
    {
        //if (mBuildSetting.bBuildApp && (string.IsNullOrWhiteSpace(mBuildSetting.sAppPublishPath) || !Directory.Exists(Path.GetFullPath(mBuildSetting.sAppPublishPath))))
        //{
        //    mBuildSetting.sAppPublishPath = Path.GetFullPath(EditorUtility.OpenFolderPanel("请选择目录", mBuildSetting.sAppPublishPath, mBuildSetting.sAppPublishPath));
        //    if (string.IsNullOrWhiteSpace(mBuildSetting.sAppPublishPath) || !Directory.Exists(Path.GetFullPath(mBuildSetting.sAppPublishPath)))
        //    {
        //        return;
        //    }
        //}

        BuildTool.Build(mBuildSetting);
    }

    private void OnGUI()
    {
        toolBarIndex = GUILayout.Toolbar(toolBarIndex, toolBarContents);

        if (toolBarIndex == 0)
        {
            OnGUI_Base();
            OnGUI_Build();
        }
        else if (toolBarIndex == 1)
        {
            OnGUI_AssetSetting();
        }
        else if (toolBarIndex == 2)
        {
            OnGUI_AddressableAssetGen();
        }
        else if (toolBarIndex == 3)
        {
            OnGUI_BaseTest();
            OnGUI_BuildTest();
        }

    }

    private void OnGUI_Base()
    {
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(mBuildSetting.mBuildTarget);

        //EditorGUILayout.HelpBox("1.选择打包平台(默认为当前切换的平台)\n2.选择打包方式\n3.填写版本号\n4.只打包资源可取消生成APP\n5.点击开始并耐心等待，完成时会有弹窗提示\n高级选项：用于逐过程配置，一般情况下不要勾选", MessageType.Info);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        mBuildSetting = (BuildSetting)EditorGUILayout.ObjectField("配置文件", mBuildSetting, typeof(BuildSetting), false, GUILayout.Width(400));
        if (GUILayout.Button("保存配置", GUILayout.Width(100)))
        {
            EditorUtility.SetDirty(mVersionSetting);
            EditorUtility.SetDirty(mBuildSetting);
            AssetDatabase.SaveAssets();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("项目路径", Application.dataPath);

        currentTime = string.Format("{0:yyyy.MMdd.HHmm}", System.DateTime.Now);
        EditorGUILayout.LabelField("当前时间", currentTime);

        EditorGUILayout.LabelField(sDefine);
        //EditorGUILayout.
        if (buildTargetGroup == BuildTargetGroup.iOS
            || buildTargetGroup == BuildTargetGroup.Android
            || buildTargetGroup == BuildTargetGroup.tvOS)
        {
            PlayerSettings.SetMobileMTRendering(buildTargetGroup, EditorGUILayout.Toggle("多线程渲染", PlayerSettings.GetMobileMTRendering(buildTargetGroup)));
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (lockEditName)
        {
#if UNITY_IOS
            if (!PlayerSettings.productName.Equals(productName))
            {
                PlayerSettings.productName = productName;
            }  
#endif
            EditorGUILayout.TextField("游戏包名", PlayerSettings.productName, GUILayout.Width(300));
        }
        else
        {
            PlayerSettings.productName = EditorGUILayout.TextField("游戏包名", PlayerSettings.productName, GUILayout.Width(300));
        }

        lockEditName = EditorGUILayout.ToggleLeft("锁定", lockEditName, GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        mBuildSetting.mBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("打包平台", mBuildSetting.mBuildTarget, GUILayout.Width(300));
        if (GUILayout.Button("当前", GUILayout.Width(50)))
        {
            mBuildSetting.mBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }
        EditorGUILayout.EndHorizontal();




        //         EditorGUILayout.BeginHorizontal();
        //         mVersionSetting.eChannelType = mBuildSetting.eChannelType = (EChannelType)EditorGUILayout.EnumPopup("选择渠道", mBuildSetting.eChannelType, GUILayout.Width(300));
        //         EditorGUILayout.EndHorizontal();        

        EditorGUILayout.BeginHorizontal();
        selectUrl = EditorGUILayout.Popup("渠道名称", selectUrl, Urls, GUILayout.Width(300));
        mVersionSetting.VersionUrl = mBuildSetting.sVersionUrl = mVersionConfigs.Urls[selectUrl].sVersionUrl;
        mVersionSetting.eChannelFlags = mVersionConfigs.Urls[selectUrl].eChannelFlags;
        mVersionSetting.ChannelName = mVersionConfigs.Urls[selectUrl].sName;
        mBuildSetting.sLastChannelId = mBuildSetting.sChannelId;
        mBuildSetting.sChannelId = mVersionConfigs.Urls[selectUrl].sId;


        string ss = mVersionSetting.eChannelFlags.ToString();
        EditorGUILayout.LabelField(ss, GUILayout.Width(500));
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //mBuildSetting.eChannelType = (EChannelType)EditorGUILayout.EnumPopup("渠道类型", mBuildSetting.eChannelType, GUILayout.Width(300));
        //mVersionSetting.eChannelType = mBuildSetting.eChannelType;
        //EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("版本文件地址", mBuildSetting.sVersionUrl, GUILayout.Width(500));
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //mVersionSetting.eHotFixType = mBuildSetting.eHotFixMode = (EHotFixMode)EditorGUILayout.EnumPopup("热更新方式", mBuildSetting.eHotFixMode, GUILayout.Width(300));
        mVersionSetting.eHotFixType = mBuildSetting.eHotFixMode = EHotFixMode.Normal;
        //EditorGUILayout.EndHorizontal();

#if UNITY_ANDROID || UNITY_IOS
        EditorGUILayout.BeginHorizontal();
        if (string.IsNullOrWhiteSpace(mBuildSetting.sVersionCode))
            mBuildSetting.sVersionCode = "0";
        mBuildSetting.sVersionCode = EditorGUILayout.TextField("App VersionCode", mBuildSetting.sVersionCode, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (string.IsNullOrWhiteSpace(mBuildSetting.sAppVersion))
        {
            mBuildSetting.sAppVersion = "0.0.0";
        }
        mVersionSetting.AppVersion = mBuildSetting.sAppVersion = EditorGUILayout.TextField("App版本号", mBuildSetting.sAppVersion, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
#endif

        EditorGUILayout.BeginHorizontal();
        if (string.IsNullOrWhiteSpace(mBuildSetting.sVersion))
        {
            mBuildSetting.sVersion = "0.0.0";
        }
        mVersionSetting.AssetVersion = mBuildSetting.sVersion = EditorGUILayout.TextField("资源版本号", mBuildSetting.sVersion, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label("——————————————【选择以下选项：应用宏编译】——————————————");

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        string useDebugStr = string.Format("调试模式：开启日志及屏幕信息   [宏:{0}]", Debug_Mode ? true : false);
        mBuildSetting.bUseDebugMode = EditorGUILayout.ToggleLeft(useDebugStr, mBuildSetting.bUseDebugMode, GUILayout.Width(300));
        //string defineStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        //string debugModeStr = string.Format("{0}[{1}]", "应用", defineStr.Contains("DEBUG_MODE") ? "Debug" : "NoDebug");
        //if (GUILayout.Button(debugModeStr, GUILayout.Width(100)))
        //{
        //    SetScriptingDefine(mBuildSetting.bUseDebugMode);
        //}
        EditorGUILayout.EndHorizontal();


#if UNITY_ANDROID || UNITY_IOS
        EditorGUILayout.BeginHorizontal();
        if (mBuildSetting.sChannelId == 7)
        {
            mBuildSetting.bUseProtocal_noEncrypt = true;
        }
        else
        {
            if (mBuildSetting.sLastChannelId == 7 && mBuildSetting.sChannelId != 7)
                mBuildSetting.bUseProtocal_noEncrypt = false;
        }
        string useProtocalStr = string.Format("协议方式：不加密   [宏:{0}]", Protocol_NoEncrypt ? true : false);
        mBuildSetting.bUseProtocal_noEncrypt = EditorGUILayout.ToggleLeft(useProtocalStr, mBuildSetting.bUseProtocal_noEncrypt, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        if (mBuildSetting.sChannelId == 8)
        {
            mBuildSetting.bUseQRcode = true;
        }
        else
        {
            if (mBuildSetting.sLastChannelId == 8 && mBuildSetting.sChannelId != 8)
                mBuildSetting.bUseQRcode = false;
        }

        string useQRcodeStr = string.Format("登录模式：二维码登录   [宏:{0}]", Use_SDK_QRCode ? true : false);
        mBuildSetting.bUseQRcode = EditorGUILayout.ToggleLeft(useQRcodeStr, mBuildSetting.bUseQRcode, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
#else
        mBuildSetting.bUseProtocal_noEncrypt = false;
        mBuildSetting.bUseQRcode = false;
#endif

        EditorGUILayout.BeginHorizontal();
        string useProfilerStr = string.Format("打包设置：连接Profiler   [宏:{0}]", Have_Profiler ? true : false);
        mBuildSetting.bBuildOptions = EditorGUILayout.ToggleLeft(useProfilerStr, mBuildSetting.bBuildOptions, GUILayout.Width(300));
        if (mBuildSetting.bBuildOptions)
        {
            mBuildSetting.mBuildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler;
        }
        else
        {
            mBuildSetting.mBuildOptions = BuildOptions.None;
        }
        EditorGUILayout.EndHorizontal();

#if UNITY_STANDALONE_WIN
        EditorGUILayout.BeginHorizontal();
        string useGm = string.Format("打包设置：港台宣发版本(gm)   [宏:{0}]", Gm_PropagateVersion ? true : false);
        mBuildSetting.bGmPropagateVersion = EditorGUILayout.ToggleLeft(useGm, mBuildSetting.bGmPropagateVersion, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
#endif

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        EditorGUILayout.BeginHorizontal();
        bool isNeedApply = (mBuildSetting.bUseDebugMode ^ Debug_Mode) ||
            (mBuildSetting.bUseProtocal_noEncrypt ^ Protocol_NoEncrypt) ||
            (mBuildSetting.bUseQRcode ^ mBuildSetting.bUseQRcode) ||
            (mBuildSetting.bBuildOptions ^ Have_Profiler) ||
            (mBuildSetting.bGmPropagateVersion ^ Gm_PropagateVersion);
        string defineStr = string.Format("1.宏应用   [{0}点击应用]", isNeedApply ? "需要" : "不需要");
        if (GUILayout.Button(defineStr, GUILayout.Width(200)))
        {
            EditorUtility.SetDirty(mVersionSetting);
            EditorUtility.SetDirty(mBuildSetting);
            AssetDatabase.SaveAssets();
            BuildTool.SetScriptingDefine(mBuildSetting);
        }
        EditorGUILayout.EndHorizontal();



        //EditorGUILayout.BeginHorizontal();
        //GetAllProfiler();
        //selectProfileId = EditorGUILayout.Popup("热更profile选择", selectUrl, Urls, GUILayout.Width(500));
        //mVersionSetting.VersionUrl = mBuildSetting.sVersionUrl = mVersionConfigs.Urls[selectUrl].sVersionUrl;
        //mVersionSetting.HotFixUrl = mBuildSetting.sHotFixUrl = mVersionConfigs.Urls[selectUrl].sHotFixUrl;
        //mVersionSetting.eChannelFlags = mVersionConfigs.Urls[selectUrl].eChannelFlags;
        //mVersionSetting.ChannelName = mVersionConfigs.Urls[selectUrl].sName;
        //EditorGUILayout.EndHorizontal();
    }


    private void SetScriptingDefine(bool bUseDebugMode)
    {
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

        List<string> adds = new List<string>();
        List<string> removes = new List<string>();
        BuildTool.CheckScriptingDefine(adds, removes, "DEBUG_MODE", bUseDebugMode);
        BuildTool.CheckScriptingDefine(adds, removes, "ADDRESSABLES_LOG_ALL", bUseDebugMode);
        BuildTool.CheckScriptingDefine(adds, removes, "DISABLE_ILRUNTIME_DEBUG", !bUseDebugMode);

        BuildTool.ApplyScriptingDefine(adds, removes, buildTargetGroup);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private void OnGUI_Build()
    {
        //buildToolBarIndex = GUILayout.Toolbar(buildToolBarIndex, buildToolBarContents);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("—————————————————— 【自行定制】 ——————————————————");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(700));
        mBuildSetting.bGenAssetBundle = EditorGUILayout.ToggleLeft("1.生成资源包", mBuildSetting.bGenAssetBundle, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
        if (mBuildSetting.bGenAssetBundle)
        {
            mBuildSetting.bClearCache = true;
            mBuildSetting.bRebuildGroup = true;
            //mBuildSetting.mBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("打包设置", mBuildSetting.mBundleOptions, GUILayout.Width(300));
            //mBuildSetting.bIncreasePacking = EditorGUILayout.Toggle("增量打包", mBuildSetting.bIncreasePacking, GUILayout.Width(600));
            //EditorGUILayout.Space();
        }

        mBuildSetting.bGenCsv = EditorGUILayout.ToggleLeft("2.生成加密CSV", mBuildSetting.bGenCsv, GUILayout.Width(150));
        mBuildSetting.bGenLogic = EditorGUILayout.ToggleLeft("3.生成加密Logic", mBuildSetting.bGenLogic, GUILayout.Width(150));
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(700));
        mBuildSetting.bBuildApp = EditorGUILayout.ToggleLeft("4.生成APP", mBuildSetting.bBuildApp, GUILayout.Width(150));

        EditorGUILayout.EndHorizontal();

        if (mBuildSetting.bBuildApp)
        {
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
            mBuildSetting.eApkMode = (ApkMode)EditorGUILayout.EnumPopup("生成模式", mBuildSetting.eApkMode, GUILayout.Width(300));
#endif

#if UNITY_STANDALONE_WIN
            mBuildSetting.eUseACEAnti = (ApkProtectModel)EditorGUILayout.EnumPopup("EXE加固模式", mBuildSetting.eUseACEAnti, GUILayout.Width(300));            
#endif
            
            mBuildSetting.eScriptingImplementation =  ScriptingImplementation.IL2CPP;
            EditorGUILayout.LabelField("打包方式", "IL2CPP");

            mBuildSetting.eRuntimeMode = ERuntimeMode.ILRuntime;
            EditorGUILayout.LabelField("运行时模式", "ILRuntime");


            mBuildSetting.sAppPublishPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "APPPublish");
#if UNITY_ANDROID
            if (mBuildSetting.eApkMode == ApkMode.UseSDK)
            {
                string appPath = string.Format("{0}\\launcher\\release", Directory.GetParent(Application.dataPath).FullName.Replace("CrossGate", "Android"));
                EditorGUILayout.LabelField("包的生成路径", appPath);
            }
            else
            {
                EditorGUILayout.LabelField("包的生成路径", mBuildSetting.sAppPublishPath);
            }
#else
            EditorGUILayout.LabelField("包的生成路径", mBuildSetting.sAppPublishPath);
#endif



            //mBuildSetting.eScriptingImplementation = (ScriptingImplementation)EditorGUILayout.EnumPopup("打包方式", mBuildSetting.eScriptingImplementation, GUILayout.Width(300));
            //mBuildSetting.eRuntimeMode = (ERuntimeMode)EditorGUILayout.EnumPopup("运行时模式", mBuildSetting.eRuntimeMode, GUILayout.Width(300));
            //GUILayout.Space(2);
            //mBuildSetting.bUseILRuntime = EditorGUILayout.Toggle("代码使用ILRunTime", mBuildSetting.bUseILRuntime || mBuildSetting.mBuildTarget == BuildTarget.iOS || mBuildSetting.eScriptingImplementation == ScriptingImplementation.IL2CPP, GUILayout.Width(300));
            //mBuildSetting.bUseProfile = EditorGUILayout.Toggle("Profile模式(不用反射直接调度程序集)", mBuildSetting.bUseProfile, GUILayout.Width(300));

            //EditorGUILayout.BeginHorizontal(GUILayout.Width(700));
            //EditorGUILayout.TextField("App生成路径", mBuildSetting.sAppPublishPath, GUILayout.Width(450));
            //if (GUILayout.Button("...", GUILayout.Width(50)))
            //{
            //    mBuildSetting.sAppPublishPath = EditorUtility.OpenFolderPanel("请选择APP目录", mBuildSetting.sAppPublishPath, "");
            //mBuildSetting.sAppPublishPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "APPPublish");
            //}
            //EditorGUILayout.EndHorizontal();
        }



        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(700));
        mBuildSetting.bGenHotFix = EditorGUILayout.ToggleLeft("5.生成热更新包", mBuildSetting.bGenHotFix, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.Space();
        EditorGUILayout.Space();




        if (GUILayout.Button("2.开始生成", GUILayout.Width(200)))
        {
            if (Application.isPlaying || EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                EditorUtility.DisplayDialog("错误", "游戏正在运行或者暂定，请不要操作！", "确定");
                return;
            }

            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("错误", "游戏脚本正在编译，请不要操作！", "确定");
                return;
            }

            if (EditorUtility.DisplayDialog("提示", "该过程可能需要较长的时间,继续请按确定", "确定", "取消"))
            {
                Gen();
                EditorUtility.DisplayDialog("生成完成", "全部定制流程生成完成", "晓得了");
            }
        }
    }






    private void OnGUI_BaseTest()
    {
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(mBuildSetting.mBuildTarget);

        //EditorGUILayout.HelpBox("1.选择打包平台(默认为当前切换的平台)\n2.选择打包方式\n3.填写版本号\n4.只打包资源可取消生成APP\n5.点击开始并耐心等待，完成时会有弹窗提示\n高级选项：用于逐过程配置，一般情况下不要勾选", MessageType.Info);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        mBuildSetting = (BuildSetting)EditorGUILayout.ObjectField("配置文件", mBuildSetting, typeof(BuildSetting), false, GUILayout.Width(400));
        if (GUILayout.Button("保存配置", GUILayout.Width(100)))
        {
            EditorUtility.SetDirty(mVersionSetting);
            EditorUtility.SetDirty(mBuildSetting);
            AssetDatabase.SaveAssets();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("项目路径", Application.dataPath);

        currentTime = string.Format("{0:yyyy.MMdd.HHmm}", System.DateTime.Now);
        EditorGUILayout.LabelField("当前时间", currentTime);

        EditorGUILayout.LabelField(sDefine);
        //EditorGUILayout.
        if (buildTargetGroup == BuildTargetGroup.iOS
            || buildTargetGroup == BuildTargetGroup.Android
            || buildTargetGroup == BuildTargetGroup.tvOS)
        {
            PlayerSettings.SetMobileMTRendering(buildTargetGroup, EditorGUILayout.Toggle("多线程渲染", PlayerSettings.GetMobileMTRendering(buildTargetGroup)));
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (lockEditName)
        {
#if UNITY_IOS
            if (!PlayerSettings.productName.Equals(productName))
            {
                PlayerSettings.productName = productName;
            }  
#endif
            EditorGUILayout.TextField("游戏包名", PlayerSettings.productName, GUILayout.Width(300));
        }
        else
        {
            PlayerSettings.productName = EditorGUILayout.TextField("游戏包名", PlayerSettings.productName, GUILayout.Width(300));
        }

        lockEditName = EditorGUILayout.ToggleLeft("锁定", lockEditName, GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        mBuildSetting.mBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("打包平台", mBuildSetting.mBuildTarget, GUILayout.Width(300));
        if (GUILayout.Button("当前", GUILayout.Width(50)))
        {
            mBuildSetting.mBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }
        EditorGUILayout.EndHorizontal();




        //         EditorGUILayout.BeginHorizontal();
        //         mVersionSetting.eChannelType = mBuildSetting.eChannelType = (EChannelType)EditorGUILayout.EnumPopup("选择渠道", mBuildSetting.eChannelType, GUILayout.Width(300));
        //         EditorGUILayout.EndHorizontal();        

        EditorGUILayout.BeginHorizontal();
        selectUrl = EditorGUILayout.Popup("渠道名称", selectUrl, Urls, GUILayout.Width(300));
        mVersionSetting.VersionUrl = mBuildSetting.sVersionUrl = mVersionConfigs.Urls[selectUrl].sVersionUrl;
        mVersionSetting.eChannelFlags = mVersionConfigs.Urls[selectUrl].eChannelFlags;
        mVersionSetting.ChannelName = mVersionConfigs.Urls[selectUrl].sName;
        string ss = mVersionSetting.eChannelFlags.ToString();
        EditorGUILayout.LabelField(ss, GUILayout.Width(500));
        EditorGUILayout.EndHorizontal();


        //EditorGUILayout.BeginHorizontal();
        //mBuildSetting.eChannelType = (EChannelType)EditorGUILayout.EnumPopup("渠道类型", mBuildSetting.eChannelType, GUILayout.Width(300));
        //mVersionSetting.eChannelType = mBuildSetting.eChannelType;
        //EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("版本文件地址", mBuildSetting.sVersionUrl, GUILayout.Width(500));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        mVersionSetting.eHotFixType = mBuildSetting.eHotFixMode = (EHotFixMode)EditorGUILayout.EnumPopup("热更新方式", mBuildSetting.eHotFixMode, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();

#if UNITY_ANDROID
        EditorGUILayout.BeginHorizontal();
        if (string.IsNullOrWhiteSpace(mBuildSetting.sVersionCode))
            mBuildSetting.sVersionCode = "0";
        mBuildSetting.sVersionCode = EditorGUILayout.TextField("App VersionCode", mBuildSetting.sVersionCode, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (string.IsNullOrWhiteSpace(mBuildSetting.sAppVersion))
        {
            mBuildSetting.sAppVersion = "0.0.0";
        }
        mVersionSetting.AppVersion = mBuildSetting.sAppVersion = EditorGUILayout.TextField("App版本号", mBuildSetting.sAppVersion, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
#endif

        EditorGUILayout.BeginHorizontal();
        if (string.IsNullOrWhiteSpace(mBuildSetting.sVersion))
        {
            mBuildSetting.sVersion = "0.0.0";
        }
        mVersionSetting.AssetVersion = mBuildSetting.sVersion = EditorGUILayout.TextField("资源版本号", mBuildSetting.sVersion, GUILayout.Width(300));
        //string minimalVersion = VersionHelper.GetMinimalVersion(mBuildSetting.sVersion);
        //if (minimalVersion == "0")
        //{
        //    mBuildSetting.bBackUpdate = backup = EditorGUILayout.ToggleLeft("备份", backup, GUILayout.Width(50));
        //}
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label("——————————————【选择以下选项：应用宏编译】——————————————");

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        EditorGUILayout.BeginHorizontal();
        string useDebugStr = string.Format("调试模式：开启日志及屏幕信息   [宏:{0}]", Debug_Mode ? true : false);
        mBuildSetting.bUseDebugMode = EditorGUILayout.ToggleLeft(useDebugStr, mBuildSetting.bUseDebugMode, GUILayout.Width(300));
        //string defineStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        //string debugModeStr = string.Format("{0}[{1}]", "应用", defineStr.Contains("DEBUG_MODE") ? "Debug" : "NoDebug");
        //if (GUILayout.Button(debugModeStr, GUILayout.Width(100)))
        //{
        //    SetScriptingDefine(mBuildSetting.bUseDebugMode);
        //}
        EditorGUILayout.EndHorizontal();


#if UNITY_ANDROID || UNITY_IOS
        EditorGUILayout.BeginHorizontal();
        if (mBuildSetting.sChannelId == 7)
        {
            mBuildSetting.bUseProtocal_noEncrypt = true;
        }
        else
        {
            if (mBuildSetting.sLastChannelId == 7 && mBuildSetting.sChannelId != 7)
                mBuildSetting.bUseProtocal_noEncrypt = false;
        }
        string useProtocalStr = string.Format("协议方式：不加密   [宏:{0}]", Protocol_NoEncrypt ? true : false);
        mBuildSetting.bUseProtocal_noEncrypt = EditorGUILayout.ToggleLeft(useProtocalStr, mBuildSetting.bUseProtocal_noEncrypt, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        if (mBuildSetting.sChannelId == 8)
        {
            mBuildSetting.bUseQRcode = true;
        }
        else
        {
            if (mBuildSetting.sLastChannelId == 8 && mBuildSetting.sChannelId != 8)
                mBuildSetting.bUseQRcode = false;
        }

        string useQRcodeStr = string.Format("登录模式：二维码登录   [宏:{0}]", Use_SDK_QRCode ? true : false);
        mBuildSetting.bUseQRcode = EditorGUILayout.ToggleLeft(useQRcodeStr, mBuildSetting.bUseQRcode, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
#else
        mBuildSetting.bUseProtocal_noEncrypt = false;
        mBuildSetting.bUseQRcode = false;
#endif

        EditorGUILayout.BeginHorizontal();
        string useProfilerStr = string.Format("打包设置：连接Profiler   [宏:{0}]", Have_Profiler ? true : false);
        mBuildSetting.bBuildOptions = EditorGUILayout.ToggleLeft(useProfilerStr, mBuildSetting.bBuildOptions, GUILayout.Width(300));
        if (mBuildSetting.bBuildOptions)
        {
            mBuildSetting.mBuildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler;
        }
        else
        {
            mBuildSetting.mBuildOptions = BuildOptions.None;
        }
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //GetAllProfiler();
        //selectProfileId = EditorGUILayout.Popup("热更profile选择", selectUrl, Urls, GUILayout.Width(500));
        //mVersionSetting.VersionUrl = mBuildSetting.sVersionUrl = mVersionConfigs.Urls[selectUrl].sVersionUrl;
        //mVersionSetting.HotFixUrl = mBuildSetting.sHotFixUrl = mVersionConfigs.Urls[selectUrl].sHotFixUrl;
        //mVersionSetting.eChannelFlags = mVersionConfigs.Urls[selectUrl].eChannelFlags;
        //mVersionSetting.ChannelName = mVersionConfigs.Urls[selectUrl].sName;
        //EditorGUILayout.EndHorizontal();
#if UNITY_STANDALONE_WIN
        EditorGUILayout.BeginHorizontal();
        string useGm = string.Format("打包设置：港台宣发版本(gm)   [宏:{0}]", Gm_PropagateVersion ? true : false);
        mBuildSetting.bGmPropagateVersion = EditorGUILayout.ToggleLeft(useGm, mBuildSetting.bGmPropagateVersion, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
#endif

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        EditorGUILayout.BeginHorizontal();
        bool isNeedApply = (mBuildSetting.bUseDebugMode ^ Debug_Mode) ||
            (mBuildSetting.bUseProtocal_noEncrypt ^ Protocol_NoEncrypt) ||
            (mBuildSetting.bUseQRcode ^ mBuildSetting.bUseQRcode) ||
            (mBuildSetting.bBuildOptions ^ Have_Profiler) ||
            (mBuildSetting.bGmPropagateVersion ^ Gm_PropagateVersion);
        string defineStr = string.Format("1.宏应用   [{0}点击应用]", isNeedApply ? "需要" : "不需要");
        if (GUILayout.Button(defineStr, GUILayout.Width(200)))
        {
            EditorUtility.SetDirty(mVersionSetting);
            EditorUtility.SetDirty(mBuildSetting);
            AssetDatabase.SaveAssets();
            BuildTool.SetScriptingDefine(mBuildSetting);
        }
        EditorGUILayout.EndHorizontal();
    }
    private void OnGUI_BuildTest()
    {
        //EditorGUILayout.BeginHorizontal();
        //mBuildSetting.bDeepProfilerOption = EditorGUILayout.ToggleLeft("打包设置：连接Deep Profiler[默认请不要勾选]", mBuildSetting.bDeepProfilerOption, GUILayout.Width(300));
        //if (mBuildSetting.bDeepProfilerOption)
        //{
        //    mBuildSetting.mBuildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.EnableDeepProfilingSupport;
        //}
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //mBuildSetting.bUseProfileEvent = EditorGUILayout.ToggleLeft("调试模式：发送事件分析器", mBuildSetting.bUseProfileEvent, GUILayout.Width(300));
        //EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("—————————————————— 【自行定制】 ——————————————————");
        EditorGUILayout.Space();
        EditorGUILayout.Space();



        EditorGUILayout.BeginHorizontal(GUILayout.Width(700));
        mBuildSetting.bGenAssetBundle = EditorGUILayout.ToggleLeft("1.生成资源包", mBuildSetting.bGenAssetBundle, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
        if (mBuildSetting.bGenAssetBundle)
        {
            mBuildSetting.bClearCache = EditorGUILayout.Toggle("清除缓存", mBuildSetting.bClearCache, GUILayout.Width(600));
            mBuildSetting.bRebuildGroup = EditorGUILayout.Toggle("重新构建分组", mBuildSetting.bRebuildGroup, GUILayout.Width(600));
            EditorGUILayout.Space();
        }

        mBuildSetting.bGenCsv = EditorGUILayout.ToggleLeft("2.生成加密CSV", mBuildSetting.bGenCsv, GUILayout.Width(150));
        mBuildSetting.bGenLogic = EditorGUILayout.ToggleLeft("3.生成加密Logic", mBuildSetting.bGenLogic, GUILayout.Width(150));
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(700));
        mBuildSetting.bBuildApp = EditorGUILayout.ToggleLeft("4.生成APP", mBuildSetting.bBuildApp, GUILayout.Width(150));

        EditorGUILayout.EndHorizontal();

        if (mBuildSetting.bBuildApp)
        {
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
            mBuildSetting.eApkMode = (ApkMode)EditorGUILayout.EnumPopup("生成模式", mBuildSetting.eApkMode, GUILayout.Width(300));
#endif

#if UNITY_STANDALONE_WIN
            mBuildSetting.eUseACEAnti = (ApkProtectModel)EditorGUILayout.EnumPopup("EXE加固模式", mBuildSetting.eUseACEAnti, GUILayout.Width(300));
#endif
            mBuildSetting.eScriptingImplementation = (ScriptingImplementation)EditorGUILayout.EnumPopup("打包方式", mBuildSetting.eScriptingImplementation, GUILayout.Width(300));
            mBuildSetting.eRuntimeMode = (ERuntimeMode)EditorGUILayout.EnumPopup("运行时模式", mBuildSetting.eRuntimeMode, GUILayout.Width(300));
            GUILayout.Space(2);

            EditorGUILayout.BeginHorizontal(GUILayout.Width(700));
            EditorGUILayout.TextField("App生成路径", mBuildSetting.sAppPublishPath, GUILayout.Width(450));
            if (GUILayout.Button("...", GUILayout.Width(50)))
            {
                mBuildSetting.sAppPublishPath = EditorUtility.OpenFolderPanel("请选择APP目录", mBuildSetting.sAppPublishPath, "");
            }
            EditorGUILayout.EndHorizontal();
        }



        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(700));
        mBuildSetting.bGenHotFix = EditorGUILayout.ToggleLeft("5.生成热更新包", mBuildSetting.bGenHotFix, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        if (GUILayout.Button("2.开始生成", GUILayout.Width(200)))
        {
            if (Application.isPlaying || EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                EditorUtility.DisplayDialog("错误", "游戏正在运行或者暂定，请不要操作！", "确定");
                return;
            }

            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("错误", "游戏脚本正在编译，请不要操作！", "确定");
                return;
            }

            if (EditorUtility.DisplayDialog("提示", "该过程可能需要较长的时间,继续请按确定", "确定", "取消"))
            {
                Gen();
                EditorUtility.DisplayDialog("生成完成", "全部定制流程生成完成", "晓得了");
            }
        }
    }


    private void OnGUI_AssetSetting()
    {
        mAssetNamePos = EditorGUILayout.BeginScrollView(mAssetNamePos);
        mABSettingList.DoLayoutList();
        EditorGUILayout.BeginHorizontal(GUILayout.Width(600));
        if (GUILayout.Button("生成 AaGroup(分析+修正)", GUILayout.Width(180)))
        {
            int noSelectedCount = 0;
            foreach (var item in mBuildSetting.mAssetBundleSettingCells)
            {
                if (item.bSelected == false)
                    noSelectedCount++;
            }

            if (noSelectedCount == mBuildSetting.mAssetBundleSettingCells.Count)
            {
                Debug.LogError("资源配置栏中的搜集相关资源没有勾选");
            }
            else
            {
                BuildTool.GenLocalAssetGroup_Addressable(mBuildSetting, true);
            }
            GUIUtility.ExitGUI();
        }

        if (GUILayout.Button("生成 AaGroup(No 分析+修正)", GUILayout.Width(180)))
        {
            int noSelectedCount = 0;
            foreach (var item in mBuildSetting.mAssetBundleSettingCells)
            {
                if (item.bSelected == false)
                    noSelectedCount++;
            }

            if (noSelectedCount == mBuildSetting.mAssetBundleSettingCells.Count)
            {
                Debug.LogError("资源配置栏中的搜集相关资源没有勾选");
            }
            else
            {
                BuildTool.GenLocalAssetGroup_Addressable(mBuildSetting, false);
            }
            GUIUtility.ExitGUI();
        }


        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }


    bool bIncreasePacking = false;
    private void OnGUI_AddressableAssetGen()
    {
        EditorGUILayout.Space();

        currentTime = string.Format("{0:yyyy.MMdd.HHmm}", System.DateTime.Now);
        EditorGUILayout.LabelField("当前时间", currentTime);

        DirectoryInfo info = new DirectoryInfo(Application.dataPath);
        string papaPath = info.Parent.Parent.FullName.Replace('\\', '/');
        string mLocalBuildPath = string.Format("{0}/{1}/{2}", papaPath, "AddressableBundle", PlatformMappingService.GetPlatformPathSubFolder());
        EditorGUILayout.LabelField("生成Addressable资源路径：", mLocalBuildPath);

        EditorGUILayout.LabelField("Addressable资源打包：", "不勾选增量打包，会清除缓存");
        bIncreasePacking = EditorGUILayout.Toggle("增量打包", bIncreasePacking, GUILayout.Width(600));
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(600));
        if (GUILayout.Button("生成 Addressable 资源", GUILayout.Width(160)))
        {
            if (Application.isPlaying || EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                EditorUtility.DisplayDialog("错误", "游戏正在运行或者暂定，请不要操作！", "确定");
                return;
            }

            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("错误", "游戏脚本正在编译，请不要操作！", "确定");
                return;
            }

            if (EditorUtility.DisplayDialog("提示", "该过程可能需要较长的时间,继续请按确定", "确定", "取消"))
            {
                GenLocalBundle(mLocalBuildPath);
                EditorUtility.DisplayDialog("生成完成", "全部定制流程生成完成", "晓得了");
            }
        }

        if (GUILayout.Button("打开Bundle生成路径", GUILayout.Width(160)))
        {
            string path = string.Format("{0}/../../AddressableBundle", Application.dataPath);
            Application.OpenURL(path);
        }
        EditorGUILayout.EndHorizontal();

    }


    private void GenLocalBundle(string mLocalBuildPath)
    {
        string path = mLocalBuildPath.Substring(0, mLocalBuildPath.LastIndexOf("/"));
        string versionPath = string.Format("{0}/Record.txt", path);
        if (File.Exists(versionPath))
            File.Delete(versionPath);

        //1.要不要清除缓存，增量打包可以节省时间提高效率，但是缓存没清可能会导致bug
        if (!bIncreasePacking)
        {
            AddressableAssetSettings.CleanPlayerContent(null);
            BuildCache.PurgeCache(false);
        }

        //2.分析材质球，设置图集的状态，打包所有图集
        BuildTool.StartBuild(EditorUserBuildSettings.activeBuildTarget, true, false);

        //3.先设置打包的本地加载和打包路径
        if (Directory.Exists(mLocalBuildPath))
            Directory.Delete(mLocalBuildPath, true);
        Directory.CreateDirectory(mLocalBuildPath);
        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        string LocalBuildPathStr = "[UnityEngine.AddressableAssets.PathRebuild.LocalBuildPath]/[BuildTarget]";
        m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kLocalBuildPath, LocalBuildPathStr);
        string LocalLoadPathStr = "{UnityEngine.AddressableAssets.PathRebuild.LocalLoadPath}/[BuildTarget]";
        m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kLocalLoadPath, LocalLoadPathStr);

        //4.进行打包(重新构建分组 + 打包)
        //AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex = 3;
        BuildTool.GenLocalAsset_Addressable(mBuildSetting);
        AssetDatabase.Refresh();


        //5.打包完进行 文件夹的copy （setting.json ,Catalog.json 拷贝到 工程外AddressableBundle资源路径）
        string souceFileCatalog = string.Format("{0}/{1}/{2}/{3}/catalog.json", Application.dataPath, BuildTool.AddressableOriPath, AssetPath.sAddressableDir, PlatformMappingService.GetPlatformPathSubFolder());
        if (!File.Exists(souceFileCatalog))
        {
            Debug.LogError("不存在文件：" + souceFileCatalog);
            return;
        }
        string desFileCatalog = string.Format("{0}/catalog.json", mLocalBuildPath);
        File.Copy(souceFileCatalog, desFileCatalog, true);


        string souceFileSetting = string.Format("{0}/{1}/{2}/{3}/settings.json", Application.dataPath, BuildTool.AddressableOriPath, AssetPath.sAddressableDir, PlatformMappingService.GetPlatformPathSubFolder());
        if (!File.Exists(souceFileSetting))
        {
            Debug.LogError("不存在文件：" + souceFileSetting);
            return;
        }
        string desFileSetting = string.Format("{0}/settings.json", mLocalBuildPath);
        string readSettingContent = File.ReadAllText(souceFileSetting);
        readSettingContent = readSettingContent.Replace("{UnityEngine.AddressableAssets.Addressables.RuntimePath}", "{UnityEngine.AddressableAssets.PathRebuild.LocalLoadPath}");
        File.WriteAllText(desFileSetting, readSettingContent);
        AssetDatabase.Refresh();

        //打包成功记录标识
        File.WriteAllText(versionPath, "success");

        //6.停止构建
        BuildTool.EndBuild(EditorUserBuildSettings.activeBuildTarget, true, true);
    }



    public void SetHotFixVersion(string sVersion, BuildTarget mBuildTarget)
    {
        //已经自加1
        string version = BuildTool.GetMinimalVersionByVersion(sVersion, mBuildTarget, mVersionSetting, mBuildSetting);
    }


    private void CreateAssetBundleSettingList()
    {
        mABSettingList = new ReorderableList(mBuildSetting.mAssetBundleSettingCells, typeof(AssetBundleSettingCell));
        mABSettingList.drawElementCallback = OnDrawABSetting;
        mABSettingList.onAddCallback = OnABSetting_Add;
        mABSettingList.onRemoveCallback = OnABSetting_Remove;
        mABSettingList.drawHeaderCallback = OnDrawABSettingHeader;
        mABSettingList.elementHeight = 28;

        //mABSettingList.showDefaultBackground = true;
    }

    private void OnDrawABSettingHeader(Rect rect)
    {
        Rect rect6 = new Rect(rect.x, rect.y, 35, rect.height);

        Rect rect0 = new Rect(rect.x + 40, rect.y, 35, rect.height);
        Rect rect1 = new Rect(rect.x + 120, rect.y, 200, rect.height);
        Rect rect2 = new Rect(rect.x + 340, rect.y, 200, rect.height);
        Rect rect3 = new Rect(rect.x + 600, rect.y, 100, rect.height);
        Rect rect4 = new Rect(rect.x + 750, rect.y, 200, rect.height);
        Rect rect5 = new Rect(rect.x + 900, rect.y, 300, rect.height);

        if (GUI.Button(rect6, lockEdit ? "编辑" : "锁定"))
        {
            lockEdit = !lockEdit;
        }

        if (GUI.Button(rect0, "全选"))
        {
            bool isAllSelected = true;
            for (int i = 0; i < mBuildSetting.mAssetBundleSettingCells.Count; ++i)
            {
                AssetBundleSettingCell settingCell = mBuildSetting.mAssetBundleSettingCells[i];
                if (!settingCell.bSelected)
                {
                    isAllSelected = false;
                    break;
                }
            }

            for (int i = 0; i < mBuildSetting.mAssetBundleSettingCells.Count; ++i)
            {
                AssetBundleSettingCell settingCell = mBuildSetting.mAssetBundleSettingCells[i];
                settingCell.bSelected = !isAllSelected;
            }
        }

        GUI.Label(rect1, "文件路径");
        GUI.Label(rect2, "文件过滤");
        GUI.Label(rect3, "引用过滤");
        GUI.Label(rect4, "统一资源包名");
        GUI.Label(rect5, "查找类型");
    }

    private void OnDrawABSetting(Rect rect, int index, bool isActive, bool isFocused)
    {
        AssetBundleSettingCell cell = mBuildSetting.mAssetBundleSettingCells[index];

        Rect rect0 = new Rect(rect.x + 40, rect.y + 4, 50, rect.height - 8);
        Rect rect1 = new Rect(rect.x + 100, rect.y + 4, 200, rect.height - 8);
        Rect rect2 = new Rect(rect.x + 320, rect.y + 4, 250, rect.height - 8);
        Rect rect3 = new Rect(rect.x + 600, rect.y + 4, 150, rect.height - 8);
        Rect rect4 = new Rect(rect.x + 750, rect.y + 4, 200, rect.height - 8);
        Rect rect5 = new Rect(rect.x + 900, rect.y + 4, 300, rect.height - 8);

        cell.bSelected = GUI.Toggle(rect0, cell.bSelected, "");
        if (!lockEdit)
        {
            cell.sRelativePath = GUI.TextField(rect1, cell.sRelativePath);
            cell.sFilter = GUI.TextField(rect2, cell.sFilter);
            cell.sDependencyFilter = GUI.TextField(rect3, cell.sDependencyFilter);
            cell.sOverrideName = GUI.TextField(rect4, cell.sOverrideName);
            cell.eSearchOption = (SearchOption)EditorGUI.EnumPopup(rect5, cell.eSearchOption);
        }
        else
        {
            GUI.Label(rect1, cell.sRelativePath);
            GUI.Label(rect2, cell.sFilter);
            GUI.Label(rect3, cell.sDependencyFilter);
            GUI.Label(rect4, cell.sOverrideName);
            GUI.Label(rect5, cell.eSearchOption.ToString());
        }
    }

    private void OnABSetting_Add(ReorderableList list)
    {
        if (lockEdit)
        {
            return;
        }
    }

    private void OnABSetting_Remove(ReorderableList list)
    {
        if (lockEdit)
        {
            return;
        }
    }
}