using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;


namespace Framework
{
    public class AddressableAssetManager : TSingleton<AddressableAssetManager>
    {
        public void InternalIdRegister()
        {
#if UNITY_EDITOR  //编辑器下由我自己测试 -- 后面修改
         Addressables.InternalIdTransformFunc = InternalIdTransformFunc;
#else
         Addressables.InternalIdTransformFunc = InternalIdTransformFunc;
#endif         

        }



#if UNITY_EDITOR
        public string RemoteBuildPath
        {
            get { return "ServerData" + "/" + Enum.GetName(typeof(UnityEditor.BuildTarget), UnityEditor.EditorUserBuildSettings.activeBuildTarget); }
        }
#endif


        private string InternalIdTransformFunc(UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation location)
        {
            
            if (location.Data is AssetBundleRequestOptions)
            {
                //PrimaryKey 是AB包的名字
                //path就是StreamingAsset/Bundles/AB包名.bundle，其中Bundles是自定义文件夹名字，发布应用程序时，复制的目录
#if UNITY_EDITOR
                string path = Path.Combine(RemoteBuildPath, location.PrimaryKey).Replace("\\", "/");
#else
                string path = Path.Combine(Application.streamingAssetsPath, "AssetBundle", location.PrimaryKey).Replace("\\", "/");
#endif
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return location.InternalId;
        }
        
    }
}

