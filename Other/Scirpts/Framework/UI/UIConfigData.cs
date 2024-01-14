using System;

namespace Framework
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

    [Flags]
    public enum EUIOption
    { 
        eInvalid = 0,
        eHideMainCamera = 1,            //UI打开后关闭主相机绘制 （进入动画播放完成后）
        eHideBeforeUI = 2,              //UI打开后隐藏前面打开的UI （堆栈中的主节点）
        eIgnoreClear = 4,               //不受UIManager.ClearUI管理
        eIgnoreStack = 8,               //不参与UI栈逻辑，需要自定义层级sortingOrder
        eReduceFrameRate = 16,          //当UI在最上层打开时已低帧率执行
        eReduceMainCameraQuality = 32,  //当UI显示时降低场景质量
        eDontHideFlag = 64,             //标记UI不被下一个标记了eDontHideBefore的主UI隐藏
        eDontHideBeforeIfHasFlag = 128, //不隐藏前一个有eDontHideFlag标记的主UI（仅对主UI有用）
    }

    public class UIConfigData
    {
        public readonly string prefabPath;
        public readonly Type script;
        public readonly EUIOption options;
        public readonly int order;

        public UIConfigData(Type script, string prefabPath, EUIOption options = EUIOption.eInvalid, int order = 0)
        {
            this.prefabPath = prefabPath;
            this.options = options;
            this.script = script;
            this.order = order;
        }
    }
}
