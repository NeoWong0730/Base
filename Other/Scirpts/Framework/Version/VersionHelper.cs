using Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.ResourceManagement;

namespace Framework
{
    //热更新类型
    public enum EHotFixMode
    {
        Normal,
        Test,
        Close,
    }


    public enum EChannelIDType
    {
        None = 0,
        InternalTest = 1, //塔人内网测试
        ExternalTest = 2, //塔人外网测试
        ExternalOutTest = 3, //塔人外网测试(对外)
        BanShuTest = 4, //塔人版署
        KuaiShouPublish = 5,  //快手(正式)
        KuaiShouTest = 6//快手(测试)
    }

    [Serializable]
    public class Serialization<T>
    {
        [SerializeField]
        List<T> target;
        public List<T> ToList() { return target; }

        public Serialization(List<T> obj)
        {
            target = obj;
        }
    }

    [Serializable]
    public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<TKey> keys;

        [SerializeField]
        List<TValue> values;

        Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> ToDictionary() { return target; }

        public Serialization(Dictionary<TKey, TValue> obj)
        {
            target = obj;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            var count = Math.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            for (int i = 0; i < count; ++i)
            {
                target.Add(keys[i], values[i]);
            }
        }
    }


    [Serializable]
    public class VersionInfoRemoteExternal
    {
        [SerializeField]
        public int result;

        [SerializeField]
        public string error_msg;

        [SerializeField]
        public VersionInfoRemote hotfix;

    }



    [Serializable]
    public class VersionInfoRemote
    {
        [SerializeField]
        public int result;

        [SerializeField]
        public int zone_id;//区id

        [SerializeField]
        public string loginHost;

        [SerializeField]
        public string hotfix_version;

        [SerializeField]
        public string hotfix_url;

        [SerializeField]
        public bool blackUser;

        [SerializeField]
        public string zone_status;

        [SerializeField]
        public bool is_maintaining_whitelist;

        [SerializeField]
        public Dictionary<string, string> ext_json;

        //[SerializeField]
        //public string hotfixListMD5;

        //[SerializeField]
        //public string subpackage_url;

        //[SerializeField]
        //public string zone_status;

        //[SerializeField]
        //public string[] LoginUrl;
        //[SerializeField]
        //public string[] DirsvrUrl;
        //[SerializeField]
        //public string ChannelID;
    }

    [Serializable]
    public class UserInfoRealName
    {
        [SerializeField]
        public bool certificated;
    }



    public enum ZoneStatus
    {
        NORMAL = 0,
        MAINTAINING = 1,
    }


    //[Serializable]
    public class AssetsInfo
    {
        public int State;//0=修改或者新增, 1=刪除
        public int Version;
        public string AssetName; //hashOfFileName.bundle(aa)
        public ulong Size;
        public string AssetMD5;
        public int AssetType;//0=config,dll,1=aa,2=catalog.json
        public string BundleName; //沙盒目录下,该bundle资源对应的文件夹名 = BundleCache/BundleName
        public string HashOfBundle;//沙盒路径下,该bundle资源对应的唯一性文件夹 = BundleCache/BundleName/HashOfBundle/__data
        //public byte[] Datas;
    }

    public class AssetList
    {
        public string Version;
        public string VersionIdentifier;

        public Dictionary<string, AssetsInfo> Contents = new Dictionary<string, AssetsInfo>();


        public static string Serialize(AssetList assetList)
        {
            StringBuilder tempStringBuilder = StringBuilderPool.GetTemporary();

            tempStringBuilder.AppendFormat(assetList.Version == null ? "" : assetList.Version);
            tempStringBuilder.Append("\n");
            tempStringBuilder.AppendFormat(assetList.VersionIdentifier == null ? "" : assetList.VersionIdentifier);

            var itor = assetList.Contents.GetEnumerator();
            while (itor.MoveNext())
            {
                if (string.IsNullOrEmpty(itor.Current.Value.HashOfBundle))
                    tempStringBuilder.AppendFormat("\n{0}|{1}|{2}|{3}|{4}|{5}",
                        itor.Current.Value.Version,
                        itor.Current.Value.State,
                        itor.Current.Value.AssetMD5,
                        itor.Current.Value.AssetName,
                        itor.Current.Value.Size,
                        itor.Current.Value.AssetType);
                else
                    tempStringBuilder.AppendFormat("\n{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                       itor.Current.Value.Version,
                       itor.Current.Value.State,
                       itor.Current.Value.AssetMD5,
                       itor.Current.Value.AssetName,
                       itor.Current.Value.Size,
                       itor.Current.Value.AssetType,
                       itor.Current.Value.BundleName,
                       itor.Current.Value.HashOfBundle);
            }

            tempStringBuilder.Append("\n");

            string rlt = StringBuilderPool.ReleaseTemporaryAndToString(tempStringBuilder);
            return rlt;
        }

        public static AssetList Deserialize(Stream s)
        {
            AssetList assetList = Deserialize(new StreamReader(s));
            s.Close();
            return assetList;
        }

        public static AssetList Deserialize(string s)
        {
            AssetList assetList = Deserialize(new StringReader(s));
            return assetList;
        }

        public static AssetList Deserialize(TextReader sr)
        {
            AssetList assetList = new AssetList();

            string line = sr.ReadLine();
            assetList.Version = line;

            line = sr.ReadLine();
            assetList.VersionIdentifier = line;

            while (null != (line = sr.ReadLine()))
            {
                string[] kv = line.Split('|');
                if (kv != null && (kv.Length == 6 || kv.Length == 8))
                {
                    int version = 0;
                    int.TryParse(kv[0], out version);
                    int state = 0;
                    int.TryParse(kv[1], out state);

                    string md5 = kv[2];
                    string name = kv[3];
                    ulong size = ulong.Parse(kv[4]);

                    int assetType = 0;
                    int.TryParse(kv[5], out assetType);

                    string hashOfBundle = null;
                    string bundleName = null;
                    if (kv.Length == 8)
                    {
                        bundleName = kv[6];
                        hashOfBundle = kv[7];
                    }


                    if (!assetList.Contents.ContainsKey(name))
                    {
                        AssetsInfo info = new AssetsInfo();
                        info.Version = version;
                        info.State = state;
                        info.AssetMD5 = md5;
                        info.AssetName = name;
                        info.BundleName = bundleName;
                        info.HashOfBundle = hashOfBundle;
                        info.Size = size;
                        info.AssetType = assetType;
                        assetList.Contents.Add(info.AssetName, info);
                    }
                    else
                    {
                        Debug.LogWarningFormat("Exception Has replicate Asset {0}", kv);
                    }
                }
                else
                {
                    Debug.LogWarningFormat("Exception Line {0}", kv);
                }
            }

            sr.Close();

            return assetList;
        }

    }

    public static class VersionHelper
    {
        /// <summary>
        /// 渠道类型
        /// 配置在本地Streaming Version.txt中
        /// </summary>
        //public static EChannelType ChannelType { get; private set; }
        //public static EChannelIDType eChannelIDType { get; private set; } = EChannelIDType.None;
        public static string ChannelName { get; private set; }
        public static EChannelFlags eChannelFlags { get; private set; }
        public static EHotFixMode eHotFixMode { get; private set; } = EHotFixMode.Normal;
        public static string ResourceHotFixUniqueIdentifier { get; private set; }
        public static string PackageIdentifier { get; private set; }


        /// <summary>
        /// 版本信息获取地址
        /// 配置在本地Streaming Version.txt中
        /// </summary>
        static private string _VersionUrl;
        static private string _VersionUrlOverride;
        static public string VersionUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_VersionUrlOverride))
                    return _VersionUrlOverride;

