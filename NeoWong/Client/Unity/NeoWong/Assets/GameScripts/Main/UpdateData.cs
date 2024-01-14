namespace GameMain
{
    /// <summary>
    /// APP更新类型
    /// </summary>
    public enum UpdateType
    {
        None = 0,

        /// <summary>
        /// 资源更新
        /// </summary>
        ResourceUpdate = 1,

        /// <summary>
        /// 底包更新
        /// </summary>
        PackageUpdate = 2,
    }

    /// <summary>
    /// 强制更新类型
    /// </summary>
    public enum UpdateStyle
    {
        None = 0,

        /// <summary>
        /// 强制(不更新无法进入游戏)
        /// </summary>
        Force = 1,

        /// <summary>
        /// 非强制(不更新可以进入游戏)
        /// </summary>
        Optional = 2,
    }

    /// <summary>
    /// 是否提示更新
    /// </summary>
    public enum UpdateNotice
    {
        None = 0,

        /// <summary>
        /// 提示
        /// </summary>
        Notice = 1,

        /// <summary>
        /// 非提示
        /// </summary>
        NoNotice = 2,
    }

    /// <summary>
    /// 版本更新数据
    /// </summary>
    public class UpdateData
    {
        /// <summary>
        /// 当前版本信息
        /// </summary>
        public string CurrentVersion;

        /// <summary>
        /// 是否底包更新
        /// </summary>
        public UpdateType UpdateType;

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public UpdateStyle UpdateStyle;

        /// <summary>
        /// 是否提示
        /// </summary>
        public UpdateNotice UpdateNotice;

        /// <summary>
        /// 热更资源地址
        /// </summary>
        public string HostServerURL;

        /// <summary>
        /// 备用热更资源地址
        /// </summary>
        public string FallbackHostServerURL;
    }
}