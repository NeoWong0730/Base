using Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipSDKLogin : MonoBehaviour
{
#if UNITY_EDITOR && SKIP_SDK_Login

    //连接成功后，请求登录
    public uint Time = 1682004826;
    public string Signature = "b35b6cf3e643473d5b1fdf12077c7da2";

    //以机器人身份登入 robot_cp,玩家的userid（server的gameid）
    public string Account = "120592009db2d8e97607fb38a230182a69";
    public ulong Roleid = 1000200007694;

    //连接 游戏服的ip,端口号
    public string ServerIp = "118.89.214.151";
    public int ServerPort = 12001;
    public int ServerId = 10002;

    //首包版本号
    public string appVersion = "1.1.13";
    //渠道号 android（默认ks） ios（默认appstore）
    public string channel = "ks";
    //android用户16，ios用户32
    public int accountType = 16;

    public static SkipSDKLogin Instance;

    //KwaiGatewayZoneInfo信息获取：
    //以下由于url快手不在维护，使用快手接口获取大区，在测试环境不带sdk情况下，快手返回的数据错误，所以需要手动设置，设置前先用正式包跑下拿到正式的数据：KwaiGatewayZoneInfo，进行填充以下字段
    //热更版本号
    public string assetVersion = "1.19.3";
    public int zoneid = 20;
    public string loginHost = "https://mlbb-dir-online.game.kuaishou.com/api";
    public string hotfixUrl = "https://d1-bb.game.kspkg.com/kos/nlav10966/hotfix";


    void Start()
    {
        Instance = this;
    }
  
  
#endif

}
