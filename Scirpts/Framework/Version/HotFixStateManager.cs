using Lib;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Util;


using System.Linq;
using UnityEngine.ResourceManagement;
using System;

namespace Framework
{

    public enum EHotFixState
    {
        Invalid = 0,
        DownLoad_HotFixList = 1,
        Check_AddressCatalog = 2,
        Check_HotFixList = 3,
        Wait_FixAsset = 4,
        Destroy_SpareAssets = 5,
        DownLoad_AddressAsset = 6,
        DownLoad_ConfigAsset = 7,
        ReCheck_HotFixList = 8,
        Success = 9,
        //Fail = 10,
    }

    public enum EUserConfirm
    {
        Yes,
        No,
    }


    public class HotFixStateManager : TSingleton<HotFixStateManager>
    {
        public const ulong KiloByte = 1024UL;
        public const ulong MegaByte = 1048576UL;
        public const ulong GigaByte = 1073741824UL;

        public const string sByteFormat = "{0:F2}Byte";
        public const string sKiloByteFormat = "{0:F2}K";
        public const string sMegaByteFormat = "{0:F2}M";
        public const string sGigaByteFormat = "{0:F2}G";

        /// <summary>��Ҫ��ʾ�Ĵ�С</summary>
        public readonly ulong HotFixNeedTipSize = MegaByte * 100UL;

        /// <summary>�ȸ���С�ı��ʣ����ڼ����ڴ�ʣ��ռ� </summary>
        public readonly float HotFixSizeRate = 2f;

        /// <summary>�����ϵ����� ��С�ļ���С</summary>
        public readonly ulong CacheFileMinSize = MegaByte;

        /// <summary>�汾������</summary>
        public float CheckVersionProgress; //{ get; private set; }

        /// <summary>�ȸ����ļ�������</summary>
        public int HotFixFileCount; //{ get; private set; }

        /// <summary>�����ٶ�</summary>
        public ulong DownloadSpeed { get; private set; }

        /// <summary>Bundle��Դ��С</summary>
        public ulong AssetBundleTotalSize; //{ get; private set; }

        /// <summary>���ʹ�����Դ�ܴ�С</summary>
        //public ulong CSVAndDllTotalSize; //{ get; private set; }

        /// <summary>�����ܴ�С</summary>
        public ulong HotFixTotalSize; //{ get; private set; }

        /// <summary>�Ѿ����µĴ�С</summary>
        public ulong HotFixSize { get; set; }

        /// <summary>Զ��catalog�Ĵ�С KBΪ��λ</summary>
        public readonly ulong NeedLoadCatalogSize = 1024 * 20UL;


        /// <summary> ���һ�μ��������ٶ�ʱ���Ѹ��´�С </summary>
        private ulong RecodeHotFixSize = 0;

        private float RecodeTime = 0;

        /// <summary> �Ѿ��ȸ���ϵ��ȸ���Դ </summary>
        public int LoadedHotFixFileCount = 0;

        //��������Զ����Դ��ַ
        public string DownloadABUrl;
        //public string RemoteHotFixAssetsListUrl;
#if UNITY_STANDALONE_WIN
        public string PackagePlatformName = "StandaloneWindows64";
#elif UNITY_IOS
        public string PackagePlatformName = "iOS";
#elif UNITY_ANDROID
        public string PackagePlatformName = "Android";
#endif


        public bool HotFixSeriesErrorRetry = false;

        private AssetList mCacheAssetList = null;
        public AssetList CacheAssetList
        {
            set
            {
                mCacheAssetList = value;
            }
            get
            {
                return mCacheAssetList;
            }
        }

        public List<AssetsInfo> NewAssets = new List<AssetsInfo>();
        public Queue<AssetsInfo> ExcessAssets = new Queue<AssetsInfo>();
        public Dictionary<string, int> RetryDownloadCountDict = new Dictionary<string, int>();
        private int RetryCount = 3;
        private int ReallyRetryCount = 6;
        private int RecordDownloadCount = 0;

        IHotFixState _IHotFixState = null;

        private EHotFixState _CurrentHotFixState;
        public EHotFixState CurrentHotFixState
        {
            set { _CurrentHotFixState = value; }
            get { return _CurrentHotFixState; }
        }


