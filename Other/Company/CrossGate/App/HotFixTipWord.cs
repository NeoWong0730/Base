using Lib.Core;

public class HotFixTipWord
{
    public string btn_fix = "更新";
    public string btn_goto = "前往";
    public string btn_quit = "退出";
    public string btn_try = "重试";
    public string network_error = "当前网络设备异常，请先检查网络({0})";
    public string http_error = "更新失败，请检查您的网络环境({0})";
    public string game_http_error = "当前网络设备异常,请检查您的网络环境({0})";
   // public string remote_asset_notexist = "更新失败，请检查您的网络环境({0})";//"请求的服务器资源不存在({0})";
    public string streaming_maxversion_error = "本地大版本号解析错误";
    public string remote_notsameseries_error = "服务器端资源版本过旧，请更换服务器资源";
    public string remote_version_net_error = "获取服务器版本信息失败，请检查网络!";
    public string remote_version_info_error = "服务器版本解析错误";   
    public string remote_assetlist_error = "服务器资源列表解析错误";
    public string remote_hotfixlist_md5_error = "热更资源列表校验失败，建议重新更新资源包";
    public string maximal_version = "游戏版本过旧，请去商店下载游戏最新版本!";//"需要重新下载新的客户端\n版本{0}.{1}";
    public string verify_error = "更新校验失败，建议重新更新资源包";
    public string verify_hotfixSeries_error = "该安装包与服务器热更资源包不匹配，请使用新的安装包";//,如若强制使用不匹配的热更新资源,可能会导致BUG!";
    public string destroy_asset = "正在清理多余资源";
    public string no_need_net = "此过程不消耗流量";
    public string success = "当前为最新版本，祝您游戏愉快";
    public string asset_check = "资源更新检测中";
    public string asset_verify = "资源校验中";
    public string asset_fix = "下载更新中";
    public string asset_load = "资源加载中";
    public string asset_download = "下载资源: [{0}/{1}], 当前速度: {2}/S";
    public string asset_download_tip = "需要更新\n资源大小:{0}\n建议在WIFI环境下更新";
    public string server_maintaining = "服务器维护中，请稍后再试!";
    public string local_memory_not_enough = "设备存储空间不足,请先清理手机";
    public string remote_asset_complete_failure = "由于尝试多次下载失败，请尝试点击修复按钮，进行重新下载";
    public string remote_asset_lost_error = "远端文件丢失,文件名：{0}";

    //1.版本更新文件下载不到时，如线上包是1.0.2，服务器的version为1.0.5，但是没去上传3.4.5的热更包，现在的提示是网络错误，应该提示：更新文件下载失败！
    //比如热更包部分文件丢失，或者上传之后个别文件md5码对不上，下载不到正确的资源的时候应该不让他下载。 提示文字可以是：xx文件下载失败，请退出游戏重新尝试下载。
}

public class HotFixTipWordManager : TSingleton<HotFixTipWordManager>
{
    public HotFixTipWord hotFixTipWord { get; private set; }

    public void Init()
    {
        hotFixTipWord = new HotFixTipWord();
    }
}
