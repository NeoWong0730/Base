
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif
public class PostprocessBuild_iOS
{
    /// <summary>
    /// 构建前置处理-IOS
    /// </summary>
    /// <param name="path"></param>
    public static void OnPreprocessBuild_iOS(string path)
    {
        Debug.Log("OnPreprocessBuild_iOS: " + path);
    }




    /// <summary>
    /// 构建后置处理-IOS
    /// </summary>
    /// <param name="path"></param>
    public static void OnPostprocessBuild_iOS(string path)
    {
        Debug.Log("OnPostprocessBuild_iOS: " + path);

#if UNITY_IOS
        //导入资源过程
        string dstAaPath = string.Format("{0}/{1}/{2}", path, "Data", "Raw");
        string srcAaPath = string.Format("{0}/{1}/{2}", Application.dataPath, "../AssetBundle", Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget));
        FileTool.CopyAndReplaceDirectory(srcAaPath, dstAaPath);


        string projpath = PBXProject.GetPBXProjectPath(path);
        PBXProject project = new PBXProject();
        project.ReadFromString(File.ReadAllText(projpath));
      
        string targetGUID = project.GetUnityMainTargetGuid();
        string frameworkGuid = project.GetUnityFrameworkTargetGuid();

        ////添加本地依赖库
        //FileTool.CopyAndReplaceDirectory(Application.dataPath + "/../Sdk/IOS/Third", Path.Combine(path, "Third"));

        ////添加推送文件
        //FileTool.CopyAndReplaceDirectory(Application.dataPath + "/../Sdk/IOS/Sdk/NotificationService", Path.Combine(path, "NotificationService"));

