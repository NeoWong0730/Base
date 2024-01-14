using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityEngine.AddressableAssets
{
    public static class PathRebuild
    {
        public static bool LoadRemoteVersionFinish = false; 
        //设置Addressable 的远端加载路径
        //private static string mRemoteLoadPath = string.Format("{0}/{1}/{2}.{3}/{4}", VersionHelper.HotFixUrl, AssetPath.sPlatformName, VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion, AssetPath.sAddressableDir);
        //private static string mRemoteLoadPath = string.Format("{0}/{1}/{2}/{3}", "ftp://192.168.3.12", VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion, AssetPath.sAddressableDir);
        public static string RemoteLoadPath
        {
            get
            {
                string rlt=string.Empty;
                if (LoadRemoteVersionFinish)
                {
                    rlt = string.Format("{0}/{1}/{2}.{3}/{4}", VersionHelper.HotFixUrl, AssetPath.sPlatformName, VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion, AssetPath.sAddressableDir);
                }
                else
                {
                    DebugUtil.LogError("加载远端版本信息没有成功，导致重定向远端路径出错！");
                }
                return rlt;
            }
        }


        //设置Addressable 的本地构建路径
        private static string mLocalBuildPath;
        public static string LocalBuildPath
        {
            get
            {
                if (string.IsNullOrEmpty(mLocalBuildPath))
                {
                    DirectoryInfo info = new DirectoryInfo(Application.dataPath);
                    string papaPath = info.Parent.Parent.FullName.Replace('\\', '/');
                    mLocalBuildPath = string.Format("{0}/{1}/{2}", papaPath, AssetPath.sAddressableBundle, PlatformMappingService.GetPlatformPathSubFolder());
                } 
                return mLocalBuildPath;
            }
        }

        //Path.GetFullPath(Application.dataPath + "/../AssetBundle/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString());

        //设置Addressable 的本地加载路径
        private static string mLocalLoadPath;
        public static string LocalLoadPath {
            get
            {
                return LocalBuildPath;
            }
        }


    }
}


