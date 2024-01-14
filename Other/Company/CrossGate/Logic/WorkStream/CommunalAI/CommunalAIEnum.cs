using System.ComponentModel;

public enum CommunalAIEnum
{
	None = 0,

    #region 1-5000是node节点枚举
    [Description("模型shader渐隐")]
    ModelShaderDissolve = 1,
    [Description("模型显隐")]
    ShowModel = 2,
    [Description("等待时间")]
    WaitTime = 3,
    [Description("偏移模型位置")]
    OffsetModelPos = 4,
    #endregion

    #region 5001+是block枚举
    [Description("通用模块")]
    CommonBlock = 5001,
	#endregion
}