        ////修改属性
        //project.AddBuildProperty(frameworkGuid, "OTHER_LDFLAGS", "-ObjC");
        project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "$(inherited)");
        project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");
        project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-Wl,-stack_size,0x10000000");

        //解决ios系统12.2以下的手机崩溃，缺少激活内嵌的swift标准库
        project.SetBuildProperty(targetGUID, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");


        //XCode can not build UnityWebRequest.o
        project.AddBuildProperty(frameworkGuid, "GCC_PREPROCESSOR_DEFINITIONS", "$(inherited)");
        project.AddBuildProperty(frameworkGuid, "GCC_PREPROCESSOR_DEFINITIONS", "COCOAPODS=1");
        project.AddBuildProperty(frameworkGuid, "GCC_PREPROCESSOR_DEFINITIONS", "IL2CPP_LARGE_EXECUTABLE_ARM_WORKAROUND=1");


        project.AddBuildProperty(frameworkGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
        project.AddBuildProperty(frameworkGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
        project.AddBuildProperty(frameworkGuid, "LD_RUNPATH_SEARCH_PATHS", "@loader_path/Frameworks");


        ////修改bitcode
        //project.SetBuildProperty(targetGUID, "ENABLE_BITCODE", "NO");
        //project.SetBuildProperty(frameworkGuid, "ENABLE_BITCODE", "NO");

        //修改xcode的ProductName
        project.SetBuildProperty(targetGUID, "PRODUCT_NAME", "CrossGate");

#region 系统默认带的 14个
        //project.AddFrameworkToProject(frameworkGuid, "AudioToolbox.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "AVFoundation.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CFNetwork.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CoreGraphics.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CoreMedia.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CoreMotion.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CoreVideo.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CoreText.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "Foundation.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "OpenAL.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "OpenGLES.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "QuartzCore.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "SystemConfiguration.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "UIKit.framework", false);
        ////Framework dependencies
        //project.AddFrameworkToProject(frameworkGuid, "AdSupport.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CoreData.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CoreFoundation.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CoreLocation.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CoreTelephony.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "MobileCoreServices.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "Security.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "StoreKit.framework", false);
        ////Rarely Used frameworks
        //project.AddFrameworkToProject(frameworkGuid, "Accelerate.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "AddressBook.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "Contacts.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "GLKit.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "ImageIO.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "JavaScriptCore.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "LocalAuthentication.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "MediaPlayer.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "MediaToolbox.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "MessageUI.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "Photos.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "QuickLook.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "UserNotifications.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "WebKit.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "CoreAudio.framework", false);
#endregion

        //other frameworks
        project.AddFrameworkToProject(frameworkGuid, "CoreServices.framework", false);
        project.AddFrameworkToProject(frameworkGuid, "iAd.framework", false);
        project.AddFrameworkToProject(frameworkGuid, "Contacts.framework", false);
        project.AddFrameworkToProject(frameworkGuid, "VideoToolbox.framework", false);
        project.AddFrameworkToProject(frameworkGuid, "AdServices.framework", true);
        ////klink 需要依赖
        //project.AddFrameworkToProject(frameworkGuid, "MetalKit.framework", false);
        //project.AddFrameworkToProject(frameworkGuid, "MetalPerformanceShaders.framework", false);


        ////添加tbd
        //AddLibToProject(project, frameworkGuid, "libbz2.tbd");
        //AddLibToProject(project, frameworkGuid, "libc++.tbd");
        //AddLibToProject(project, frameworkGuid, "libc++abi.tbd");
        //AddLibToProject(project, frameworkGuid, "libiconv.tbd");
        //AddLibToProject(project, frameworkGuid, "libresolv.tbd");
        //AddLibToProject(project, frameworkGuid, "libresolv.9.tbd");
        //AddLibToProject(project, frameworkGuid, "libxml2.tbd");
        //AddLibToProject(project, frameworkGuid, "libz.tbd");
        //AddLibToProject(project, frameworkGuid, "libsqlite3.tbd");

        ////添加stdc++
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/libstdc++/libstdc++.tbd", "Third/libstdc++/libstdc++.tbd", PBXSourceTree.Source));
        //添加第三方framework bundle

        //添加内嵌klink.framework
        //string fileguid = project.AddFile("Third/KwaiGameSDK/klink.framework", "Third/KwaiGameSDK/klink.framework");
        //project.AddFileToEmbedFrameworks(targetGUID, fileguid);

        //接入热云需要把frameworkGuid 放入到 targetGUID，即使frameworkGuid已经内嵌，sdk要求的，至于什么原因不懂！
        //project.AddFrameworkToProject(targetGUID, frameworkGuid, false);

        ////mtp 动态库添加
        //string mtpLibGuid = project.FindFileGuidByProjectPath("Frameworks/Plugins/iOS/" + "tersafe2.framework");
        //project.AddFileToEmbedFrameworks(targetGUID, mtpLibGuid);
        //project.AddBuildProperty(targetGUID, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");

        //GME 语音动态库添加
        //{
        //    string[] framework_names = {
        //        "libgmefdkaac.framework",
        //        "libgmelamemp3.framework",
        //        "libgmeogg.framework",
        //        "libgmesoundtouch.framework"
        //    };

        //    for (int i = 0; i < framework_names.Length; i++)
        //    {
        //        string framework_name = framework_names[i];
        //        string dylibGuid = null;
        //        dylibGuid = project.FindFileGuidByProjectPath("Frameworks/Plugins/iOS/" + framework_name);

        //        if (dylibGuid == null)
        //        {
        //            UnityEngine.Debug.LogWarning(framework_name + " guid not found");
        //        }
        //        else
        //        {
        //            UnityEngine.Debug.LogWarning(framework_name + " guid:" + dylibGuid);
        //            PBXProjectExtensions.AddFileToEmbedFrameworks(project, targetGUID, dylibGuid);
        //            project.AddBuildProperty(targetGUID, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
        //        }
        //    }
        //}

        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-CrashReport-Bugly.framework", "Third/KwaiGameSDK/KwaiGameSDK-CrashReport-Bugly.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-OpenPlatform.framework", "Third/KwaiGameSDK/KwaiGameSDK-OpenPlatform.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-Pay.framework", "Third/KwaiGameSDK/KwaiGameSDK-Pay.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-QRScan.framework", "Third/KwaiGameSDK/KwaiGameSDK-QRScan.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-QuickLogin.framework", "Third/KwaiGameSDK/KwaiGameSDK-QuickLogin.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-VisitorPasswordLogin.framework", "Third/KwaiGameSDK/KwaiGameSDK-VisitorPasswordLogin.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-Report-Thinking.framework", "Third/KwaiGameSDK/KwaiGameSDK-Report-Thinking.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-WeaponArmy.framework", "Third/KwaiGameSDK/KwaiGameSDK-WeaponArmy.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK.framework", "Third/KwaiGameSDK/KwaiGameSDK.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/MainFrameworks_dependency.framework", "Third/KwaiGameSDK/MainFrameworks_dependency.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-ADPath-Reyun.framework", "Third/KwaiGameSDK/KwaiGameSDK-ADPath-Reyun.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/Guardian.framework", "Third/KwaiGameSDK/Guardian.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-Report-Sensors.framework", "Third/KwaiGameSDK/KwaiGameSDK-Report-Sensors.framework", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-ADPath-ASA.framework", "Third/KwaiGameSDK/KwaiGameSDK-ADPath-ASA.framework", PBXSourceTree.Source));

        ////删除项目中的CustomLogo.bundle --去品牌
        ////project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/CustomLogo.bundle", "Third/KwaiGameSDK/CustomLogo.bundle", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/AzerothResources.bundle", "Third/KwaiGameSDK/AzerothResources.bundle", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KSCommon.bundle", "Third/KwaiGameSDK/KSCommon.bundle", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KSKanas.bundle", "Third/KwaiGameSDK/KSKanas.bundle", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KSSecurityZTCore.bundle", "Third/KwaiGameSDK/KSSecurityZTCore.bundle", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameExternalWebViewAssets.bundle", "Third/KwaiGameSDK/KwaiGameExternalWebViewAssets.bundle", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameMockResource.bundle", "Third/KwaiGameSDK/KwaiGameMockResource.bundle", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/KwaiGameXCAssets.bundle", "Third/KwaiGameSDK/KwaiGameXCAssets.bundle", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/SensorsAnalyticsSDK.bundle", "Third/KwaiGameSDK/SensorsAnalyticsSDK.bundle", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/WeiboSDK.bundle", "Third/KwaiGameSDK/WeiboSDK.bundle", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/Yoda.bundle", "Third/KwaiGameSDK/Yoda.bundle", PBXSourceTree.Source));


        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/v1.json", "Third/KwaiGameSDK/v1.json", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/v2.json", "Third/KwaiGameSDK/v2.json", PBXSourceTree.Source));
        //project.AddFileToBuild(frameworkGuid, project.AddFile("Third/KwaiGameSDK/v3.json", "Third/KwaiGameSDK/v3.json", PBXSourceTree.Source));
        ////project.AddFileToBuild(frameworkGuid, project.AddFile("Third/Bugly/Bugly.framework", "Third/Bugly/Bugly.framework", PBXSourceTree.Source));

        ////添加Framework Search Paths、Header Search Paths、Library Search Paths
        //project.AddBuildProperty(frameworkGuid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Third");
        //project.AddBuildProperty(frameworkGuid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Third/KwaiGameSDK");
        //project.AddBuildProperty(frameworkGuid, "LIBRARY_SEARCH_PATHS", "$(PROJECT_DIR)/Third");
        //project.AddBuildProperty(frameworkGuid, "HEADER_SEARCH_PATHS", "xxxx");

        ////添加appdomain
        //string entitlementsFileName = "CrossGate.entitlements";
        //string targetName = "Unity-iPhone";
        //bool success = project.AddCapability(targetGUID,PBXCapabilityType.AssociatedDomains, entitlementsFileName);
        //var entitlements = new ProjectCapabilityManager(projpath, entitlementsFileName, targetName, targetGUID);
        //entitlements.AddAssociatedDomains(new string[] { "applinks:linkmlbb.gamekuaishou.com" });
        //entitlements.AddSignInWithApple();
        //entitlements.AddPushNotifications(true); //添加Capabilities-推送
        //entitlements.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
        //entitlements.WriteToFile();

        #region 自动添加推送
        //string pushTarget = project.AddAppExtension(targetGUID, "NotificationService", "com.crossgate.kuaishou.ios.NotificationService", "NotificationService/Info.plist");// path + "/NotificationService/Info.plist"
        //project.AddBuildProperty(pushTarget, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Third");
        //project.AddBuildProperty(pushTarget, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Third/KwaiGameSDK");
        //project.AddBuildProperty(pushTarget, "LIBRARY_SEARCH_PATHS", "$(PROJECT_DIR)/Third");
        //project.AddBuildProperty(pushTarget, "HEADER_SEARCH_PATHS", "$(PROJECT_DIR)/NotificationService");

        //project.AddFileToBuild(pushTarget, project.AddFile("Third/KwaiGameSDK/NotificationService_dependency.framework", "Third/KwaiGameSDK/NotificationService_dependency.framework"));
        //project.AddFileToBuild(pushTarget, project.AddFile("Third/KwaiGameSDK/KwaiGameSDK-PushReport.framework", "Third/KwaiGameSDK/KwaiGameSDK-PushReport.framework"));  

        //project.AddFileToBuild(pushTarget, project.AddFile(path + "/NotificationService/NotificationService.h", "NotificationService/NotificationService.h"));
        //project.AddFileToBuild(pushTarget, project.AddFile(path + "/NotificationService/NotificationService.m", "NotificationService/NotificationService.m"));
        //project.AddFile(path + "/NotificationService/NotificationService.entitlements", "NotificationService/NotificationService.entitlements");
        //project.AddFile(path + "/NotificationService/Info.plist", "NotificationService/Info.plist");

        ////推送设置
        //project.SetBuildProperty(pushTarget, "ENABLE_BITCODE", "NO");
        //project.SetBuildProperty(pushTarget, "ARCHS", "arm64");
        //project.SetBuildProperty(pushTarget, "IPHONEOS_DEPLOYMENT_TARGET", "10.0");
        //project.SetBuildProperty(pushTarget, "TARGETED_DEVICE_FAMILY", "1,2");
        //project.SetBuildProperty(pushTarget, "CURRENT_PROJECT_VERSION", "1.0"); 
        //project.SetBuildProperty(pushTarget, "DEVELOPMENT_TEAM", "355K9264BZ");
        //project.SetBuildProperty(pushTarget, "INFOPLIST_KEY_CFBundleDisplayName", "NotificationService");
        //project.SetBuildProperty(pushTarget, "MARKETING_VERSION", "1.0");
        ////这些是为了和手动创建的推送匹配，为了不必要的错误，因为设置
        //project.SetBuildProperty(pushTarget, "ALWAYS_SEARCH_USER_PATHS", "NO");
        //project.SetBuildProperty(pushTarget, "CLANG_ANALYZER_NONNULL", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_ANALYZER_NUMBER_OBJECT_CONVERSION", "YES_AGGRESSIVE");//CURRENT_PROJECT_VERSION = 1.0
        //project.SetBuildProperty(pushTarget, "CLANG_CXX_LANGUAGE_STANDARD", "gnu++17");  //
        //project.SetBuildProperty(pushTarget, "CLANG_ENABLE_MODULES", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_ENABLE_OBJC_ARC", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_ENABLE_OBJC_WEAK", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_BLOCK_CAPTURE_AUTORELEASING", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_BOOL_CONVERSION", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_COMMA", "YES");//CURRENT_PROJECT_VERSION = 1.0
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_CONSTANT_CONVERSION", "YES");  //
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_DEPRECATED_OBJC_IMPLEMENTATIONS", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_DIRECT_OBJC_ISA_USAGE", "YES_ERROR");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_DOCUMENTATION_COMMENTS", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_EMPTY_BODY", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_ENUM_CONVERSION", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_INFINITE_RECURSION", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_INT_CONVERSION", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_NON_LITERAL_NULL_CONVERSION", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_OBJC_IMPLICIT_RETAIN_SELF", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_OBJC_LITERAL_CONVERSION", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_OBJC_ROOT_CLASS", "YES_ERROR");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_QUOTED_INCLUDE_IN_FRAMEWORK_HEADER", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_RANGE_LOOP_ANALYSIS", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_STRICT_PROTOTYPES", "YES");  //
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_SUSPICIOUS_MOVE", "YES");  //
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_UNGUARDED_AVAILABILITY", "YES_AGGRESSIVE");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN_UNREACHABLE_CODE", "YES");
        //project.SetBuildProperty(pushTarget, "CLANG_WARN__DUPLICATE_METHOD_MATCH", "YES");

        //project.SetBuildProperty(pushTarget, "COPY_PHASE_STRIP", "NO");
        //project.SetBuildProperty(pushTarget, "ENABLE_NS_ASSERTIONS", "NO");
        //project.SetBuildProperty(pushTarget, "ENABLE_STRICT_OBJC_MSGSEND", "YES");
        //project.SetBuildProperty(pushTarget, "GCC_C_LANGUAGE_STANDARD", "gnu11");
        //project.SetBuildProperty(pushTarget, "GCC_NO_COMMON_BLOCKS", "YES");
        //project.SetBuildProperty(pushTarget, "GCC_WARN_64_TO_32_BIT_CONVERSION", "YES");
        //project.SetBuildProperty(pushTarget, "GCC_WARN_ABOUT_RETURN_TYPE", "YES_ERROR");
        //project.SetBuildProperty(pushTarget, "GCC_WARN_UNDECLARED_SELECTOR", "YES");
        //project.SetBuildProperty(pushTarget, "GCC_WARN_UNINITIALIZED_AUTOS", "YES_AGGRESSIVE");
        //project.SetBuildProperty(pushTarget, "GCC_WARN_UNUSED_FUNCTION", "YES");
        //project.SetBuildProperty(pushTarget, "GENERATE_INFOPLIST_FILE", "YES");
        //project.SetBuildProperty(pushTarget, "MTL_ENABLE_DEBUG_INFO", "NO");
        //project.SetBuildProperty(pushTarget, "MTL_FAST_MATH", "YES");
        //project.SetBuildProperty(pushTarget, "VALIDATE_PRODUCT", "YES");

        //string entitlementsFileName2 = "NotificationService/NotificationService.entitlements";
        ////关闭自动证书管理
        //project.SetBuildProperty(pushTarget, "CODE_SIGN_STYLE", "Manual");
        //// 修改Code Signing Entitlements为上级目录的NotificationService.entitlements文件
        //project.SetBuildProperty(pushTarget, "CODE_SIGN_ENTITLEMENTS", entitlementsFileName2);
        //// 修改Provisioning Profile
        //project.SetBuildProperty(pushTarget, "PROVISIONING_PROFILE_SPECIFIER", "MOLI_NotificationService_dev_20230629");
        //// 修改Development Team
        ////project.SetBuildProperty(pushTarget, "DEVELOPMENT_TEAM", "SWQL42468W");
        //// 修改Code Signing Identity
        ////var codeSignIdentity = Debug.isDebugBuild ? "iPhone Developer: Alex Shen (VZ46PPHW64)" : "iPhone Distribution: Efun Japan Ltd. (SWQL42468W)";
        //project.SetBuildProperty(pushTarget, "CODE_SIGN_IDENTITY", "iPhone Developer");
        //project.SetBuildProperty(pushTarget, "CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Developer");
        //project.SetBuildProperty(pushTarget, "DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");

        //这边不用在添加推送capability
        //project.AddCapability(pushTarget, PBXCapabilityType.PushNotifications, entitlementsFileName2);
        //project.AddFrameworkToProject(pushTarget, "NotificationCenter.framework", true);

        #endregion


        project.WriteToFile(projpath);

        //设置infoplist中的key
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        PlistElementDict rootDict = plist.root;
        plist.root.SetString("NSUserTrackingUsageDescription", "获取IDFA标记权限向您提供个性化的服务及内容广告；开启后，您也可以前往系统“设置-隐私-跟踪 ”中随时关闭");
        //plist.root.SetString("NSPhotoLibraryAddUsageDescription", "需要保存图片到相册，才能进行分享");
        //plist.root.SetString("NSPhotoLibraryUsageDescription", "需要相册权限");
        //plist.root.SetString("NSCalendarsUsageDescription", "需要日历权限");
        //plist.root.SetString("NSMicrophoneUsageDescription", "录音需要麦克风权限");
        //plist.root.SetString("NSCameraUsageDescription", "需要获取摄像头权限，以使用拍照和视频录制功能");
        plist.root.SetString("NSLocationWhenInUseUsageDescription", "需要定位权限");
        plist.root.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", "需要定位权限");
        plist.root.SetString("NSAppleMusicUsageDescription", "需要获得音乐播放权限");
        //苹果AppStore提审时的出口合规信息
        plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);

        ////设置appid
        //PlistElementDict elementDict_KawiGame = rootDict.CreateDict("KwaiGameSDKDirectLoad");
        //elementDict_KawiGame.SetString("KwaiAppId", "ks695524668676586328");
        ////设置buglykey
        //PlistElementDict elementDict_bugly = elementDict_KawiGame.CreateDict("Bugly");
        //elementDict_bugly.SetString("AppId", "924e67b13a");
        ////设置reyun
        //PlistElementDict elementDict_reyun = elementDict_KawiGame.CreateDict("Reyun");
        //elementDict_reyun.SetString("AppKey", "c0c8209508acc739fc5d5aea869240f4");
        ////设置数数
        //PlistElementDict elementDict_shushu = elementDict_KawiGame.CreateDict("Thingking");
        //elementDict_shushu.SetString("AppId", "202de933c8a8443ab18d34d3cacb13c6");
        ////设置神策
        //PlistElementDict elementDict_shence = elementDict_KawiGame.CreateDict("Sensors");
        //elementDict_shence.SetString("AppKey", "ks_1");

        // URL schemes 追加
        //PlistElementArray elementArray = rootDict.CreateArray("CFBundleURLTypes");
        //PlistElementDict elementDict1 = elementArray.AddDict();
        //elementDict1.SetString("CFBundleTypeRole", "Editor");
        //elementDict1.SetString("CFBundleURLName", "kuaishou");
        //PlistElementArray urlScheme1 = elementDict1.CreateArray("CFBundleURLSchemes");
        //urlScheme1.AddString("ks695524668676586328");

        //PlistElementDict elementDict2 = elementArray.AddDict();
        //elementDict2.SetString("CFBundleTypeRole", "Editor");
        //elementDict2.SetString("CFBundleURLName", "weixin");
        //PlistElementArray urlScheme2 = elementDict2.CreateArray("CFBundleURLSchemes");
        //urlScheme2.AddString("wxd92a17eb264e3070");

        //PlistElementDict elementDict3 = elementArray.AddDict();
        //elementDict3.SetString("CFBundleTypeRole", "Editor");
        //elementDict3.SetString("CFBundleURLName", "qq");
        //PlistElementArray urlScheme3 = elementDict3.CreateArray("CFBundleURLSchemes");
        //urlScheme3.AddString("tencent101937219");

        //PlistElementDict elementDict4 = elementArray.AddDict();
        //elementDict4.SetString("CFBundleTypeRole", "Editor");
        //elementDict4.SetString("CFBundleURLName", "SensorsData.production");
        //PlistElementArray urlScheme4 = elementDict4.CreateArray("CFBundleURLSchemes");
        //urlScheme4.AddString("sa62a428ec");


        //// 增加白名单 scheme   打开别的app需要  比如 分享到微信 需要添加 wechat、weixin、weixinULAPI
        //PlistElement array = null;
        //if (rootDict.values.ContainsKey("LSApplicationQueriesSchemes"))
        //    array = rootDict["LSApplicationQueriesSchemes"].AsArray();
        //else
        //    array = rootDict.CreateArray("LSApplicationQueriesSchemes");
        //rootDict.values.TryGetValue("LSApplicationQueriesSchemes", out array);
        //PlistElementArray Qchemes = array.AsArray();
        ////快手平台Scheme
        //Qchemes.AddString("kwai");
        //Qchemes.AddString("kwaiAuth2");
        //Qchemes.AddString("KwaiBundleToken");
        ////微信平台Scheme
        //Qchemes.AddString("wechat");
        //Qchemes.AddString("weixin");
        //Qchemes.AddString("weixinULAPI");
        ////QQ平台Scheme
        //Qchemes.AddString("mqqOpensdkSSoLogin");
        //Qchemes.AddString("mqqopensdkapiV2");
        //Qchemes.AddString("mqqopensdkapiV3");
        //Qchemes.AddString("wtloginmqq2");
        //Qchemes.AddString("mqq");
        //Qchemes.AddString("mqqapi");

        plist.WriteToFile(plistPath);
#endif
    }


#if UNITY_IOS

    // <summary>
    // 添加Lib到Xcode工程
    // </summary>
    // <param name="inst"></param>
    // <param name="targetGuid"></param>
    // <param name="lib"></param>
    public static void AddLibToProject(PBXProject inst, string targetGuid, string lib)
    {
        string fileGuid = inst.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
        inst.AddFileToBuild(targetGuid, fileGuid);
    }
#endif
}