        private EHotFixState _NextHotFixState;
        public EHotFixState NextHotFixState
        {
            get { return _NextHotFixState; }
            set { _NextHotFixState = value; }
        }


        public static string CountSize(ulong Size)
        {
            string m_strSize = string.Empty;
            ulong FactSize = Size;

            if (Size < KiloByte)
            {
                m_strSize = string.Format(sByteFormat, FactSize);
            }
            else if (Size >= KiloByte && Size < MegaByte)
            {
                FactSize = Size / KiloByte;

                m_strSize = string.Format(sKiloByteFormat, FactSize);
            }
            else if (Size >= MegaByte && Size < GigaByte)
            {
                FactSize = Size / MegaByte;
                m_strSize = string.Format(sMegaByteFormat, FactSize);
            }
            else
            {
                FactSize = Size / GigaByte;
                m_strSize = string.Format(sGigaByteFormat, FactSize);

            }
            return m_strSize;
        }
        public void OnEnter()
        {
            VersionHelper.Clear();

            CheckVersionProgress = 0;
            RecodeHotFixSize = 0;
            RecodeTime = 0;

            DownloadABUrl = string.Format("{0}/{1}/{2}.{3}", VersionHelper.HotFixUrl, AssetPath.sPlatformName, VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion);
            //RemoteHotFixAssetsListUrl = string.Format("{0}/{1}/{2}.{3}/{4}", VersionHelper.HotFixUrl, AssetPath.sPlatformName, VersionHelper.RemoteBuildVersion, VersionHelper.RemoteAssetVersion, AssetPath.sHotFixListName);
            //string settingsPath = string.Format("{0}/{1}/settings.json", Application.streamingAssetsPath, AssetPath.sAddressableDir);
            //var Runtimedata = JsonUtility.FromJson<UnityEngine.AddressableAssets.Initialization.ResourceManagerRuntimeData>(File.ReadAllText(settingsPath));
            //if (Runtimedata != null)
            //PackagePlatformName = Runtimedata.BuildTarget;

            _CurrentHotFixState = EHotFixState.Invalid;
            _NextHotFixState = EHotFixState.DownLoad_HotFixList;
        }
        public void OnExit()
        {
            VersionHelper.Clear();
            if (_IHotFixState != null)
            {
                _IHotFixState.OnExit();
                _IHotFixState = null;
            }
        }
        public void OnUpdate()
        {
            HotFixStateTranslate();
            _IHotFixState?.OnUpdate();

            if (CurrentHotFixState == EHotFixState.DownLoad_ConfigAsset || CurrentHotFixState == EHotFixState.DownLoad_AddressAsset)
            {
                UpdateDownloadSpeed();
            }
        }
        private bool HotFixStateTranslate()
        {
            if (_NextHotFixState == EHotFixState.Invalid)
                return false;
            EHotFixState currentState = _IHotFixState != null ? _IHotFixState.State : EHotFixState.Invalid;

            if (_NextHotFixState == currentState)
                return false;

            if (_IHotFixState != null)
            {
                _IHotFixState.OnExit();
                _IHotFixState = null;
            }

            switch (_NextHotFixState)
            {
                case EHotFixState.DownLoad_HotFixList:
                    _IHotFixState = new HotFix_DownLoadAssetList();
                    break;
                case EHotFixState.Check_HotFixList:
                    _IHotFixState = new HotFix_CheckHotFixList();
                    break;
                case EHotFixState.Wait_FixAsset:
                    //������ʾ >100M ������ͬ��
                    break;
                case EHotFixState.Destroy_SpareAssets:
                    _IHotFixState = new HotFix_DestroySpareAssets();
                    break;
                case EHotFixState.DownLoad_AddressAsset:
                    //Addressable �ȸ�ͣ��
                    //_IHotFixState = new HotFix_DownLoadAddressAsset();
                    break;
                case EHotFixState.DownLoad_ConfigAsset:
                    _IHotFixState = new HotFix_DownLoadConfigAsset();
                    break;
                case EHotFixState.ReCheck_HotFixList:
                    _IHotFixState = new HotFix_ReCheckHotFixList();
                    break;
                case EHotFixState.Check_AddressCatalog:
                    //Addressable �ȸ�ͣ��
                    //_IHotFixState = new HotFix_AddressCheckCatalog();
                    break;
                case EHotFixState.Success:
                    AppManager.NextAppState = EAppState.Game;
                    break;
            }

            currentState = _CurrentHotFixState = _NextHotFixState;

            _NextHotFixState = EHotFixState.Invalid;

            if (_IHotFixState != null)
            {
                _IHotFixState.OnEnter();
            }

            return true;
        }
        public bool AssetsLoadComplete()
        {
            if (LoadedHotFixFileCount >= HotFixFileCount)
                return true;
            return false;
        }
        public bool MemoryEnough()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            //����ֻ�ʣ���ڴ��С; ����android��
            uint NeedTotalLeftMemorySize = (uint)((HotFixTotalSize * HotFixSizeRate) / KiloByte);
            uint persistDirleftMemorySize = (uint)SDKManager.SDKGetLeftMemorySize();

