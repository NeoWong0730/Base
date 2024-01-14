using UnityEngine;

namespace Framework
{
    public class VersionSetting : ScriptableObject
    {
        public EHotFixMode eHotFixType;
        public string AssetVersion;//资源版本号
        public string VersionUrl;
        public string ProfileName;
        public string ChannelName;
        public EChannelFlags eChannelFlags;
        public string HotFixUniqueIdentifier;//热更系列版本标识
        public string PackageIdentifier;//包的唯一标识
        public string AppVersion;//App版本号
    }
}
