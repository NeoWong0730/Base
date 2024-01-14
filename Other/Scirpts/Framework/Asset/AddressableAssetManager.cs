using System;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;


namespace Framework
{
    public class AddressableAssetManager : TSingleton<AddressableAssetManager>
    {
        public void InternalIdRegister()
        {
#if UNITY_EDITOR  //�༭���������Լ����� -- �����޸�
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
                //PrimaryKey ��AB��������
                //path����StreamingAsset/Bundles/AB����.bundle������Bundles���Զ����ļ������֣�����Ӧ�ó���ʱ�����Ƶ�Ŀ¼
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

