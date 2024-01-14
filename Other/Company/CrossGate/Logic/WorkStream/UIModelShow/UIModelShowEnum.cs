using System.ComponentModel;

public enum UIModelShowEnum
{
	None = 0,

    #region 1-5000是node节点枚举
    [Description("播放动画")]
    PlayAnimation = 1,
    [Description("等待时间")]
    WaitTime = 2,
    [Description("随机选择组")]
    RandomSelectGroup = 3,
    [Description("循环节点标记")]
    LoopStart = 4,
    [Description("返回循环节点标记")]
    BackToLoopStart = 5,
    [Description("播放动画到动画结束")]
    PlayAnimationToEnd = 6,
    [Description("监听点击模型操作")]
    ListenTouchModel = 7,
    [Description("收集数据")]
    CollectBehaveInfo = 8,
    [Description("等待动画加载完")]
    WaitAnimationsLoadOver = 9,
    [Description("显示模型")]
    ShowModel = 10,
    [Description("武器类型")]
    WeaponType = 11,
    [Description("武器模型显隐")]
    ShowWeaponModel = 12,
    [Description("武器模型是否脱离骨骼")]
    WeaponModelAwayBone = 13,
    [Description("武器模型shader渐隐")]
    WeaponModelShaderDissolve = 14,
    [Description("开关动态骨骼")]
    OpenOrCloseDynamicBone = 15,
    [Description("播放特效")]
    PlayEffect = 16,
    [Description("删除特效")]
    DelEffect = 17,

    //100-199
    [Description("控制切换模型")]
    ConrolSwitchModel = 100,
    [Description("控制旋转模型")]
    ConrolRotateModel = 101,
    [Description("控制点击模型")]
    ConrolTouchModel = 102,

    [Description("生成节点子WorkStream")]
    CreateChildWorkStream = 4000,
    #endregion

    #region 5001+是block枚举
    [Description("模型展示块")]
    UIModeShowBlock = 5001,
    [Description("模型表演块")]
    UIModeActBlock = 5002,
    #endregion
}