            Debug.LogErrorFormat("�ж��ڴ棺��Ҫ���ڴ�ռ�{0}��ʣ����ڴ�ռ�{1}", NeedTotalLeftMemorySize, persistDirleftMemorySize);

            if (persistDirleftMemorySize <= NeedTotalLeftMemorySize)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("left_space", string.Format("{0}KB", persistDirleftMemorySize));
                dict.Add("need_space", string.Format("{0}KB", NeedTotalLeftMemorySize));
                HitPointManager.HitPoint("game_update_space", dict);

                AppManager.eAppError = EAppError.MemoryNotEnoughError;
                NextHotFixState = EHotFixState.Invalid;
                return false;
            }
            else
            {
                return true;
            }
#else
            //windows ƽ̨�Ͳ�Ҫ�ж��˰�
            return true;
#endif

        }

        /// <summary>
        /// �ȸ�����ʧ�ܴ��
        /// </summary>
        /// <param name="webRequestResult"></param>
        public static void HotFixHttpErrorHitPoint(UnityWebRequestResult webRequestResult)
        {
            //���ó�ʱ,�᷵����isNetworkErrorΪtrue
            if (webRequestResult.Error == "Request timeout")//== 408
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("file_url", webRequestResult.Url);
                dict.Add("wait_time", "10");
                dict.Add("speed", string.Format("{0}/S", HotFixStateManager.CountSize(HotFixStateManager.Instance.DownloadSpeed))); //DownloadSpeed ��׼
                //HitPointManager.HitPoint("game_update_httptimeout", dict);
            }
            else if (webRequestResult.ResponseCode == 404)//�ļ���ʧ
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("file_url", webRequestResult.Url);
                //HitPointManager.HitPoint("game_update_missing", dict);
            }
            else//����ԭ��
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("file_url", webRequestResult.Url);
                dict.Add("reason", webRequestResult.Error);
                //HitPointManager.HitPoint("game_update_othercase", dict);
            }

            DebugUtil.LogFormat(ELogType.eNone, string.Format("error:{0}, url={1}", webRequestResult.Error, webRequestResult.Url));
        }

        /// <summary>
        /// �û�ȷ�ϲ���
        /// </summary>
        /// <param name="userConfirm"></param>
        public void SetUserConfirm(EUserConfirm userConfirm)
        {
            if (_CurrentHotFixState == EHotFixState.Wait_FixAsset)
            {
                if (userConfirm == EUserConfirm.Yes)
                {
                    //HitPointManager.HitPoint("game_update_confirm");
                    if (ExcessAssets.Count > 0)
                    {
                        NextHotFixState = EHotFixState.Destroy_SpareAssets;
                    }
                    else
                    {
                        //������bundle
                        //if (VersionHelper.mRemoteAaBundleList.Count > 0)
                        //{
                        //    NextHotFixState = EHotFixState.DownLoad_AddressAsset;
                        //}
                        if (NewAssets.Count > 0)
                        {
                            NextHotFixState = EHotFixState.DownLoad_ConfigAsset;
                        }
                        else
                        {
                            NextHotFixState = EHotFixState.ReCheck_HotFixList;
                        }
                    }
                }
                else if (userConfirm == EUserConfirm.No)
                {
                    //NextHotFixState = EHotFixState.Fail;--����Ҫ�ȵ���һ֡
                    OnExit();
                }
            }
            //else if (_CurrentHotFixState == EHotFixState.WaitFail)
            //{
            //    if (userConfirm == EUserConfirm.Yes)
            //    {
            //        NextHotFixState = EHotFixState.CheckAssetList;
            //    }
            //    else if (userConfirm == EUserConfirm.No)
            //    {
            //        NextHotFixState = EHotFixState.Fail;
            //    }
            //}
        }
        private void UpdateDownloadSpeed()
        {
            RecodeTime += Time.unscaledDeltaTime;
            if (RecodeTime >= 0.5f)
            {
                DownloadSpeed = (ulong)((double)(HotFixSize - RecodeHotFixSize) / (double)RecodeTime);
                RecodeHotFixSize = HotFixSize;
                RecodeTime = 0;
            }
        }
        public void SetProgress(float min, float max, float p)
        {
            CheckVersionProgress = (max - min) * p + min;
        }

        /// <summary>
        /// ��Ϸ��ͣ
        /// </summary>
        /// <param name="pause"></param>
        private void OnApplicationPause(bool pause)
        {
            DebugUtil.LogFormat(ELogType.eNone, "OnApplicationPause({0})", pause);

            //_IHotFixState?.OnApplicationPause(pause);
        }


        #region ����У��ɳ��·�����е��ȸ�����Դ
        public static Action CheckMD5AssetList;
        UnityWebRequest webRequest;
        public void CheckPersistentAssetMd5(Action CheckMD5Action)
        {
            string RemoteHotFixAssetsListUrl = string.Format("{0}/{1}/{2}.{3}/{4}",
                VersionHelper.HotFixUrl,
                AssetPath.sPlatformName,
                VersionHelper.RemoteBuildVersion,
                VersionHelper.RemoteAssetVersion,
                AssetPath.sHotFixListName);


            webRequest = UnityWebRequest.Get(new System.Uri(RemoteHotFixAssetsListUrl));
            UnityWebRequestAsyncOperation webAsyncOperation = webRequest.SendWebRequest();
            webAsyncOperation.completed += WebAsyncOperation_completed;

            CheckMD5AssetList = CheckMD5Action;
        }
        private void WebAsyncOperation_completed(AsyncOperation obj)
        {
            UnityWebRequestResult uwrResult = null;
            if (webRequest != null && !UnityWebRequestUtilities.RequestHasErrors(webRequest, out uwrResult))
            {
                //�������
                //HotFixStateManager.Instance.CheckVersionProgress = 0.3f;

                MemoryStream ms = new MemoryStream(webRequest.downloadHandler.data);
                VersionHelper.SetRemoteAssetsList(ms);

                webRequest.Dispose();
                webRequest = null;
            }
            else
            {
                Debug.LogFormat("����Զ����Դ�б�ʧ�� {0}", webRequest.error);
                webRequest.Dispose();
                webRequest = null;
            }

            LoadPersistentAssetList();
        }

        private void LoadPersistentAssetList()
        {
            string assetsListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);
            if (File.Exists(assetsListPath))
            {
                assetsListPath = string.Format("{0}{1}", AssetPath.sPersistentPrefix, assetsListPath);
                DebugUtil.LogFormat(ELogType.eNone, "����Persistent��Դ�б� {0}", assetsListPath);

                webRequest = UnityWebRequest.Get(new System.Uri(assetsListPath));
                UnityWebRequestAsyncOperation webAsyncOperation = webRequest.SendWebRequest();
                webAsyncOperation.completed += PersistWebAsyncOperation_completed;
            }
            else
            {
                DebugUtil.LogFormat(ELogType.eNone, "Persistent������ {0}", assetsListPath);
                CheckErrorAsset();
            }
        }

        private void PersistWebAsyncOperation_completed(AsyncOperation obj)
        {
            //��Դ���ܻᱻɾ��
            UnityWebRequestResult uwrResult = null;
            if (webRequest != null && !UnityWebRequestUtilities.RequestHasErrors(webRequest, out uwrResult))
            {
                MemoryStream ms = new MemoryStream(webRequest.downloadHandler.data);
                VersionHelper.SetPersistentAssetsList(ms);

                webRequest.Dispose();
                webRequest = null;
            }
            else
            {
                DebugUtil.LogFormat(ELogType.eNone, "����ɳ��·����Դ�б�ʧ�� {0}", webRequest.error);
                webRequest.Dispose();
                webRequest = null;
            }

            CheckErrorAsset();
        }
        void CheckErrorAsset()
        {
            if (VersionHelper.mRemoteAssetsList != null)
            {
                var RemoteItor = VersionHelper.mRemoteAssetsList.Contents.GetEnumerator();
                AssetsInfo remoteInfo;
                int StreamAssetVersion = int.Parse(VersionHelper.StreamingAssetVersion);

                while (RemoteItor.MoveNext())
                {
                    remoteInfo = RemoteItor.Current.Value;

                    //ɾ������Դ ����ҪMD5У��
                    if (remoteInfo.State == 1)
                        continue;
                    //С���װ���������Դ������ҪУ�飨1.7.5�ȸ���,����ҪУ��<=5����Դ��
                    if (remoteInfo.Version <= StreamAssetVersion)
                        continue;

                    if (remoteInfo.AssetType == 1)
                    {
                        string bundlePath = string.Format("{0}/{1}/{2}/__data", Caching.defaultCache.path, remoteInfo.BundleName, remoteInfo.HashOfBundle);
                        if (File.Exists(bundlePath))
                        {
                            string md5 = FrameworkTool.GetFileMD5(bundlePath);
                            if (!string.Equals(remoteInfo.AssetMD5, md5, StringComparison.Ordinal))
                            {
                                ExcessAssets.Enqueue(remoteInfo);
                                string hashPath = string.Format("{0}/{1}", Caching.defaultCache.path, remoteInfo.BundleName);
                                FileOperationHelper.DeleteDirectory(hashPath);
                                DebugUtil.LogFormat(ELogType.eNone, string.Format("��Դmd5����ɾ����{0}", hashPath));
                            }
                        }
                        else
                        {
                            //�ļ������ڣ�������Ϊ�ֶ�ɾ��
                            DebugUtil.LogFormat(ELogType.eNone, string.Format("�ļ������ڣ�{0}", bundlePath));
                            ExcessAssets.Enqueue(remoteInfo);
                        }
                    }
                    else
                    {
                        string path = remoteInfo.AssetName.Replace("\\", "/");
                        string filePath = AssetPath.GetPersistentFullPath(path);
                        if (File.Exists(filePath))
                        {
                            string md5 = FrameworkTool.GetFileMD5(filePath);
                            if (!string.Equals(remoteInfo.AssetMD5, md5, StringComparison.Ordinal))
                            {
                                ExcessAssets.Enqueue(remoteInfo);
                                File.Delete(filePath);
                                DebugUtil.LogFormat(ELogType.eNone, string.Format("��Դmd5����ɾ����{0}", filePath));
                            }
                        }
                        else
                        {
                            //�ļ������ڣ�������Ϊ�ֶ�ɾ��
                            DebugUtil.LogFormat(ELogType.eNone, string.Format("�ļ������ڣ�{0}", filePath));
                            ExcessAssets.Enqueue(remoteInfo);
                        }
                    }
                }
            }


            if (ExcessAssets.Count > 0)
            {
                //ɾ��Persistent�У�������Դ����Ϊ�ֶ�ɾ����Դ�ļ�¼
                if (VersionHelper.mPersistentAssetsList != null && VersionHelper.mPersistentAssetsList.Contents != null)
                {
                    foreach (var item in ExcessAssets)
                    {
                        if (VersionHelper.mPersistentAssetsList.Contents.ContainsKey(item.AssetName))
                            VersionHelper.mPersistentAssetsList.Contents.Remove(item.AssetName);
                    }

                    //����д�ص�Persistent��¼
                    string HotFixListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);
                    VersionHelper.WriteAssetList(HotFixListPath, VersionHelper.mPersistentAssetsList);
                }


                //���İ汾�� -- ��ȫ�������£����ᣬ�����¼����,���绺���¼�ļ���ɾ������ôȫ���ͻ�������
                VersionHelper.SetPersistentVersion(VersionHelper.StreamingBuildVersion, VersionHelper.StreamingAssetVersion);
                string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
                VersionHelper.WritePersistentVersion(persistentVersionPath);
            }

            VersionHelper.Clear();

            if (CheckMD5AssetList != null)
                CheckMD5AssetList.Invoke();
        }

        #endregion
    }

}
