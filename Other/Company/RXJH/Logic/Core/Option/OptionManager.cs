using Framework;
using Lib.Core;
using Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Logic.Core
{
    public enum EQuality
    {
        Low = 0,
        Middle = 1,
        High = 2,
        Custom,
    }

    public enum EPostProcessQuality
    {
        Close = 0,
        Low = 1,
        Middle = 2,
        High = 3,
    }

    //TODO:还未添加默认设置
    public class OptionManager : TSingleton<OptionManager>
    {
        public enum EOptionID
        {
            LocalOptionStart = 0,
            Quality = 0,
            FilterStyle = 1,                    //滤镜
            LightEffect = 2,                    //光效
            //HDR = 3,
            FrameRate = 4,                      //帧率
            RenderShadow = 5,                   //阴影
            RoleCount = 6,                      //同屏角色数量
            //AdditionalLights = 7,             //附加光源
            ResolutionScale = 8,                //分辨率
            GrassScale = 9,                     //植被
            SceneScale = 10,                    //场景
            PCAspectRatioOpt = 11,              //PC分辨率切换                       
            LocalOptionEnd,

            ServerOptionStart = 1000,
            BGMValue = 1000,                    //音乐音量 //特殊处理 因为背景声音和音效在账号登录前就要用到，所以多存一份在本地
            SoundValue = 1001,                  //音效音量 //特殊处理 因为背景声音和音效在账号登录前就要用到，所以多存一份在本地
            SystemVoiceValue = 1002,            //系统语音音量
            PlayerVoiceValue = 1003,            //玩家语音音量
            AbandonAutoPet = 1004,              //保留宠物档位设置
            AutoPetCatchCardQuality = 1005,     //使用宠物封印卡
            ChatSimplifyDisplayFlag = 1006,     //精简界面消息接受flag
            ChatSystemChannelShow = 1007,       //系统界面显示 喇叭/个人
            VoiceLanguage = 1008,
            ServerOptionEnd,

            ServerToggleStart = 2000,
            BGM = 2000,                         //音乐 //特殊处理 因为背景声音和音效在账号登录前就要用到，所以多存一份在本地
            Sound = 2001,                       //音效 //特殊处理 因为背景声音和音效在账号登录前就要用到，所以多存一份在本地
            SystemVoice = 2002,                 //系统语音
            PlayerVoice = 2003,                 //玩家语音
            AutoPlayWorld = 2004,               //自动播放世界语音
            AutoPlayLocal = 2005,               //自动播放当前语音
            AutoPlayGuild = 2006,               //自动播放家族语音
            AutoPlayTeam = 2007,                //自动播放队伍语音
            AutoPlayIfWifi = 2008,              //仅wifi下播放
            RefusalToTeam = 2009,               //拒绝组队邀请
            RefusalToGuild = 2010,              //拒绝家族邀请
            RefusalToStrangeMsg = 2011,         //拒绝陌生人消息
            RefusalToCompete = 2012,            //拒绝切磋
            AutoCatchPet = 2013,                //自动抓宠
           // AutoBuyPetCatchCard = 2014,         //自动购买封印卡
            RefusalToTeamApplyTips = 2015,      //拒绝组队请求提示
            AutoHangup = 2016,                  //自动挂机
            AutoMatchWhenOnlineHangup = 2017,   //在线挂机匹配
            OfflineHangup = 2018,               //离线挂机
            FightBubble = 2019,                 //战斗气泡
            SceneBubble = 2020,                 //场景气泡
            UsePcStyleEnterFight = 2021,        //使用端游进战斗动画
            ClosePowerSaving = 2022,            //关闭省电
            SettingBitNtfGuildParty = 2023,     //家族酒会推送
            SettingBitNtfArena = 2024,          //荣耀竞技场推送
            SettingBitNtfSurvival = 2025,       //生存竞技场推送
            SettingBitNtfGuildBoss = 2026,      //家族BOSS(牛鬼来袭)推送
            SettingBitNtfGuildPet = 2027,       //家族兽推送
            SettingBitNtfGuildResBattle = 2028, //家族资源战推送
            TriedPointProtection = 2029,        //挂机疲劳保护
            OpenAutoRecode = 2030,           //开启自动录像
            SettingAchievementNotice = 2031,        //成就达成通知
            TutorMessageBag=2032,                //导师邀请开关
            ServerToggleEnd,
        }

        public enum EEvents : int
        {
            OptionValueChange,  //玩家设置的值变更
            OptionFinalChange,  //设置的实际最终值变更
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        private int[] _DisplayRoleCounts = new int[3] { 10, 20, 30 };
        public int DisplayRoleCount { get; private set; }

        private ulong _OptionServerToggles;     //用于对应服务器数据Bit位
        private int[] _OptionServerValues;      //用于对应服务器数据数组索引
        private int[] _OptionLocalValues;

        //外部覆盖
        private Dictionary<int, int> _OptionOverrides = new Dictionary<int, int>();
        //省电模式覆盖
        private Dictionary<int, int> _OptionEnergySaving = new Dictionary<int, int>();
        public List<int> mFrameRates = null;

        private bool firstSetting = false;
        public EQuality RecommendQuality = EQuality.Low;
        public int nPerformanceScore = 0;

        private int _overridePostProcess;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitFlag">0~31</param>
        /// <param name="onoff"></param>
        public void SetOverridePostProcess(int bitFlag, bool onoff)
        {
            int overridePostProcess = _overridePostProcess;
            if (onoff)
            {
                _overridePostProcess |= 1 << bitFlag;
            }
            else
            {
                _overridePostProcess &= (~(1 << bitFlag));
            }
            if (overridePostProcess != _overridePostProcess)
            {
                RefreshLightEffect();
            }
        }

        private bool bEnergySaving;
        private bool bReduceFrameRate;
        private bool bOptionServerDirty;
        private bool bOptionLocalDirty;

        //切换低帧率模式，比如全屏UI
        public void SwitchReduceFrameRate(bool use)
        {
            if (bReduceFrameRate != use)
            {
                bReduceFrameRate = use;
                RefreshFrameRate();
            }
        }

        //切换省电模式
        public void SwitchEnergySaving(bool use)
        {
            if (bEnergySaving != use)
            {
                bEnergySaving = use;
                RefreshFrameRate();
                RefreshAll();
                RefreshAllSound();
            }
        }

        public void Init()
        {            
            _OptionLocalValues = new int[(int)EOptionID.LocalOptionEnd - (int)EOptionID.LocalOptionStart];
            _OptionServerValues = new int[(int)EOptionID.ServerOptionEnd - (int)EOptionID.ServerOptionStart];
            _OptionServerToggles = 0;
            mFrameRates = new List<int>();

#if UNITY_IOS
            mFrameRates.Add(60);
            mFrameRates.Add(60);
            mFrameRates.Add(30);
#else
            //分析可用帧率
    #if UNITY_EDITOR
            mFrameRates.Add(-1);
    #else
            mFrameRates.Add(Screen.currentResolution.refreshRate);
    #endif
            int screenRefreshRate = Screen.currentResolution.refreshRate;
            int refreshRate = screenRefreshRate;
            mFrameRates.Add(refreshRate);
            int i = 2;
            refreshRate = screenRefreshRate / i;
            while (refreshRate >= 23)
            {
                mFrameRates.Add(refreshRate);
                ++i;
                refreshRate = screenRefreshRate / i;
            }
#endif

            //mFrameRates.Add(60);
            //mFrameRates.Add(30);
            int defaultRefreshRateIndex = mFrameRates.Count - 1;

            //再读取本地配置 和 服务器配置前 初始化原始值
            //自动匹配质量
            //Recommend = AutoCheckQuality();

            //读取本地设置
            if (Read("Shared/Options"))
            {
                //如果是本地读取的就不要在写入
                bOptionLocalDirty = false;
            }
            else
            {
                //没有就初始化
                InitLocalValues();
                firstSetting = true;
            }

            //此时初始化的值不同步到服务器
            bOptionServerDirty = false;

            //省电模式配置添加
            //画面相关配置
            _OptionEnergySaving.Add((int)EOptionID.LightEffect, 0);
            //_OptionEnergySaving.Add((int)EOptionID.FrameRate, 10);//特殊处理过了
            _OptionEnergySaving.Add((int)EOptionID.RenderShadow, 0);
            _OptionEnergySaving.Add((int)EOptionID.RoleCount, 0);
            _OptionEnergySaving.Add((int)EOptionID.ResolutionScale, 0);
            _OptionEnergySaving.Add((int)EOptionID.GrassScale, 0);
            _OptionEnergySaving.Add((int)EOptionID.SceneScale, 0);
            //声音相关配置
            _OptionEnergySaving.Add((int)EOptionID.BGMValue, 0);
            _OptionEnergySaving.Add((int)EOptionID.SoundValue, 0);
            _OptionEnergySaving.Add((int)EOptionID.SystemVoiceValue, 0);
            //_OptionEnergySaving.Add((int)EOptionID.PlayerVoiceValue, 0);

            //刷新配置
            RefreshAll();
            RefreshAllSound();
            //注册设置回调
            RegisterEvent(true);
        }

        public void InitDisplayRoleCounts()
        {
            List<int> datas = new List<int>(3);//ReadHelper.ReadArray_ReadInt(CSVParam.Instance.GetConfData(976).str_value, '|');
            _DisplayRoleCounts[0] = datas[0];
            _DisplayRoleCounts[1] = datas[1];
            _DisplayRoleCounts[2] = datas[2];

            RefreshRoleCount();
        }

#region 执行设置逻辑(部分在这个脚本处理的设置)
        //注册设置变更消息(部分在这个脚本处理的设置)
        private void RegisterEvent(bool bAdd)
        {
            if (bAdd)
            {
                CameraManager.onCameraChange += OnCameraChange;
            }
            else
            {
                if (CameraManager.onCameraChange != null)
                {
                    CameraManager.onCameraChange -= OnCameraChange;
                }
            }
            //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.SettingNtf, OnSettingNtf, CmdRoleSettingNtf.Parser);
        }
        private void OnSettingNtf(NetMsg msg)
        {
            //如果位标记为0说明，没有初始化过
            //初始化过客户端会设置首位为1
            //CmdRoleSettingNtf ntf = NetMsgUtil.Deserialize<CmdRoleSettingNtf>(CmdRoleSettingNtf.Parser, msg);
            //ulong v = ntf.Bitsettings;
            //if (v == 0ul)
            //{
            //    //没初始化过的话 为服务端同步一次当前数据
            //    InitServerValues();
            //    InitServerToggles();
            //    bOptionServerDirty = true;
            //}
            //else
            //{
            //    int count = (int)EOptionID.ServerToggleEnd - (int)EOptionID.ServerToggleStart;
            //    for (int i = 0; i < count; ++i)
            //    {
            //        int optionID = (int)EOptionID.ServerToggleStart + i;
            //        _SetValue(optionID, (int)(1ul & (v >> i)));
            //    }
            //
            //    Google.Protobuf.Collections.RepeatedField<int> datas = ntf.ValueSettings;
            //    count = datas.Count;
            //    for (int i = 0; i < count; ++i)
            //    {
            //        int optionID = (int)EOptionID.ServerOptionStart + i;
            //        _SetValue(optionID, datas[i]);
            //    }
            //}
            //Sys_SettingHotKey.Instance.eventEmitter.Trigger<uint>(Sys_SettingHotKey.Events.UpdatePC_SettingVersion, ntf.PcSettingNum);
        }
        private void ReqSetting()
        {
            //将首位标记为0 用于判断服务器数据是否为已初始化的数据
            _OptionServerToggles |= (1ul << 63);

            //CmdRoleSettingReq req = new CmdRoleSettingReq();
            //req.Bitsettings = _OptionServerToggles;
            //req.ValueSettings.Add(_OptionServerValues);
            //NetClient.Instance.SendMessage((ushort)CmdRole.SettingReq, req);
        }

        public void InitLocalValues()
        {
            //SetInt(EOptionID.Quality, (int)Recommend, false);

            SetInt(EOptionID.FrameRate, 0, false);
            SetInt(EOptionID.RoleCount, 2, false);
            SetInt(EOptionID.ResolutionScale, 2, false);
            SetInt(EOptionID.PCAspectRatioOpt, 1, false);

            //特殊处理 因为背景声音和音效在账号登录前就要用到，所以多存一份在本地
            _SetValue((int)EOptionID.BGM, 1);
            _SetValue((int)EOptionID.Sound, 1);
            _SetValue((int)EOptionID.BGMValue, 500);
            _SetValue((int)EOptionID.SoundValue, 1000);
        }

        public void SetRecommendQuality(int qualityLevel, int score)
        {
            nPerformanceScore = score;

            qualityLevel = Mathf.Clamp(qualityLevel, (int)EQuality.Low, (int)EQuality.High);
            RecommendQuality = (EQuality)qualityLevel;

            if (firstSetting)
            {
                SetInt(EOptionID.Quality, (int)RecommendQuality, false);
                firstSetting = false;
            }
        }

        private void InitServerValues()
        {
            _SetValue((int)EOptionID.BGMValue, 500);
            _SetValue((int)EOptionID.SoundValue, 1000);
            _SetValue((int)EOptionID.SystemVoiceValue, 1000);
            _SetValue((int)EOptionID.PlayerVoiceValue, 1000);
            _SetValue((int)EOptionID.AbandonAutoPet, 0);
            _SetValue((int)EOptionID.AutoPetCatchCardQuality, 0);
            _SetValue((int)EOptionID.ChatSimplifyDisplayFlag, 0xffff);
            _SetValue((int)EOptionID.ChatSystemChannelShow, 1);
        }
        private void InitServerToggles()
        {
            _SetValue((int)EOptionID.BGM, 1);
            _SetValue((int)EOptionID.Sound, 1);
            _SetValue((int)EOptionID.SystemVoice, 1);
            _SetValue((int)EOptionID.PlayerVoice, 1);
            _SetValue((int)EOptionID.AutoPlayWorld, 0);
            _SetValue((int)EOptionID.AutoPlayLocal, 0);
            _SetValue((int)EOptionID.AutoPlayGuild, 0);
            _SetValue((int)EOptionID.AutoPlayTeam, 0);
            _SetValue((int)EOptionID.AutoPlayIfWifi, 1);
            _SetValue((int)EOptionID.RefusalToTeam, 0);
            _SetValue((int)EOptionID.RefusalToGuild, 0);
            _SetValue((int)EOptionID.RefusalToStrangeMsg, 0);
            _SetValue((int)EOptionID.RefusalToCompete, 0);
            _SetValue((int)EOptionID.AutoCatchPet, 1);
           // _SetValue((int)EOptionID.AutoBuyPetCatchCard, 0);  //暂时屏蔽

            _SetValue((int)EOptionID.AutoHangup, 1);
            _SetValue((int)EOptionID.AutoMatchWhenOnlineHangup, 0);
            _SetValue((int)EOptionID.TriedPointProtection, 0);
            _SetValue((int)EOptionID.OfflineHangup, 1);

            _SetValue((int)EOptionID.FightBubble, 0);
            _SetValue((int)EOptionID.SceneBubble, 0);

            _SetValue((int)EOptionID.SettingBitNtfGuildParty, 1);
            _SetValue((int)EOptionID.SettingBitNtfArena, 1);
            _SetValue((int)EOptionID.SettingBitNtfSurvival, 1);
            _SetValue((int)EOptionID.SettingBitNtfGuildBoss, 1);
            _SetValue((int)EOptionID.SettingBitNtfGuildPet, 0);
            _SetValue((int)EOptionID.SettingBitNtfGuildResBattle, 1);
            _SetValue((int)EOptionID.OpenAutoRecode, 0);
            _SetValue((int)EOptionID.SettingAchievementNotice, 0);
            _SetValue((int)EOptionID.TutorMessageBag, 0);
        }

        private bool Read(string relativePath)
        {
            string path = Lib.AssetLoader.AssetPath.GetPersistentFullPath(relativePath);
            if (System.IO.File.Exists(path))
            {
                System.IO.FileStream fs = System.IO.File.OpenRead(path);
                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);

                while (br.BaseStream.Position + 8 <= br.BaseStream.Length)
                {
                    int k = br.ReadInt32();
                    int v = br.ReadInt32();

                    SetInt(k, v, false);
                }

                br.Dispose();
                br.Close();
                fs.Dispose();
                fs.Close();

                return true;
            }
            return false;
        }
        public void Write(string relativeDir, string relativePath)
        {
            string dir = Lib.AssetLoader.AssetPath.GetPersistentFullPath(relativeDir);
            string path = dir + "/" + relativePath;

            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            System.IO.BinaryWriter br = new System.IO.BinaryWriter(fs);            

            //如果是自定义质量则都写入
            int quality = GetInt(EOptionID.Quality, false);
            if (quality == (int)EQuality.Custom)
            {
                for (int i = (int)EOptionID.LocalOptionStart; i < (int)EOptionID.LocalOptionEnd; ++i)
                {
                    br.Write(i);
                    br.Write(GetInt(i, false));
                }
            }
            else
            {
                //否则只写入质量以及非质量控制项

                br.Write((int)EOptionID.Quality);
                br.Write(quality);

                br.Write((int)EOptionID.FrameRate);
                br.Write(GetInt(EOptionID.FrameRate, false));

                br.Write((int)EOptionID.ResolutionScale);
                br.Write(GetInt(EOptionID.ResolutionScale, false));
            }

            //特殊处理 因为背景声音和音效在账号登录前就要用到，所以多存一份在本地
            br.Write((int)EOptionID.BGM);
            br.Write(GetInt(EOptionID.BGM, false));

            br.Write((int)EOptionID.BGMValue);
            br.Write(GetInt(EOptionID.BGMValue, false));

            br.Write((int)EOptionID.Sound);
            br.Write(GetInt(EOptionID.Sound, false));

            br.Write((int)EOptionID.SoundValue);
            br.Write(GetInt(EOptionID.SoundValue, false));

            br.Dispose();
            br.Close();
            fs.Dispose();
            fs.Close();
        }

        public void WriteIfDirty()
        {
            if (bOptionLocalDirty)
            {
                bOptionLocalDirty = false;
                Write("Shared", "Options");
            }

            if (bOptionServerDirty)
            {
                bOptionServerDirty = false;
                ReqSetting();
            }
        }

        private void OnCameraChange()
        {
            RefreshLightEffect();
            RefreshShadow();
        }

        private EQuality AutoCheckQuality()
        {
#if UNITY_ANDROID
            if (!SystemInfo.supportsInstancing)
            {
                return EQuality.Low;
            }

            if (SystemInfo.graphicsMemorySize < 1000)
            {
                return EQuality.Low;
            }

            if (SystemInfo.systemMemorySize < 3000)
            {
                return EQuality.Low;
            }

            if(SystemInfo.processorCount <= 4)
            {
                return EQuality.Low;
            }

            if (SystemInfo.systemMemorySize < 6000)
            {
                return EQuality.Middle;
            }

            if (SystemInfo.processorCount < 8)
            {
                return EQuality.Middle;
            }            

            if (!RenderExtensionSetting.SupportsDepthCopy())
            {
                return EQuality.Middle;
            }
#endif
            return EQuality.High;
        }
#endregion

        public int GetInt(int optionID, bool useOverride = true)
        {
            if (optionID < 0)
                return 0;

            int rlt = 0;

            if (useOverride && bEnergySaving && _OptionEnergySaving.TryGetValue(optionID, out int rltEnergySaving))
            {
                rlt = rltEnergySaving;
            }
            else if (useOverride && _OptionOverrides.TryGetValue(optionID, out int rltOverride))
            {
                rlt = rltOverride;
            }
            else
            {
                int group = optionID / 1000;
                int index = optionID % 1000;

                switch (group)
                {
                    case 0:
                        {
                            if (index < _OptionLocalValues.Length)
                            {
                                rlt = _OptionLocalValues[index];
                            }
                        }
                        break;
                    case 1:
                        {
                            if (index < _OptionServerValues.Length)
                            {
                                rlt = _OptionServerValues[index];
                            }
                        }
                        break;
                    case 2:
                        rlt = (int)((_OptionServerToggles >> index) & 1ul);
                        break;
                    default:
                        break;
                }
            }

            return rlt;
        }
        public bool GetBoolean(int optionID, bool useOverride = true)
        {
            return GetInt(optionID, useOverride) == 1;
        }
        public float GetFloat(int optionID, bool useOverride = true)
        {
            return GetInt(optionID, useOverride) / 1000f;
        }

        public int GetInt(EOptionID optionID, bool useOverride = true)
        {
            return GetInt((int)optionID, useOverride);
        }
        public bool GetBoolean(EOptionID optionID, bool useOverride = true)
        {
            return GetBoolean((int)optionID, useOverride);
        }
        public float GetFloat(EOptionID optionID, bool useOverride = true)
        {
            return GetFloat((int)optionID, useOverride);
        }

        private bool _SetValue(int optionID, int v)
        {
            if (optionID < 0)
                return false;

            bool hasChange = false;

            int group = optionID / 1000;
            int index = optionID % 1000;
            switch (group)
            {
                case 0:
                    {
                        if (index < _OptionLocalValues.Length && _OptionLocalValues[index] != v)
                        {
                            _OptionLocalValues[index] = v;
                            bOptionLocalDirty = true;
                            hasChange = true;
                        }
                    }
                    break;
                case 1:
                    {
                        if (index < _OptionServerValues.Length && _OptionServerValues[index] != v)
                        {
                            _OptionServerValues[index] = v;
                            bOptionServerDirty = true;
                            hasChange = true;
                        }
                    }
                    break;
                case 2:
                    {
                        ulong optionServerToggles = _OptionServerToggles;
                        if (v == 1)
                        {
                            optionServerToggles |= (1ul << index);
                        }
                        else
                        {
                            optionServerToggles &= ~(1ul << index);
                        }

                        if (optionServerToggles != _OptionServerToggles)
                        {
                            _OptionServerToggles = optionServerToggles;
                            bOptionServerDirty = true;
                            hasChange = true;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (hasChange)
            {
                //特殊处理 因为背景声音和音效在账号登录前就要用到，所以多存一份在本地
                if (optionID == (int)EOptionID.BGM || optionID == (int)EOptionID.BGMValue || optionID == (int)EOptionID.Sound || optionID == (int)EOptionID.SoundValue)
                {
                    bOptionLocalDirty = true;
                }

                //设置值有变更
                eventEmitter.Trigger<int>(EEvents.OptionValueChange, optionID);

                //实际最终结果有变更
                if (!_OptionOverrides.ContainsKey(optionID))
                {
                    RefreshOption((EOptionID)optionID);
                    eventEmitter.Trigger<int>(EEvents.OptionFinalChange, optionID);
                }

                return true;
            }
            return false;
        }

        public void SetInt(int optionID, int v, bool setOverride)
        {
            if (setOverride)
            {
                if (!_OptionOverrides.TryGetValue(optionID, out int old) || old != v)
                {
                    _OptionOverrides[optionID] = v;

                    RefreshOption((EOptionID)optionID);
                    eventEmitter.Trigger<int>(EEvents.OptionFinalChange, optionID);
                }
            }
            else
            {
                if (_SetValue(optionID, v))
                {
                    if (optionID == (int)EOptionID.Quality)
                    {
                        _SetQuality(GetInt(EOptionID.Quality, false));
                    }
                    else if (optionID > (int)EOptionID.LocalOptionStart && optionID < (int)EOptionID.LocalOptionEnd)
                    {
                        if (optionID != (int)EOptionID.FrameRate && optionID != (int)EOptionID.ResolutionScale)
                        {
                            _SetValue((int)EOptionID.Quality, (int)EQuality.Custom);
                        }
                    }
                }
            }
        }
        public void SetBoolean(int optionID, bool v, bool setOverride)
        {
            SetInt(optionID, v ? 1 : 0, setOverride);
        }
        public void SetFloat(int optionID, float v, bool setOverride)
        {
            SetInt(optionID, (int)(v * 1000f), setOverride);
        }
        public void SetInt(EOptionID optionID, int v, bool setOverride)
        {
            SetInt((int)optionID, v, setOverride);
        }
        public void SetBoolean(EOptionID optionID, bool v, bool setOverride)
        {
            SetInt((int)optionID, v ? 1 : 0, setOverride);
        }
        public void SetFloat(EOptionID optionID, float v, bool setOverride)
        {
            SetInt((int)optionID, (int)(v * 1000f), setOverride);
        }
        public void CancelOverride(EOptionID optionID)
        {
            if (_OptionOverrides.TryGetValue((int)optionID, out int old))
            {
                _OptionOverrides.Remove((int)optionID);

                if (old != GetInt(optionID, false))
                {
                    RefreshOption((EOptionID)optionID);
                    eventEmitter.Trigger<int>(EEvents.OptionFinalChange, (int)optionID);
                }
            }
        }

        private void _SetQuality(int quality)
        {
            bool resolutionScale = false;

#if UNITY_ANDROID
            resolutionScale = !SDKManager.SDKISEmulator();
#endif

            switch (quality)
            {
                case 0:
                    {
                        _SetValue((int)EOptionID.LightEffect, 0);
                        _SetValue((int)EOptionID.RenderShadow, 0);
                        _SetValue((int)EOptionID.ResolutionScale, resolutionScale ? 1 : 2);
                        _SetValue((int)EOptionID.GrassScale, 0);
                        _SetValue((int)EOptionID.SceneScale, 0);
                        _SetValue((int)EOptionID.RoleCount, 0);
                    }
                    break;
                case 1:
                    {
                        _SetValue((int)EOptionID.LightEffect, 0);
                        _SetValue((int)EOptionID.RenderShadow, 1);//
                        _SetValue((int)EOptionID.ResolutionScale, resolutionScale ? 1 : 2);
                        _SetValue((int)EOptionID.GrassScale, 1);//
                        _SetValue((int)EOptionID.SceneScale, 1);//
                        _SetValue((int)EOptionID.RoleCount, 1);
                    }
                    break;
                case 2:
                    {
                        _SetValue((int)EOptionID.LightEffect, 2);
                        _SetValue((int)EOptionID.RenderShadow, 2);
                        _SetValue((int)EOptionID.ResolutionScale, 2);//2
                        _SetValue((int)EOptionID.GrassScale, 2);
                        _SetValue((int)EOptionID.SceneScale, 2);//2
                        _SetValue((int)EOptionID.RoleCount, 2);
                    }
                    break;
                //case 3:
                //    {
                //        _SetValue((int)EOptionID.LightEffect, 2);//
                //        _SetValue((int)EOptionID.RenderShadow, 2);                        
                //        _SetValue((int)EOptionID.ResolutionScale, 2);
                //        _SetValue((int)EOptionID.GrassScale, 2);
                //        _SetValue((int)EOptionID.SceneScale, 2);
                //    }
                //    break;
                default:
                    break;
            }
        }
        private void RefreshOption(EOptionID optionID)
        {
            switch (optionID)
            {
                case EOptionID.Quality:
                    RefreshAll();
                    break;
                case EOptionID.LightEffect:
                    RefreshLightEffect();
                    break;
                case EOptionID.RenderShadow:
                    RefreshShadow();
                    break;
                case EOptionID.ResolutionScale:
                    RefreshResolutionScale();
                    break;
                case EOptionID.GrassScale:
                    RefreshGrassScale();
                    break;
                case EOptionID.SceneScale:
                    RefreshSceneScale();
                    break;
                case EOptionID.FrameRate:
                    RefreshFrameRate();
                    break;
                case EOptionID.BGM:
                case EOptionID.BGMValue:
                    {
                        //AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.BGM, GetBoolean(EOptionID.BGM) ? GetFloat(EOptionID.BGMValue) : 0);
                    }
                    break;
                case EOptionID.Sound:
                case EOptionID.SoundValue:
                    {
                       // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.UISound, GetBoolean(EOptionID.Sound) ? GetFloat(EOptionID.SoundValue) : 0);
                       // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.SkillSound, GetBoolean(EOptionID.Sound) ? GetFloat(EOptionID.SoundValue) : 0);
                       // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.SceneSound, GetBoolean(EOptionID.Sound) ? GetFloat(EOptionID.SoundValue) : 0);
                       // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.VideoSound, GetBoolean(EOptionID.Sound) ? GetFloat(EOptionID.SoundValue) : 0);
                    }
                    break;
                case EOptionID.SystemVoice:
                case EOptionID.SystemVoiceValue:
                    {
                       // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.NPCSound, GetBoolean(EOptionID.SystemVoice) ? GetFloat(EOptionID.SystemVoiceValue) : 0);
                    }
                    break;
                case EOptionID.PlayerVoice:
                case EOptionID.PlayerVoiceValue:
                    {

                    }
                    break;
                case EOptionID.RoleCount:
                    RefreshRoleCount();                    
                    break;
                default:
                    break;
            }
        }

        private void RefreshGrassScale()
        {            
            int v = GetInt(EOptionID.GrassScale);
            RenderExtensionSetting.bUsageGrass = v > 0;
            RenderExtensionSetting.SetGrassInteractive(v > 1);
        }
        private void RefreshSceneScale()
        {
            int v = GetInt(EOptionID.SceneScale);
            RenderExtensionSetting.nSceneMaxLOD = v;
            RenderExtensionSetting.bUsageOutLine = RenderExtensionSetting.UseDepthTexture = v > 1;
            UniversalRenderPipeline.asset.msaaSampleCount = v * 2;
            QualitySettings.SetQualityLevel(v);
        }
        private void RefreshLightEffect()
        {
            int v = GetInt(EOptionID.LightEffect);
            CameraManager.SetActivePostProcess(v > 0 || _overridePostProcess > 0);
            UniversalRenderPipeline.asset.supportsHDR = v > 1;
            UniversalRenderPipeline.asset.maxAdditionalLightsCount = (SystemInfo.supportsInstancing && v > 1) ? 1 : 0;
        }
        private void RefreshShadow()
        {
            int v = GetInt(EOptionID.RenderShadow);
            //UniversalRenderPipeline.asset.mainLightShadowmapResolution = v == 2 ? 2048 : 1024;
            CameraManager.SetActiveShadow(v > 0);
        }
        private void RefreshResolutionScale()
        {
            //0.5 0.75 1
            UniversalRenderPipeline.asset.renderScale = Mathf.Min(1, GetInt(EOptionID.ResolutionScale) * 0.25f + 0.5f);
        }
        private void RefreshFrameRate()
        {
            if (bEnergySaving)
            {
                Application.targetFrameRate = 10;
            }
            else if (bReduceFrameRate)
            {
                Application.targetFrameRate = 30;
            }
            else
            {
                int value = GetInt((int)EOptionID.FrameRate);                
                Application.targetFrameRate = mFrameRates[Mathf.Max(mFrameRates.Count - (value + 1), 0)];
            }
        }
        private void RefreshAll()
        {
            RefreshGrassScale();
            RefreshSceneScale();
            RefreshLightEffect();
            RefreshShadow();
            RefreshResolutionScale();
            RefreshRoleCount();
        }
        private void RefreshAllSound()
        {
           // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.BGM, GetBoolean(EOptionID.BGM) ? GetFloat(EOptionID.BGMValue) : 0);
           // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.UISound, GetBoolean(EOptionID.Sound) ? GetFloat(EOptionID.SoundValue) : 0);
           // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.SkillSound, GetBoolean(EOptionID.Sound) ? GetFloat(EOptionID.SoundValue) : 0);
           // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.SceneSound, GetBoolean(EOptionID.Sound) ? GetFloat(EOptionID.SoundValue) : 0);
           // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.NPCSound, GetBoolean(EOptionID.Sound) ? GetFloat(EOptionID.SoundValue) : 0);
           // AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.VideoSound, GetBoolean(EOptionID.Sound) ? GetFloat(EOptionID.SoundValue) : 0);
        }
        private void RefreshRoleCount()
        {
            DisplayRoleCount = _DisplayRoleCounts[GetInt(EOptionID.RoleCount)];
        }
    }
}