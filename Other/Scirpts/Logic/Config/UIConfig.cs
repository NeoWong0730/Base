using System;
using System.Collections.Generic;
using Framework;

namespace Logic
{
    public static partial class UIConfig
    {
        public static UIConfigData GetConfData(int id)
        {
            datas.TryGetValue(id, out UIConfigData data);
            return data;
        }

        public static UIConfigData GetConfData(EUIID id)
        {
            datas.TryGetValue((int)id, out UIConfigData data);
            return data;
        }

        public static string PrefabPath(string prefabRelativePath)
        {
            return prefabRelativePath; // UIManager.PortraitOrLandscape ? $"UI/Portrait/{prefabRelativePath}" : $"UI/Landscape/{prefabRelativePath}";
        }
    }

    public static partial class UIConfig
    {
        private static readonly Dictionary<int, UIConfigData> datas = new Dictionary<int, UIConfigData>() {
           
        };
    }
}