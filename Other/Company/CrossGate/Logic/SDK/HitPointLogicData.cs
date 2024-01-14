using Lib.AssetLoader;
using Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class HitPointDataBase
    {
        public ulong role_id;
        public string role_name;
        public uint role_level;
        public int account_type;
        public long timestamp;
        public int instance_id;
        public int zone_id;
        public int server_id_active;
        public int online;
        public string test_id;

        public void AppendBaseData()
        {
            this.role_id = Sys_Role.Instance.RoleId;
            this.role_name = Sys_Role.Instance.sRoleName;
            this.role_level = Sys_Role.Instance.Role.Level;
            this.account_type = 0;//压测为1,正常用的为0
            this.timestamp = TimeHelper.ClientNowSeconds();
            this.instance_id = Sys_Login.Instance.selectedServerId;
            this.zone_id = VersionHelper.ZoneId;
            this.server_id_active = Sys_Login.Instance.selectedServerId;
            this.test_id = CSVParam.Instance.GetConfData(1364).str_value;
            if (Net.NetClient.Instance.eNetStatus == Net.NetClient.ENetState.Connected)
                this.online = 1;
            else
                this.online = 0;
        }
    }

    /// <summary>
    /// 新手追踪埋点
    /// </summary>
    public class HitPointNewTrace : HitPointDataBase
    {
        public static string Key = "newtrace";

        //public string scene_id = string.Empty;
        //public string total_online_duration = string.Empty;
        //public string team_status = string.Empty;
        //public string inbattle = string.Empty;
        //public string main_task_id = string.Empty;
        //public string branch_task_id = string.Empty;
        //public string current_task_id = string.Empty;
        //public string coordinate_x = string.Empty;
        //public string coordinate_z = string.Empty;
        //public string fps = string.Empty;

        public uint scene_id = 0;
        public uint total_online_duration = 0;
        public int team_status = 0;
        public int inbattle = 0;
        public uint main_task_id = 0;
        public string branch_task_id;
        public uint current_task_id = 0;
        public int coordinate_x = 0;
        public int coordinate_z = 0;
        public int fps = 0;
    }

    /// <summary>
    /// 页面展示埋点
    /// </summary>
    public class HitPointShowEvent : HitPointDataBase
    {
        public static string Key = "showevent";

        public string page_id = string.Empty;
        public string page_type = string.Empty;
        public string type = string.Empty;
        public string ending = string.Empty;
        public uint duration = 0;
        public uint option = 0;
        public int button_id = 0;
    }

    /// <summary>
    /// 按钮事件埋点
    /// </summary>
    public class HitPointClickEvent : HitPointDataBase
    {
        public static string Key = "clickevent";

        public string page_type = string.Empty;
        public string page_id = string.Empty;
        public string button_id = string.Empty;        
    }

    /// <summary>
    /// Npc对话埋点
    /// </summary>
    public class HitPointNpcDialog : HitPointDataBase
    {
        public static string Key = "npcdialogue";

        public uint scene_id = 0;
        public uint npc_id = 0;
        public uint online_duration;
    }

    /// <summary>
    /// cutscene埋点
    /// </summary>
    public class HitPointCutScene : HitPointDataBase
    {
        public static string Key = "plot";

        public string plot_id = string.Empty;
        public string stage = string.Empty;
        public string is_ff = string.Empty;
        public string is_skip = string.Empty;
        public string scene_id = string.Empty;
        public string plot_duration = string.Empty;

        public HitPointCutScene() { AppendBaseData(); }
    }

    /// <summary>
    /// 跳转埋点
    /// </summary>
    public class HitPointTeleporter : HitPointDataBase
    {
        public static string Key = "teleporter";

        public string bfscene_id = string.Empty;
        public string scene_id = string.Empty;
        public string result = string.Empty;
        public string online_duration = string.Empty;
        public string main_task_id = string.Empty;
        public string branch_task_id = string.Empty;

        public HitPointTeleporter() { AppendBaseData(); }
    }

    /// <summary>
    /// 引导埋点
    /// </summary>
    public class HitPointGuide : HitPointDataBase
    {
        public static string Key = "guide";

        public uint guideId;
        public int end;
        public uint last_id;
        public uint if_force;        
    }

    /// <summary>
    /// 帧数埋点
    /// </summary>
    public class HitPointFPSEvent 
    {
        public static string Key = "appFps";

        public int fps;
        public int instance_id;
    }

    /// <summary>
    /// 网络延迟埋点
    /// </summary>
    public class HitPointNetWorkEvent
    {
        public static string Key = "appNetwork";

        public long delay_time;
        public string net_address;
        public string ping_type;
        public string port;
        public int instance_id;        
    }

    /// <summary>
    /// android 跑分埋点
    /// </summary>
    public class HitPointPerformanceScore
    {
        public string device_model;
        public int recommend_quality;
        public int performance_score;
        public string system_version;
        public string graphics_device_version;
        public int is_emulator;
    }

    /// <summary>
    /// GameKey埋点 
    /// </summary>
    public class HitPointGameKey
    {
        public static string Key = "GameKey";
        public string gameKey;
        public string eventTag;
        public string param;
    }
}

