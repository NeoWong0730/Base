using UnityEngine;

namespace Lib.AssetLoader
{
    public class VersionSetting : ScriptableObject
    {
        public EHotFixMode eHotFixType;
        public string AssetVersion;//资源版本号
        public string VersionUrl;
        public string ProfileName;
        public string ChannelName;
        //public EChannelIDType eChannelIDType;
        public EChannelFlags eChannelFlags;
        //public EChannelType eChannelType;
        public string HotFixUniqueIdentifier;//标识热更一系列版本
        public string PackageIdentifier;//包的唯一标识
        public string AppVersion;//App版本号
    }
}
