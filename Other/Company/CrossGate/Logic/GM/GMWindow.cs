#if DEBUG_MODE
using Framework;
using Logic;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class GMWindow : DebugWindowBase
{
    string cmd = string.Empty;
    string param = string.Empty;
    //string gmContent = string.Empty;
    string functionId = string.Empty;

    private Vector2 buttonSize = new Vector2(128, 64);

    private Vector2 scrollViewVector = Vector2.zero;
    private Vector2 gmContentPos = Vector2.zero;

    private GUIStyle debugBoxStyle;
    private bool lastToggle = true;
    private bool bgmToggle = true;
    private bool lastReconnectToggle = true;
    private bool reconnectToggle = true;
    private string[] gms;
    private int _combatBehaveType;
    private static ulong testUID = 100000;

    private bool _combatDebugFlag;
    private Vector2 _scrollPos;
    private GUIStyle _labelSt;
    private GUIStyle _buttonSt;
    private bool _selecDLogEnumFlag;
    private bool _showDLogEnumNeedAddFlag;

    private int _chatIndex;
    private bool _autoChat;

    private DebugPanel _debugPanel;

    private string powerSavingCD = "5";

    public GMWindow(int id) : base(id) { }

    public override void OnAwake()
    {
        debugBoxStyle = new GUIStyle();
        debugBoxStyle.alignment = TextAnchor.UpperLeft;
        debugBoxStyle.wordWrap = true;
        debugBoxStyle.normal.background = new Texture2D(120, 80, TextureFormat.BGRA32, false);
        debugBoxStyle.normal.textColor = Color.blue;
        gms = SplitGMOrders();
    }

    public override void WindowFunction(int id)
    {
        if (_combatDebugFlag)
        {
            OnDrawCombatInfo();
        }
        else
        {
            LeftArea();
            RightArea();
        }

        if (_autoChat)
        {
            Sys_Chat.ChatBaseInfo chatBaseInfo = new Sys_Chat.ChatBaseInfo();
            chatBaseInfo.sSenderName = "饭米粒";
            chatBaseInfo.nHeroID = Sys_Role.Instance.HeroId;
            chatBaseInfo.SenderHead = 100;
            chatBaseInfo.BackActivity = _chatIndex % 2 == 0;
            //Sys_Chat.Instance.PushMessage(ChatType.System, chatBaseInfo, $"银币{_chatIndex}个"
            //    , Sys_Chat.EMessageProcess.AddSenderName | Sys_Chat.EMessageProcess.IsPerson, Sys_Chat.EExtMsgType.Normal);
            //Sys_Chat.Instance.PushMessage(ChatType.World, chatBaseInfo, $"哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈{_chatIndex}"
            //    , Sys_Chat.EMessageProcess.AddSenderName | Sys_Chat.EMessageProcess.SimplifyAddSenderName, Sys_Chat.EExtMsgType.Normal);
            //Sys_Chat.Instance.PushMessage(ChatType.LookForTeam, chatBaseInfo, $"目标：奇怪的洞窟（3/{_chatIndex}）23-59级进组<color=#00ff00>[申请入队]</color>"
            //    , Sys_Chat.EMessageProcess.AddSenderName | Sys_Chat.EMessageProcess.SimplifyAddSenderName, Sys_Chat.EExtMsgType.Normal);
            //Sys_Chat.Instance.PushMessage(ChatType.LookForTeam, chatBaseInfo, $"目标：奇怪的洞窟（3/{_chatIndex}）23-59级进组<color=#00ff00>[申请入队]</color>"
            //    , Sys_Chat.EMessageProcess.AddSenderName | Sys_Chat.EMessageProcess.SimplifyAddSenderName, Sys_Chat.EExtMsgType.Normal);
            Sys_Chat.Instance.PushMessage(ChatType.World, chatBaseInfo, $"哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈{_chatIndex}"
                , Sys_Chat.EMessageProcess.None, Sys_Chat.EExtMsgType.Normal);
            ++_chatIndex;
        }
    }

    public void LeftArea()
    {
        GUILayout.BeginArea(new Rect(0, 20, Screen.width * 0.4f, Screen.height * 0.9f));

        if (Sys_Role.Instance.Role != null)
        {
            GUILayout.TextField("RoleID:" + Sys_Role.Instance.Role.RoleId.ToString());
        }
        if (Sys_Time.Instance != null)
        {
            uint secondsWithTimeZone = Sys_Time.Instance.GetServerTime();
            DateTime dateTime = Sys_Time.ConvertToLocalTime(secondsWithTimeZone);
            GUILayout.TextField("secondsWithTimeZone: " + secondsWithTimeZone);
            GUILayout.TextField("serverTimestamp: " + Sys_Time.Instance.GetServerTime(true));
            GUILayout.TextField("本机时区: " + dateTime.ToString());
            GUILayout.TextField("server时区: " + Sys_Time.ConvertToDatetime(secondsWithTimeZone).ToString());
            GUILayout.TextField("本机时间: " + DateTime.Now);
        }
        if (Sys_Login.Instance != null)
        {
            GUILayout.TextField("Account:" + Sys_Login.Instance.Account);
        }

        if (CombatManager.Instance != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("战Id", GUILayout.Width(30f));
            GUILayout.TextField($"{CombatManager.Instance.m_BattleId.ToString()}");
#if DEBUG_MODE
            GUILayout.Label("战类Id", GUILayout.Width(50f));
            GUILayout.TextField($"{CombatManager.Instance.GetBattleTypeId().ToString()}");
#endif
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("遇敌组", GUILayout.Width(50f));
            GUILayout.TextField($"{Sys_Fight.curMonsterGroupId.ToString()}");
            GUILayout.EndHorizontal();
        }

        scrollViewVector = GUILayout.BeginScrollView(scrollViewVector);

        if (CombatManager.Instance != null)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("战斗信息"))
            {
                _combatDebugFlag = true;
            }
            if (GUILayout.Button("退出战斗"))
            {
                Sys_Fight.Instance.EndReq();
            }
            string combatBehaveTypeStr = GUILayout.TextField($"{_combatBehaveType.ToString()}");
            if (int.TryParse(combatBehaveTypeStr, out int cbtInt))
            {
                _combatBehaveType = cbtInt;
                if (CombatConfigManager.Instance.m_BehaveType != _combatBehaveType)
                {
                    CombatConfigManager.Instance.m_BehaveType = _combatBehaveType;
                    CombatConfigManager.Instance.ResetConfigData(5);
                }
            }
            GUILayout.EndHorizontal();
        }

        bgmToggle = GUILayout.Toggle(bgmToggle, "BGM");
        if (lastToggle != bgmToggle)
        {
            lastToggle = bgmToggle;
            float volume = bgmToggle ? 1f : 0f;
            AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.BGM, volume);
        }

        reconnectToggle = GUILayout.Toggle(reconnectToggle, "开启断线重连");
        if (lastReconnectToggle != reconnectToggle)
        {
            lastReconnectToggle = reconnectToggle;
            Sys_Net.Instance.DisableReconnect(Sys_Net.EReconnectSwitchType.Debug, !lastReconnectToggle);
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("断线"))
        {
            NetClient.Instance.Disconnect();
        }
        if (GUILayout.Button(Sys_Guide.Instance.isUseGuide ? "关闭引导" : "打开引导"))
        {
            Sys_Role.Instance.RoleClientStateReq((uint)Sys_Role.EClientState.Guide, Sys_Guide.Instance.isUseGuide);
            //Sys_Guide.Instance.isUseGuide = !Sys_Guide.Instance.isUseGuide;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("关闭移动"))
        {
            //var component = World.GetComponent<MovementComponent>(GameCenter.mainHero);
            if (GameCenter.mainHero != null)
            {
                var component = GameCenter.mainHero.movementComponent;
                component.enableflag = false;
            }
        }

        if (GUILayout.Button("打开移动"))
        {
            //var component = World.GetComponent<MovementComponent>(GameCenter.mainHero);
            if (GameCenter.mainHero != null)
            {
                var component = GameCenter.mainHero.movementComponent;
                component.enableflag = true;
            }

            GameCenter.mCameraController.SetFollowActor(GameCenter.mainHero);
            GameCenter.mCameraController.virtualCamera.ForceRecalculation();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("关闭对话人物"))
        {
            Sys_Dialogue.Instance.ShowUIActorFlag = false;
        }

        if (GUILayout.Button("打开对话人物"))
        {
            Sys_Dialogue.Instance.ShowUIActorFlag = true;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("动画解锁功能"))
        {
            Sys_FunctionOpen.Instance.OnUnlockAllFunctionOpen(true);
        }

        if (GUILayout.Button("直接解锁功能"))
        {
            Sys_FunctionOpen.Instance.OnUnlockAllFunctionOpen(false);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("解锁功能ID"))
        {
            uint id;
            uint.TryParse(functionId, out id);
            Sys_FunctionOpen.Instance.OnUnlockOneFunctionOpen(id);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("输入需要解锁的功能ID:");
        functionId = GUILayout.TextField(functionId);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Test"))
        {
            UIManager.OpenUI(EUIID.UI_CarrerTrans);
            //UIManager.OpenUI(EUIID.UI_Activity_Mall, false, 700001);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("测试用过场展示地图11"))
        {
            Tuple<uint, object> tuple = new Tuple<uint, object>(0, 11);
            UIManager.OpenUI(EUIID.UI_PreviewMap, false, tuple);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("测试用过场展示地图12"))
        {
            Tuple<uint, object> tuple = new Tuple<uint, object>(0, 12);
            UIManager.OpenUI(EUIID.UI_PreviewMap, false, tuple);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("测试用过场展示地图13"))
        {
            Tuple<uint, object> tuple = new Tuple<uint, object>(0, 13);
            UIManager.OpenUI(EUIID.UI_PreviewMap, false, tuple);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("测试用场景光照"))
        {
            Light[] lights = Light.GetLights(LightType.Directional, 0);
            if (lights != null)
            {
                Light dirLight = lights[0];
                if (dirLight != null)
                {
                    if (dirLight.intensity > 1.5f)
                    {
                        dirLight.intensity = 1.0f;
                        dirLight.shadowStrength = 1.0f;
                    }
                    else
                    {
                        dirLight.intensity = 1.75f;
                        dirLight.shadowStrength = 0.933f;
                    }
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("返回登录"))
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                Sys_Hint.Instance.PushForbidOprationInFight();
                return;
            }
            Sys_Role.Instance.ExitGameReq();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        _autoChat = GUILayout.Toggle(_autoChat, "自动发送");

        if (GUILayout.Button("模拟聊天文本"))
        {
            Sys_Chat.ChatBaseInfo chatBaseInfo = new Sys_Chat.ChatBaseInfo();
            chatBaseInfo.sSenderName = "饭米粒";
            chatBaseInfo.nHeroID = Sys_Role.Instance.HeroId;
            chatBaseInfo.SenderHead = 100;
            //Sys_Chat.Instance.PushMessage(ChatType.System, chatBaseInfo, $"银币{_chatIndex}个"
            //    , Sys_Chat.EMessageProcess.AddSenderName | Sys_Chat.EMessageProcess.IsPerson, Sys_Chat.EExtMsgType.Normal);
            //Sys_Chat.Instance.PushMessage(ChatType.World, chatBaseInfo, $"哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈{_chatIndex}"
            //    , Sys_Chat.EMessageProcess.AddSenderName | Sys_Chat.EMessageProcess.SimplifyAddSenderName, Sys_Chat.EExtMsgType.Normal);
            //Sys_Chat.Instance.PushMessage(ChatType.LookForTeam, chatBaseInfo, $"目标：奇怪的洞窟（3/{_chatIndex}）23-59级进组<color=#00ff00>[申请入队]</color>"
            //    , Sys_Chat.EMessageProcess.AddSenderName | Sys_Chat.EMessageProcess.SimplifyAddSenderName, Sys_Chat.EExtMsgType.Normal);
            //Sys_Chat.Instance.PushMessage(ChatType.LookForTeam, chatBaseInfo, $"目标：奇怪的洞窟（3/{_chatIndex}）23-59级进组<color=#00ff00>[申请入队]</color>"
            //    , Sys_Chat.EMessageProcess.AddSenderName | Sys_Chat.EMessageProcess.SimplifyAddSenderName, Sys_Chat.EExtMsgType.Normal);
            Sys_Chat.Instance.PushMessage(ChatType.World, chatBaseInfo, $"哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈{_chatIndex}"
                , Sys_Chat.EMessageProcess.None, Sys_Chat.EExtMsgType.Normal);
            ++_chatIndex;

            Sys_Chat.Instance.PushMessage(ChatType.System, null, "<color=#00b609><color=#BE6E39>[路路莎伊雪儿]</color></color>供奉鼠王后，揭开了火焰鼠礼券的秘密，意外获得<color=#e2a438>一等奖</color>中的<color=#e2a438><color=#CC923E a=6_630103>水蓝鼠</color></color>x<color=#e2a438>1</color>。真是太幸运了！！！", Sys_Chat.EMessageProcess.None, Sys_Chat.EExtMsgType.Normal);
            chatuid = Sys_Chat.Instance.PushMessage(ChatType.System, null, "[@超级卡兹]供奉鼠王后，揭开了火焰鼠礼券的秘密，意外获得<color=#ff69da>二等奖</color>中的<color=#3291BD a=6_2>金币</color>x<color=#b6f6ff>60000</color>。真是太幸运了！"
                , Sys_Chat.EMessageProcess.AddUID, Sys_Chat.EExtMsgType.Normal);
        }

        if (GUILayout.Button("修改聊天文本"))
        {
            Sys_Chat.Instance.SetChatContent(chatuid, "[@超级卡兹]供奉鼠王后，揭开了火焰鼠礼券的秘密，意外获得<color=#888888>二等奖</color>中的<color=#888888 a=6_2>金币</color>x<color=#b6f6ff>60000</color>。真是太幸运了！");
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("设置省电模式CD(秒)"))
        {
            uint cd;
            uint.TryParse(powerSavingCD, out cd);
            if (cd <= 0)
            {
                cd = 5;
            }
            Sys_PowerSaving.Instance.SetPowerSavingCDForGM(cd);
        }
        powerSavingCD = GUILayout.TextField(powerSavingCD);
        GUILayout.EndHorizontal();

#if UNITY_EDITOR
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(Sys_OperationalActivity.Instance.AlipayGMSwitch ? "IOS支付宝活动状态(开)" : "IOS支付宝活动状态(关)"))
        {
            Sys_OperationalActivity.Instance.AlipayGMSwitch = !Sys_OperationalActivity.Instance.AlipayGMSwitch;
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("天梯界面"))
        {
            UIManager.OpenUI(EUIID.UI_LadderPvp);
        }

        if (GUILayout.Button("打开公告界面"))
        {
            UIManager.OpenUI(EUIID.UI_ExternalNotice);
        }
#endif

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
    ulong chatuid = 0;
    public void RightArea()
    {
        GUILayout.BeginArea(new Rect(Screen.width * 0.45f, 20, Screen.width * 0.4f, Screen.height * 0.9f));

        Sys_Chat.Instance.gmContent = GUILayout.TextField(Sys_Chat.Instance.gmContent, GUILayout.Height(Screen.height * 0.7f - 100));

        cmd = GUILayout.TextArea(cmd, GUILayout.Height(50));
        param = GUILayout.TextArea(param, GUILayout.Height(50));
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("清除内容", GUILayout.Height(50)))
        {
            Sys_Chat.Instance.gmContent = string.Empty;
            cmd = string.Empty;
            param = string.Empty;
            functionId = string.Empty;
        }

        if (GUILayout.Button("发送GM命令", GUILayout.Height(50)))
        {
            //SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKPayFailure, "3003|test");
            if (cmd.Equals("testmap"))
            {
                List<uint> temp = ReadHelper.ReadArray_ReadUInt(param, '|');
                uint mapId = temp[0];
                uint posX = temp[1];
                uint posY = temp[2];

                if (GameCenter.mainHero != null)
                {
                    //var component = World.GetComponent<MovementComponent>(GameCenter.mainHero);
                    var component = GameCenter.mainHero.movementComponent;
                    component.enableflag = true;
                }

                GameCenter.mPathFindControlSystem?.FindMapPoint(mapId, new Vector2(posX, -posY));

                return;
            }
            else if (cmd.Equals("reloadcsv")) {
                var ass = Assembly.Load("Logic");
                // foreach (var one in ass.GetTypes()) {
                //     Debug.LogError(one);
                // }
                var tp = ass?.GetType("Table." + param);
                var m = tp?.GetMethod("Load", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                m?.Invoke(null, new object[]{true});
                return;
            }

            CmdGmReq req = new CmdGmReq();
            //req.Roleid = Sys_Player.Instance.m_RoleId;
            req.Cmd = Google.Protobuf.ByteString.CopyFrom(cmd, System.Text.Encoding.UTF8); ;
            req.Param = Google.Protobuf.ByteString.CopyFrom(param, System.Text.Encoding.UTF8);

            NetClient.Instance.SendMessage((ushort)CmdGm.Req, req);

            if (cmd == "startfight" || cmd == "fight")
            {
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    string[] SplitGMOrders()
    {
        string[] orders = null;
        if (File.Exists(Application.streamingAssetsPath + "\\text\\GM.txt"))
        {
            orders = File.ReadAllLines(Application.streamingAssetsPath + "\\text\\GM.txt");
        }

        return orders;
    }

    private void OnDrawCombatInfo()
    {
        if (_labelSt == null)
        {
            _labelSt = new GUIStyle();
            _labelSt.alignment = TextAnchor.MiddleLeft;
            _labelSt.wordWrap = true;
            _labelSt.fontSize = 18;
        }
        if (_buttonSt == null)
        {
            _buttonSt = new GUIStyle(GUI.skin.button);
            _buttonSt.onNormal.textColor = Color.green;
            _buttonSt.onHover.textColor = Color.green;
            _buttonSt.onFocused.textColor = Color.green;
            _buttonSt.onActive.textColor = Color.green;
        }

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        float height01 = Screen.height * 0.05f;
        if (GUILayout.Button("X", _buttonSt, GUILayout.Height(height01)))
        {
            _combatDebugFlag = false;
            _selecDLogEnumFlag = false;
            _showDLogEnumNeedAddFlag = false;
        }
        GUILayout.Label(CombatManager.Instance.m_BattleId.ToString(), GUILayout.Height(height01));
        if (GUILayout.Button("过滤日志输出", _buttonSt, GUILayout.Height(height01)))
        {
            _selecDLogEnumFlag = !_selecDLogEnumFlag;
            if (_selecDLogEnumFlag)
                _showDLogEnumNeedAddFlag = false;
        }
        if (GUILayout.Button("选择日志记录", _buttonSt, GUILayout.Height(height01)))
        {
            _showDLogEnumNeedAddFlag = !_showDLogEnumNeedAddFlag;
            if (_showDLogEnumNeedAddFlag)
                _selecDLogEnumFlag = false;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        using (GUILayout.ScrollViewScope svs = new GUILayout.ScrollViewScope(_scrollPos))
        {
            if (_showDLogEnumNeedAddFlag)
            {
                GUILayout.Label("注释：*需要记录,就选取*");

                int enumIndex = 0;
                foreach (var item in Enum.GetValues(typeof(DLogManager.DLogEnum)))
                {
                    string enumName = ((DLogManager.DLogEnum)item).ToString();
                    int enumInt = (int)item;
                    if (enumInt == 0)
                        continue;

                    bool isNeedLog = (DLogManager.m_DLogEnumNeedVal & enumInt) > 0;

                    if (GUILayout.Button($"{enumName}【{(isNeedLog ? "已选择" : "")}】", (isNeedLog ? _buttonSt : GUI.skin.button)))
                    {
                        if (isNeedLog)
                            DLogManager.m_DLogEnumNeedVal &= ~enumInt;
                        else
                            DLogManager.m_DLogEnumNeedVal |= enumInt;
                    }

                    ++enumIndex;
                }
            }
            else if (_selecDLogEnumFlag)
            {
                GUILayout.Label("注释：*滤掉不看,就选取*");

                int enumIndex = 0;
                foreach (var item in Enum.GetValues(typeof(DLogManager.DLogEnum)))
                {
                    string enumName = ((DLogManager.DLogEnum)item).ToString();
                    int enumInt = (int)item;
                    if (enumInt == 0)
                        continue;

                    bool isNeedLog = (DLogManager.m_DLogEnumFilterVal & enumInt) > 0;

                    if (GUILayout.Button($"{enumName}{(isNeedLog ? "【已选择】" : "")}", (isNeedLog ? _buttonSt : GUI.skin.button)))
                    {
                        if (isNeedLog)
                            DLogManager.m_DLogEnumFilterVal &= ~enumInt;
                        else
                            DLogManager.m_DLogEnumFilterVal |= enumInt;
                    }

                    ++enumIndex;
                }
            }
            else
            {
                if (DLogManager.m_DLogList != null && DLogManager.m_DLogList.Count > 0)
                {
                    for (int i = 0; i < DLogManager.m_DLogList.Count; i++)
                    {
                        DLogManager.DLogInfo dLogInfo = DLogManager.m_DLogList[i];

                        if ((DLogManager.m_DLogEnumFilterVal & (int)dLogInfo.DLogEnum) == 0)
                            continue;

                        GUILayout.Label($"<color=white>[{string.Format("{0:T}", dLogInfo.SystemTime)}]<color=magenta>[{dLogInfo.DLogEnum.ToString()}][{dLogInfo.FrameCount.ToString()}][{dLogInfo.RealtimeSinceStartup.ToString("F3")}]</color>---{dLogInfo.LogStr}</color>",
                            _labelSt);

                        GUILayout.Space(10f);
                    }
                }
            }

            _scrollPos = svs.scrollPosition;
        }
        GUILayout.EndVertical();

        GUILayout.EndVertical();
    }
}
#endif