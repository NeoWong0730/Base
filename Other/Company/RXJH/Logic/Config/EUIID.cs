using System;
using System.Collections.Generic;
using Logic;
using EUIOption = Framework.UI.EUIOption;
using UIConfigData = Framework.UI.UIConfigData;

namespace Logic {
    //public class UIConfigData : Framework.Core.UI.UIConfigData
    //{
    //    /// <summary>
    //    /// UI配置
    //    /// </summary>
    //    /// <param name="prefabPath">预制体加载路径</param>
    //    /// <param name="script">逻辑脚本类型</param>
    //    /// <param name="options">UI设置组</param>
    //    /// <param name="order">
    //    /// 深度值
    //    /// 当设置包含eIgnoreStack时:
    //    /// order 大于等于 0 : UI栈层级上限 + order
    //    /// order 小于 0 : UI栈层级下限 + order
    //    ///
    //    /// 当设置不包含eIgnoreStack时:
    //    /// order暂时无效
    //    /// </param>
    //    public UIConfigData(Type script, string prefabPath, Framework.Core.UI.EUIOption options = Framework.Core.UI.EUIOption.eInvalid, int order = 0)
    //        : base(script, prefabPath, options, order) { }
    //}

    /// <summary>
    /// 添加界面 依次添加!
    /// 添加界面 依次添加!
    /// 添加界面 依次添加!
    /// </summary>
    public enum EUIID {
        Invalid = 0,
        UI_Login = 1, // 登录界面
        UI_ServerList = 2, // 服务器
        UI_CreateCharacter = 3, // 创角
        UI_LoginOrCreateCharacter = 4, // 创角
        UI_MakeFace = 5, // 捏脸
        UI_CharacterPreview = 6, // 所有职业展示
        UI_BlockClick = 7, // 转菊花
        UI_TaskMain = 8, // 任务主界面
        UI_TaskOp = 9, // 任务快捷操作
        UI_TaskAccept = 10, // 任务接受

        // 以下为老项目的一些
        UI_MainInterface, // 主界面面板管理容器
        UI_ExternalNotice, // 公告
        UI_MainBattle, // 战斗界面

        UI_Test,
    }
}