#if UNITY_EDITOR
                if (string.IsNullOrWhiteSpace(_VersionUrl))
                {
                    return ChannelConfigs.GetDefaultVersionUrl(1);
                }
#endif

                return _VersionUrl;
            }
        }

        static public string HotFixUrl;
        static public string[] HotFixUrlArr;
        static public string LoginUrl { get; private set; }
        static public string DirsvrUrl { get; private set; }
        static public string DirNoticeUrl { get; private set; }
        static public Dictionary<string, string> ExtJson = new Dictionary<string, string>();

        static public string UserInfoUrl = "https://open.kuaishou.com/game/user_info";

        static public int ZoneId { get; private set; }
        static public ZoneStatus zoneStatus { get; private set; } = ZoneStatus.NORMAL;
        static public bool Is_Maintaining_whitelist { get; private set; }
        static public bool BlackUser { get; private set; }


        //static public string[] LoginUrls { get; private set; }
        //static public string[] DirsvrUrls { get; private set; }
        static public string AppBuildVersion { get; private set; } = "0.0.0";

        static public string StreamingBuildVersion { get; private set; } = "0.0";
        static public string PersistentBuildVersion { get; private set; } = "0.0";
        static public string RemoteBuildVersion { get; private set; } = "0.0";

        static public string StreamingAssetVersion { get; private set; } = "0";
        static public string PersistentAssetVersion { get; private set; } = "0";
        static public string RemoteAssetVersion { get; private set; } = "0";

        static public string FirstPackAssetListMD5;


        static public AssetList mPersistentAssetsList { get; private set; }
        static public AssetList mRemoteAssetsList { get; private set; }

        static public List<string> mRemoteAaBundleList;



        static string[] AppendLines = new string[1];

        public static void Clear()
        {
            //mRemoteAssetsList = null;
            mPersistentAssetsList = null;
            mRemoteAaBundleList = null;
        }



        public static void SetStreamingVersion(VersionSetting setting)
        {
            ChannelName = setting.ChannelName;
            //eChannelIDType = setting.eChannelIDType;
            //ChannelType = setting.eChannelType;
            eChannelFlags = setting.eChannelFlags;

            _VersionUrl = setting.VersionUrl;
            eHotFixMode = setting.eHotFixType;
            StreamingBuildVersion = GetMaximalVersion(setting.AssetVersion);
            StreamingAssetVersion = GetMinimalVersion(setting.AssetVersion);
            ResourceHotFixUniqueIdentifier = setting.HotFixUniqueIdentifier;
            PackageIdentifier = setting.PackageIdentifier;
            AppBuildVersion = setting.AppVersion;

            if (eHotFixMode == EHotFixMode.Close)
            {
                PersistentBuildVersion = StreamingBuildVersion;
                PersistentAssetVersion = StreamingAssetVersion;
            }
        }

        public static void ReadPersistentVersion(Stream stream)
        {
            StreamReader sr = new StreamReader(stream);

            string line = sr.ReadLine();
            while (!string.IsNullOrWhiteSpace(line))
            {
                int index = line.IndexOf('=');
                if (index > 0)
                {
                    string k = line.Substring(0, index);
                    string v = line.Substring(index + 1, line.Length - index - 1);

                    switch (k)
                    {
                        case "version":
                            {
                                PersistentBuildVersion = GetMaximalVersion(v);
                                PersistentAssetVersion = GetMinimalVersion(v);
                            }
                            break;
                        case "url":
                            {
                                //设置当前的 url
                                _VersionUrlOverride = v; //SetOverideCurrentChannelType(v);
                            }
                            break;
                        case "hotfix":
                            {
                                EHotFixMode hotFixMode = EHotFixMode.Close;
                                Enum.TryParse<EHotFixMode>(v, out hotFixMode);
                                eHotFixMode = hotFixMode;
                            }
                            break;
                        default:
                            break;
                    }

                }

                line = sr.ReadLine();
            }

            sr.Close();
            stream.Close();
        }


        public static void ReadRemoteVersion(Stream stream)
        {
            StreamReader sr = new StreamReader(stream);
            string content = sr.ReadToEnd();
            DebugUtil.LogFormat(ELogType.eNone, "接口获取内容：\n {0}", content);
            sr.Close();
            stream.Close();

            VersionInfoRemote versionInfo = LitJson.JsonMapper.ToObject<VersionInfoRemote>(content);
            string version = versionInfo.hotfix_version;

            #region 用于外部修改版本号和热更新地址
            //通过修改沙盒路径的 RemoteAppVersion.txt 修改远端版本号
            string remoteVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, "RemoteAppVersion.txt");
            if (File.Exists(remoteVersionPath))
            {
                string[] lines = File.ReadAllLines(remoteVersionPath);
                version = lines[0];
                if (lines.Length > 1)
                {
                    List<string> strList = lines.ToList();
                    strList.RemoveAt(0);
                    HotFixUrlArr = strList.ToArray();
                }
                else
                {
                    HotFixUrlArr = versionInfo.hotfix_url.Split(',');
                }
            }
            else
            {
                HotFixUrlArr = versionInfo.hotfix_url.Split(',');
            }
            #endregion

            RemoteBuildVersion = GetMaximalVersion(version);
            RemoteAssetVersion = GetMinimalVersion(version);
            ZoneId = versionInfo.zone_id;

            BlackUser = versionInfo.blackUser;
            zoneStatus = (ZoneStatus)Enum.Parse(typeof(ZoneStatus), versionInfo.zone_status);
            Is_Maintaining_whitelist = versionInfo.is_maintaining_whitelist;
            HotFixUrl = HotFixUrlArr[0];
            //ResourceManager.HotFixUrlArr = HotFixUrlArr;

            LoginUrl = string.Format("{0}/userlogin", versionInfo.loginHost);
            DirsvrUrl = string.Format("{0}/getserver", versionInfo.loginHost);
            DirNoticeUrl = string.Format("{0}/getloginnotice", versionInfo.loginHost);
            ExtJson = versionInfo.ext_json == null ? ExtJson : versionInfo.ext_json;

            SDKManager.GetThirdSdkSetting(versionInfo.ext_json);


            //获取游戏热更新信息成功
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("current_version", StreamingBuildVersion == PersistentBuildVersion ?
                string.Format("{0}.{1}", PersistentBuildVersion, PersistentAssetVersion) :
                string.Format("{0}.{1}", StreamingBuildVersion, StreamingAssetVersion));
            dict.Add("update_version", version);
            HitPointManager.HitPoint("game_checkversion_done", dict);
        }

        public static void SetLocalRemoteVersion()
        {
            RemoteBuildVersion = PersistentBuildVersion;
            RemoteAssetVersion = PersistentAssetVersion;
        }



        public static void ReadUserinfo(Stream stream)
        {
            StreamReader sr = new StreamReader(stream);
            string content = sr.ReadToEnd();
            DebugUtil.LogFormat(ELogType.eNone, "接口获取内容：\n {0}", content);
            VersionInfoRemote versionInfo = LitJson.JsonMapper.ToObject<VersionInfoRemote>(content);

            sr.Close();
            stream.Close();

            string version = versionInfo.hotfix_version;
        }


        public static void SetPersistentAssetsList(Stream stream)
        {
            mPersistentAssetsList = AssetList.Deserialize(stream);
            stream.Close();
        }
        public static void SetRemoteAssetsList(Stream stream)
        {
            mRemoteAssetsList = AssetList.Deserialize(stream);
            stream.Close();
        }

        public static void SetPersistentVersion(string buildVersion, string assetVersion)
        {
            PersistentBuildVersion = buildVersion;
            PersistentAssetVersion = assetVersion;
        }
        public static void SetPersistentAssetsList(AssetList assetList)
        {
            mPersistentAssetsList = assetList;
        }


        public static void AppendAssetInfoToPersistent(AssetsInfo info)
        {
            string path = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sHotFixListName);

            if (mPersistentAssetsList == null)
            {
                mPersistentAssetsList = new AssetList();
                mPersistentAssetsList.Version = string.Format("{0}.{1}", RemoteBuildVersion, RemoteAssetVersion);
                if (string.IsNullOrWhiteSpace(mPersistentAssetsList.Version))
                {
                    mPersistentAssetsList.Version = "0.0.0";
                }
                AppendLines[0] = mPersistentAssetsList.Version;
                File.AppendAllLines(path, AppendLines);

                AppendLines[0] = mPersistentAssetsList.VersionIdentifier = VersionHelper.mRemoteAssetsList.VersionIdentifier;
                File.AppendAllLines(path, AppendLines);
            }

            //同一份资源（资源名相同）应丢弃老的资源信息，保存最新的热更资源信息
            if (mPersistentAssetsList.Contents.ContainsKey(info.AssetName))
            {
                mPersistentAssetsList.Contents.Remove(info.AssetName);
            }
            mPersistentAssetsList.Contents.Add(info.AssetName, info);

            if (String.IsNullOrEmpty(info.HashOfBundle))
                AppendLines[0] = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", info.Version, info.State, info.AssetMD5, info.AssetName, info.Size, info.AssetType);
            else
                AppendLines[0] = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", info.Version, info.State, info.AssetMD5, info.AssetName, info.Size, info.AssetType, info.BundleName, info.HashOfBundle);

            File.AppendAllLines(path, AppendLines);
        }
        public static void AppendAssetInfoList(List<AssetsInfo> assetList)
        {
            if (assetList == null || assetList.Count <= 0)
                return;

            if (mPersistentAssetsList == null)
            {
                mPersistentAssetsList = new AssetList();
                mPersistentAssetsList.Version = string.Format("{0}.{1}", RemoteBuildVersion, RemoteAssetVersion);
                if (string.IsNullOrWhiteSpace(mPersistentAssetsList.Version))
                    mPersistentAssetsList.Version = "0.0.0";

                mPersistentAssetsList.VersionIdentifier = VersionHelper.mRemoteAssetsList.VersionIdentifier;
            }


            foreach (var item in assetList)
            {
                mPersistentAssetsList.Contents.Add(item.AssetName, item);
            }

            string HotFixListPath = AssetPath.GetPersistentFullPath(AssetPath.sHotFixListName);
            VersionHelper.WriteAssetList(HotFixListPath, VersionHelper.mPersistentAssetsList);
        }



        public static void WriteAssetList(string path, AssetList assetList)
        {
            string s = AssetList.Serialize(assetList);
            File.WriteAllText(path, s);
        }
        public static void WritePersistentVersion(string path)
        {
            List<string> content = new List<string>(4);
            string version = string.Format("version={0}.{1}", PersistentBuildVersion, PersistentAssetVersion);
            content.Add(version);

            //屏蔽写入热更模式到Persistent【之前估计是为了方便测试，打首包关闭热更的情况下，只需要更改version.txt中的热更方式即可实现热更】
            string hotfix = string.Format("hotfix={0}", eHotFixMode.ToString());
            content.Add(hotfix);

            if (!string.IsNullOrWhiteSpace(_VersionUrlOverride))
            {
                string url = string.Format("url={0}", _VersionUrlOverride);
                content.Add(url);
            }

            File.WriteAllLines(path, content);
        }



        public static List<string> hotfixModeList = new List<string>() { "hotfix=0", "hotfix=1", "hotfix=2", "url=1", };

        public static int CheckWritePersistentHotFixMode(string hotfixStr)
        {
            int rlt = 0;
            if (hotfixModeList.Contains(hotfixStr))
            {
                string[] str = hotfixStr.Split('=');
                eHotFixMode = (EHotFixMode)Enum.Parse(typeof(EHotFixMode), str[1]);

                string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
                WritePersistentVersion(persistentVersionPath);
                rlt = -5;
            }
            return rlt;
        }


        public static List<string> urlList = new List<string>() { "url=1", "url=2", "url=3", "url=4", "url=5", "url=6" };
        public static int CheckWritePersistentURL(string urlStr)
        {
            int rlt = 0;
            if (urlList.Contains(urlStr))
            {
                string[] str = urlStr.Split('=');
                // EChannelType eChannelType = (EChannelType)Enum.Parse(typeof(EChannelType), str[1]);

                string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
                WritePersistentVersion(persistentVersionPath);
                rlt = -5;
            }
            return rlt;
        }







        public static string GetMaximalVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return "0.0";
            }

            int index = version.LastIndexOf(".");
            if (index < 0)
            {
                return version;
            }
            else
            {
                return version.Remove(index);
            }
        }

        public static string GetMinimalVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return "0";
            }

            int index = version.LastIndexOf(".");
            if (index < 0)
            {
                return "0";
            }
            else
            {
                return version.Remove(0, index + 1);
            }
        }

        public static void SetTestHotFix(bool v)
        {
            string persistentVersionPath = string.Format("{0}/{1}", AssetPath.persistentDataPath, AssetPath.sVersionName);
            if (v)
            {
                eHotFixMode = EHotFixMode.Test;
            }
            else
            {
                eHotFixMode = EHotFixMode.Normal;
            }
            WritePersistentVersion(persistentVersionPath);
        }

        //public static void SetChannelType(EChannelType eChannelType)
        //{
        //    //eChannelConfig = GetVersionUrlByEChannelType(eChannelType);
        //}


        public static ChannelConfig GetVersionUrlByEChannelType(EChannelIDType eChannelType)
        {
            ChannelConfig schannelConfig = null;
            ChannelConfigs setting = Resources.Load<ChannelConfigs>("Resources/VersionConfigs");
            foreach (var item in setting.Urls)
            {
                if (item.sId == (int)eChannelType)
                    schannelConfig = item;
            }

            return schannelConfig;
        }

        public static string SetOverideCurrentChannelType(string chanelid)
        {
            string url = string.Empty;
            EChannelIDType _EChannelTypeOverride = EChannelIDType.None;
            if (Enum.TryParse(chanelid, true, out _EChannelTypeOverride))
            {
                if (_EChannelTypeOverride != EChannelIDType.None)
                {
                    ChannelConfig channelConfig = GetVersionUrlByEChannelType(_EChannelTypeOverride);
                    url = channelConfig.sVersionUrl;
                }
            }

            return url;
        }


    }
}