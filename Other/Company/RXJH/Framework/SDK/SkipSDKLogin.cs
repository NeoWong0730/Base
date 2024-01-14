using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipSDKLogin : MonoBehaviour
{
#if UNITY_EDITOR && SKIP_SDK_Login

    //连接成功后，请求登录
    public uint Time = 1659975218;
    public string Signature = "ce1450b8af92f7b3ef8ff10212863822";

    //以机器人身份登入 robot_cp,玩家的userid（server的gameid）
    public string Account = "120592009db2d8e97607fb38a24a58aa32";
    public ulong Roleid = 1000800008685;

    //连接 游戏服的ip,端口号
    public string ServerIp = "49.233.247.56";
    public int ServerPort = 12001;
    public int ServerId = 10008;

    //首包版本号
    public string appVersion = "1.7.7";
    //渠道号 android（默认ks） ios（默认appstore）
    public string channel = "ks";
    //android用户16，ios用户32
    public int accountType = 16;

    public static SkipSDKLogin Instance;
    void Start()
    {
        Instance = this;
    }
  
  
#endif

}
