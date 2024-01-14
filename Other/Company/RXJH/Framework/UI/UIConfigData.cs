using System;

namespace Framework.UI
{
    public enum EUIState
    {
        Invalid,
        WaitShow,
        Showing,
        Show,
        WaitHide,
        Hiding,
        Hide,
        Close,
        Destroy,
    }

    [FlagsAttribute]
    public enum EUIOption
    {
        eInvalid = 0,
        eHideMainCamera = 1,        //UI打开后关闭主相机绘制（进入动画播放完成后）
        eHideBeforeUI = 2,          //UI打开后隐藏前面打开的UI（堆栈中的主节点）
        eIgnoreClear = 4,           //不受UIManager.ClearUI管理
        eIgnoreStack = 8,           //不参与UI栈逻辑 需要自定义层级SortingOrde
        eReduceFrameRate = 16,      //当UI在最上层打开时以低帧率执行
        eReduceMainCameraQuality = 32,      //当UI显示时降低场景质量
        eDontHideFlag = 64,         //标记UI不被 下一个标记了eDontHideBefore的主UI隐藏
        eDontHideBeforeIfHasFlag = 128,      //不影藏前一个有eDontHideFlag标记的主UI（仅对主UI有用）
    }

    public class UIConfigData
    {
        public readonly string prefabPath;
        public readonly Type script;
        public readonly EUIOption options;
        public readonly int order;

        /// <summary>
        /// UI配置
        /// </summary>
        /// <param name="prefabPath">预制体加载路径</param>
        /// <param name="script">逻辑脚本类型</param>
        /// <param name="options">UI设置组</param>
        /// <param name="order">
        /// 深度值
        /// 当设置包含eIgnoreStack时:
        /// order 大于等于 0 : UI栈层级上限 + order
        /// order 小于 0 : UI栈层级下限 + order
        ///
        /// 当设置不包含eIgnoreStack时:
        /// order暂时无效
        /// </param>
        public UIConfigData(Type script, string prefabPath, EUIOption options = EUIOption.eInvalid, int order = 0)
        {
            this.prefabPath = prefabPath;
            this.options = options;
            this.script = script;
            this.order = order;
        }
    }
}