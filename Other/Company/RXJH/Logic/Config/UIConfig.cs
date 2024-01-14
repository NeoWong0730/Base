using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using Logic.UI;
using EUIOption = Framework.UI.EUIOption;
using UIConfigData = Framework.UI.UIConfigData;

namespace Logic {
    public static partial class UIConfig {
        public static UIConfigData GetConfData(int id) {
            datas.TryGetValue(id, out UIConfigData data);
            return data;
        }

        public static UIConfigData GetConfData(EUIID id) {
            datas.TryGetValue((int)id, out UIConfigData data);
            return data;
        }

        public static string PrefabPath(string prefabRelativePath) {
            return prefabRelativePath; // UIManager.PortraitOrLandscape ? $"UI/Portrait/{prefabRelativePath}" : $"UI/Landscape/{prefabRelativePath}";
        }
    }

    public static partial class UIConfig {
        private static readonly Dictionary<int, UIConfigData> datas = new Dictionary<int, UIConfigData>() {
            {
                (int)EUIID.UI_Login, new UIConfigData(
                    typeof(UI_Login),
                    PrefabPath("ui_login"))
            }, {
                (int)EUIID.UI_ServerList, new UIConfigData(
                    typeof(UI_ServerList),
                    PrefabPath("ui_severlist"))
            }, {
                (int)EUIID.UI_LoginOrCreateCharacter, new UIConfigData(
                    typeof(UI_LoginOrCreateCharacter),
                    PrefabPath("ui_loginorcreatecharacter"), EUIOption.eHideBeforeUI)
            }, {
                (int)EUIID.UI_CreateCharacter, new UIConfigData(
                    typeof(UI_CreateCharacter),
                    PrefabPath("ui_createcharacter"), EUIOption.eHideBeforeUI)
            }, {
                (int)EUIID.UI_MakeFace, new UIConfigData(
                    typeof(UI_MakeFace),
                    PrefabPath("ui_makeface"), EUIOption.eHideBeforeUI)
            }, {
                (int)EUIID.UI_CharacterPreview, new UIConfigData(
                    typeof(UI_CharacterPreview),
                    PrefabPath("ui_characterpreview"))
            }, {
                (int)EUIID.UI_BlockClick, new UIConfigData(
                    typeof(UI_BlockClick),
                    PrefabPath("ui_blockclick"))
            }, {
                (int)EUIID.UI_TaskMain, new UIConfigData(
                    typeof(UI_TaskMain),
                    PrefabPath("ui_blockclick"))
            }, {
                (int)EUIID.UI_TaskOp, new UIConfigData(
                    typeof(UI_TaskOp),
                    PrefabPath("ui_blockclick"))
            }, {
                (int)EUIID.UI_TaskAccept, new UIConfigData(
                    typeof(UI_TaskAccept),
                    PrefabPath("ui_blockclick"))
            }, {
                (int)EUIID.UI_Test, new UIConfigData(
                    typeof(UI_Test),
                    PrefabPath("ui_test"), EUIOption.eIgnoreStack)
            },
        };
    }
}