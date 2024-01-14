using UnityEngine;

namespace Framework
{
    public class VersionSetting : ScriptableObject
    {
        public EHotFixMode eHotFixType;
        public string AssetVersion;//��Դ�汾��
        public string VersionUrl;
        public string ProfileName;
        public string ChannelName;
        public EChannelFlags eChannelFlags;
        public string HotFixUniqueIdentifier;//�ȸ�ϵ�а汾��ʶ
        public string PackageIdentifier;//����Ψһ��ʶ
        public string AppVersion;//App�汾��
    }
}
