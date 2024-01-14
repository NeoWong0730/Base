using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Net;
using Packet;
using Google.Protobuf;
using System.IO;

namespace Logic
{
    public class Sys_SettingHotKey : SystemModuleBase<Sys_SettingHotKey>, ISystemModuleUpdate
    {
        public readonly EventEmitter<Events> eventEmitter = new EventEmitter<Events>();

        public static uint PC_SettingVersion = uint.MaxValue;
        public static bool IsLogin;

        /// <summary>键值</summary>
        public List<HotKeyData> HotKeyCodeList = new List<HotKeyData>();
        public Dictionary<string, HotKeyEventData> KeyEventDic = new Dictionary<string, HotKeyEventData>();
        private int BoosKeyID; // 老板键ID
        public int CurrentFocused;
        private KeyCode preKeyDown;
        /// <summary>是否初始化数据</summary>
        private bool IsInitData;
        /// <summary>当前人物操作类型</summary>
        private Enum_CharacterOpt curOpt = Enum_CharacterOpt.Enum_None;
        private HotKeyData curOptData;
        private Timer timer;
        private bool isCanOpt = true;
        #region CharacterOptData
        HotKeyData dataUp, dataDown, dataLeft, dataRight;
        #endregion

        public enum Events
        {
            None,
            UpdateKeyCode,
            UpdatePC_SettingVersion,
            OpenRoleActionUI,
            EnterKeySendMsg,
        }

        private void InitData(bool isReadLocal = true)
        {
            //int count = CSVHotKey.Instance.Count;
            var hotKeyDatas = CSVHotKey.Instance.GetAll();
            for (int i = 0, len = hotKeyDatas.Count; i < len; i++)
            {
                var item = hotKeyDatas[i];
                HotKeyData data = PoolManager.Fetch<HotKeyData>();
                data.id = (int)item.id;
                data.name = LanguageHelper.GetTextContent(item.language_id);
                data.funcType = (HotKeyFunType)item.type;
                data.uiId = item.ui_id;
                //data.funOpenId = item.fun_id;
                data.funOpenList = item.fun_id;
                if (item.hot_key.Count > 0)
                {
                    KeyCode key = (KeyCode)item.hot_key[item.hot_key.Count - 1];
                    data.keyCode = key;
                    data.modifiersEnum = EventModifiers.None;
                    for (int j = 0; j < item.hot_key.Count - 1; j++)
                    {
                        data.modifiersEnum |= (EventModifiers)item.hot_key[j];
                    }
                    string keyStr = AsciiToString(key);
                    data.KeyViewCharacter = GetKeyCharacterStr(data.modifiersEnum, keyStr);
                }
                if (data.funcType == HotKeyFunType.Enum_BossKey)
                    BoosKeyID = data.id;
                RefreshHotKeyList(data, true);
            }

            if (isReadLocal)
            {
                ReadHotKeyLocal(string.Format("{0}/HotKey", Sys_Role.Instance.Role.RoleId));
                IsInitData = true;
            }
            dataUp = GetHotKeyDataById((int)Enum_OpenUI.Enum_Up);
            dataDown = GetHotKeyDataById((int)Enum_OpenUI.Enum_Down);
            dataLeft = GetHotKeyDataById((int)Enum_OpenUI.Enum_Left);
            dataRight = GetHotKeyDataById((int)Enum_OpenUI.Enum_Right);
        }

        #region 系统函数
#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
        public override void Init()
        {
            ProcessEvents(true);
        }
        public override void Dispose()
        {
            curOpt = Enum_CharacterOpt.Enum_None;
            IsInitData = false;
            HotKeyCodeList.Clear();
            ProcessEvents(false);
        }
        public override void OnLogin()
        {
            IsLogin = true;
        }
        public override void OnLogout()
        {
            IsLogin = false;
            timer?.Cancel();
            curOpt = Enum_CharacterOpt.Enum_None;
        }
#endif
        #endregion

        private void ProcessEvents(bool isRegister)
        {
#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
            eventEmitter.Handle<uint>(Events.UpdatePC_SettingVersion, UpdatePC_SettingVersion, isRegister);
#endif
            if (isRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdRole.PcsettingReq, (ushort)CmdRole.PcsettingRes, OnPCSettingNty, CmdRolePCSettingRes.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdRole.PcsettingRes, OnPCSettingNty);
            }
        }

        /// <summary>
        /// 玩家PC端设置返回
        /// </summary>
        private void OnPCSettingNty(NetMsg msg)
        {

            CmdRolePCSettingRes res = NetMsgUtil.Deserialize<CmdRolePCSettingRes>(CmdRolePCSettingRes.Parser, msg);
            PC_SettingVersion = res.PcSettingNum;
            for (int i = 0; i < res.Setting.Count; i++)
            {
                string dataJson = res.Setting[i].ToStringUtf8();
                HotKeyData data = LitJson.JsonMapper.ToObject<HotKeyData>(dataJson);
                RefreshHotKeyList(data);
            }
        }

        /// <summary>
        /// 请求热键设置数据，标识不一致
        /// </summary>
        private void ReqPCSetting()
        {
            CmdRolePCSettingReq req = new CmdRolePCSettingReq();
            NetClient.Instance.SendMessage((ushort)CmdRole.PcsettingReq, req);
        }

        /// <summary>
        /// 请求设置改变
        /// </summary>
        private void ReqPCSettingChange(bool isDefault=false)
        {
            CmdRolePCSettingChangeReq req = new CmdRolePCSettingChangeReq();
            req.PcSettingNum = PC_SettingVersion;
            req.Setting.Clear();
            if (!isDefault)
            {
                for (int i = 0; i < HotKeyCodeList.Count; i++)
                {
                    string json = LitJson.JsonMapper.ToJson(HotKeyCodeList[i]);
                    ByteString bytes = ByteString.CopyFromUtf8(json);
                    req.Setting.Add(bytes);
                }
            }
            NetClient.Instance.SendMessage((ushort)CmdRole.PcsettingChangeReq, req);
        }

        private void UpdatePC_SettingVersion(uint obj)
        {
            HotKeyCodeList.Clear();
            InitData();
            if (PC_SettingVersion != obj)
            {
                Write(Sys_Role.Instance.Role.RoleId.ToString(), "HotKey");
                ReqPCSetting();
            }
        }

        public void DefaultKeySetting()
        {
            HotKeyCodeList.Clear();
            KeyEventDic.Clear();
            InitData(false);
            ReqPCSettingChange(true);
        }

        /// <summary>
        /// 捕获键盘
        /// </summary>
        public void OnGUI_HotKey()
        {
            if (!IsInitData||!IsLogin)
                return;
            Event e = Event.current;
            if (e != null && !e.isKey)
                return;
            KeyCode key = e.keyCode;
            if (key == KeyCode.None)
                return;
            string keyStr = null;
            if(e.type == EventType.KeyDown)
            {
                if (preKeyDown == key)
                    return;
                keyStr = AsciiToString(key);
                preKeyDown = key;
            }
            if (e.type == EventType.KeyUp)
            {
                if (preKeyDown == key)
                {
                    curOptData = null;
                    //curOpt = Enum_CharacterOpt.Enum_None;
                    preKeyDown = KeyCode.None;
                }
            }
            if(!string.IsNullOrEmpty(keyStr))
            {
                if (keyStr == KeyCode.KeypadEnter.ToString() || keyStr == KeyCode.Return.ToString())
                {
                    eventEmitter.Trigger(Events.EnterKeySendMsg);
                    return;
                }
                EventModifiers modifiersEnum = EventModifiers.None;
                if (e.modifiers.HasFlag(EventModifiers.Alt))
                    modifiersEnum |= EventModifiers.Alt;
                if (e.modifiers.HasFlag(EventModifiers.Shift))
                    modifiersEnum |= EventModifiers.Shift;
                if (e.modifiers.HasFlag(EventModifiers.Control))
                    modifiersEnum |= EventModifiers.Control;
                if (CurrentFocused > 0)
                    RefreshData(modifiersEnum, keyStr, e.keyCode);
                else
                    ActionFunction(modifiersEnum, keyStr);
            }
        }

        /// <summary>
        /// 更新功能的热键数据
        /// </summary>
        private void RefreshData(EventModifiers modifiersEnum,string keyStr,KeyCode keycode)
        {
            string keyCharacter = GetKeyCharacterStr(modifiersEnum, keyStr);
            if (string.IsNullOrEmpty(keyCharacter))
                return;
            if (IsChangeBossKey())
                return;
            if(IsCanSetKeyCode(keyCharacter))
            {
                //HotKeyData data = GetHotKeyDataById(CurrentFocused);
                HotKeyData data = null;
                if (HotKeyCodeList.Count > CurrentFocused - 1)
                    data = HotKeyCodeList[CurrentFocused - 1];
                if (data != null && !data.KeyViewCharacter.Equals(keyCharacter))
                {
                    data.keyCode = keycode;
                    data.modifiersEnum = modifiersEnum;
                    data.KeyViewCharacter = keyCharacter;
                    //更新界面
                    eventEmitter.Trigger<int>(Events.UpdateKeyCode, CurrentFocused);
                    ReqPCSettingChange();
                }
            }
        }

        /// <summary>
        /// 执行热键功能
        /// </summary>
        private void ActionFunction(EventModifiers modifiersEnum, string keyStr)
        {
            string keyCharacter = GetKeyCharacterStr(modifiersEnum, keyStr);
            if (string.IsNullOrEmpty(keyCharacter))
                return;
            if (KeyEventDic.ContainsKey(keyCharacter))
            {
                if (IsFocusedInput())
                    return;
                HotKeyEventData eventdata = KeyEventDic[keyCharacter];
                switch (eventdata.funcType)
                {
                    case HotKeyFunType.Enum_CharacterOpt:
                        CharacterOpt(eventdata);
                        break;
                    case HotKeyFunType.Enum_OpenUI:
                        FunOpenUI(eventdata);
                        break;
                    case HotKeyFunType.Enum_BossKey:
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 获取组合后的键值Str(Control特殊处理为Ctrl,Escape 换成Esc)
        /// </summary>
        public string GetKeyCharacterStr(EventModifiers modifiersEnum, string keyStr)
        {
            if (string.IsNullOrEmpty(keyStr))
                return null;
            string modifiers = null;
            string keyCharacter = null;
            if (keyStr.Equals("Escape"))
                keyStr = "Esc";
            string ctrl = "";
            if (modifiersEnum.HasFlag(EventModifiers.Control))
            {
                modifiersEnum &= ~EventModifiers.Control;
                ctrl = "Ctrl";
            }
            if (modifiersEnum!=EventModifiers.None)
            {
                modifiers = modifiersEnum.ToString();
                if (modifiers.Contains(","))
                    modifiers = modifiers.Replace(',', '+');
                if (string.IsNullOrEmpty(ctrl))
                    keyCharacter = string.Format("{0}+{1}", modifiers, keyStr);
                else
                    keyCharacter = string.Format("Ctrl+{0}+{1}", modifiers, keyStr);
            }
            else if(string.IsNullOrEmpty(ctrl))
                keyCharacter = string.Format("{0}", keyStr);
            else
                keyCharacter = string.Format("Ctrl+{0}", keyStr);
            return keyCharacter;
        }

        /// <summary>
        /// 是否可以设置该值
        /// </summary>
        private bool IsCanSetKeyCode(string keyViewCharacter)
        {
            for (int i = 0; i < HotKeyCodeList.Count; i++)
            {
                if (HotKeyCodeList[i].KeyViewCharacter == keyViewCharacter)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1010901));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 老板键F10不可更改
        /// </summary>
        /// <returns></returns>
        private bool IsChangeBossKey()
        {
            return BoosKeyID == CurrentFocused;
        }

        /// <summary>
        /// 获取热键数据HotKeyData
        /// </summary>
        /// <returns></returns>
        private HotKeyData GetHotKeyDataById(int id)
        {
            for (int i = 0; i < HotKeyCodeList.Count; i++)
            {
                if (HotKeyCodeList[i].id == id)
                    return HotKeyCodeList[i];
            }
            return null;
        }

        /// <summary>
        /// KeyCode  0-255 Ascii值转换
        /// </summary>
        public string AsciiToString(KeyCode key)
        {
            // Ascii 0-32，127为控制字符，无法显示
            string keyStr = null;
            if (key == KeyCode.None)
                return null;
            if (key == KeyCode.Delete)
                keyStr = key.ToString();
            else if ((int)key >= 33 && (int)key <= 255)
            {
                if ((int)key >= 97 && (int)key <= 122)
                    key -= 32;
                System.Text.ASCIIEncoding aSCII = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)key };
                keyStr = aSCII.GetString(byteArray);
            }
            else if ((int)key < 303 || (int)key > 308)
                keyStr = key.ToString();
            return keyStr;
        }

        /// <summary>
        /// HotKeyList 数据更新
        /// </summary>
        private void RefreshHotKeyList(HotKeyData data, bool isAdd = false)
        {
            if (data == null)
                return;
            HotKeyData dataLocal = GetHotKeyDataById(data.id);
            if (dataLocal != null)
                dataLocal.ResetData(data);
            else if (isAdd)
                HotKeyCodeList.Add(data);
        }

        #region SaveLocal
        /// <summary>
        /// 写入
        /// </summary>
        public void Write(string relativeDir, string relativePath)
        {
            string dir = Lib.AssetLoader.AssetPath.GetPersistentFullPath(relativeDir);
            string path = dir + "/" + relativePath;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            FileStream fs = File.Open(path,FileMode.OpenOrCreate, FileAccess.Write);

            fs.SetLength(0);
            fs.Position = 0;

            BinaryWriter br = new BinaryWriter(fs);
            // 本地PC端设置编号
            br.Write(PC_SettingVersion.ToString());
            for (int i = 0; i < HotKeyCodeList.Count; i++)
            {
                string json = LitJson.JsonMapper.ToJson(HotKeyCodeList[i]);
                br.Write(json);
            }
            br.Dispose();
            br.Close();
            fs.Dispose();
            fs.Close();
        }

        public bool ReadHotKeyLocal(string hotKeyPath)
        {
            string path = Lib.AssetLoader.AssetPath.GetPersistentFullPath(hotKeyPath);
            if (File.Exists(path))
            {
                FileStream fs = File.OpenRead(path);
                BinaryReader br = new BinaryReader(fs);

                string pc_localSettingVersion = br.ReadString();
                uint.TryParse(pc_localSettingVersion, out PC_SettingVersion);
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    if (br.BaseStream.Length - br.BaseStream.Position < 50) //写入的数据结构改变，暂时对原有的本地数据报错处理 
                        break;
                    string str = br.ReadString();
                    HotKeyData data = LitJson.JsonMapper.ToObject<HotKeyData>(str);
                    RefreshHotKeyList(data);
                }
                br.Dispose(); br.Close();
                fs.Dispose(); fs.Close();
                return true;
            }
            return false;
        }
        #endregion

        #region KeyActionFun
        private void FunOpenUI(HotKeyEventData eventdata)
        {
            if (UIManager.IsVisibleAndOpen((EUIID)eventdata.uiId))
            {
                UIManager.CloseUI((EUIID)eventdata.uiId);
            }
            else
            {
                bool isOpen = true;
                if (eventdata.funOpenList != null)
                {
                    for (int i = 0; i < eventdata.funOpenList.Count; i++)
                    {
                        if (eventdata.funOpenList[i] != 0)
                            isOpen = Sys_FunctionOpen.Instance.IsOpen(eventdata.funOpenList[i], true);
                        if (!isOpen)
                            break;
                    }
                }                 
                if (isOpen)
                {
                    switch ((Enum_OpenUI)eventdata.id)
                    {
                        case Enum_OpenUI.Enum_PetBook:
                            UIManager.OpenUI(EUIID.UI_Pet_Message, false, new PetPrama { page = EPetMessageViewState.Book });
                            break;
                        case Enum_OpenUI.Enum_Setting:
                            {
                                UIManager.OpenUI((EUIID)eventdata.uiId);
                            }
                            UIManager.OpenUI((EUIID)eventdata.uiId);
                            break;
                        case Enum_OpenUI.Enum_AutoFight:
                            if (CombatManager.Instance.m_IsFight)
                            {
                                Sys_Fight.Instance.AutoFightReq(!Sys_Fight.Instance.AutoFightData.AutoState);
                            }
                            break;
                        case Enum_OpenUI.Enum_HotKeySetting:
                            {
#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
                                UIManager.OpenUI(EUIID.UI_Setting, false, System.Tuple.Create<ESettingPage, ESetting>(ESettingPage.Settings, ESetting.HotKey));
#endif
                            }
                            break;
                        case Enum_OpenUI.Enum_ExpandChatUI:
                            {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                                Sys_PCExpandChatUI.Instance.ChangeScreenResolution(!AspectRotioController.IsExpandState);
#endif
                            }
                            break;
                        case Enum_OpenUI.Enum_EnemySwitch:
                            Sys_Hangup.Instance.SetEnemySwitch();
                            break;
                        case Enum_OpenUI.Enum_Family:
                            {
                                if (UIManager.IsVisibleAndOpen(EUIID.UI_ApplyFamily))
                                    UIManager.CloseUI(EUIID.UI_ApplyFamily);
                                else
                                    Sys_Family.Instance.OpenUI_Family();
                            }
                            break;
                        case Enum_OpenUI.Enum_RoleAction:
                            eventEmitter.Trigger(Events.OpenRoleActionUI);
                            break;
                        case Enum_OpenUI.Enum_Mall:
                            UIManager.OpenUI(EUIID.UI_Mall, false, new MallPrama() { mallId = 101 });
                            break;
                        default:
                            UIManager.OpenUI((EUIID)eventdata.uiId);
                            break;
                    }
                }
            }
            //战斗中是否可以使用快捷键？？？
        }
        private void FunOpenSettingUI()
        {

        }
        private void CharacterOpt(HotKeyEventData eventData)
        {
            curOptData = GetHotKeyDataById(eventData.id);
            if (curOptData == null)
                return;
            if (curOptData.id <= (int)Enum_OpenUI.Enum_Right)
                return;
            if(isCanOpt)
            {
                isCanOpt = false;
                CSVActionState.Data data;
                if (CSVActionState.Instance.TryGetValue(curOptData.uiId, out data))
                {
                    Sys_RoleAction.Instance.PlayRoleAction(data);
                }
                timer?.Cancel();
                int cdTime = 0;
                if(int.TryParse(CSVParam.Instance.GetConfData(1263).str_value, out cdTime))
                {
                    timer = Timer.Register(1f, () =>
                    {
                        isCanOpt = true;
                    });
                }
            }
        }

        private void RefreshCharacterOpt()
        {
            curOpt = Enum_CharacterOpt.Enum_None;
            if (IsKeyDown(dataUp))
                curOpt |= Enum_CharacterOpt.Enum_Up;
            if (IsKeyDown(dataDown))
                curOpt |= Enum_CharacterOpt.Enum_Down;
            if (IsKeyDown(dataLeft))
                curOpt |= Enum_CharacterOpt.Enum_Left;
            if (IsKeyDown(dataRight))
                curOpt |= Enum_CharacterOpt.Enum_Right;
        }

        private bool IsKeyDown(HotKeyData data)
        {
            if (data == null)
                return false;
            if (!Input.GetKey(data.keyCode))
                return false;
            if (data.modifiersEnum != EventModifiers.None)
            {
                if (data.modifiersEnum.HasFlag(EventModifiers.Control))
                {
                    if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
                        return false;
                }
                if (data.modifiersEnum.HasFlag(EventModifiers.Shift))
                {
                    if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                        return false;
                }
                if (data.modifiersEnum.HasFlag(EventModifiers.Alt))
                {
                    if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
                        return false;
                }
            }
            if (IsFocusedInput())
                return false;
            return true;
        }

        public void OnUpdate()
        {
#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
            if (!IsInitData)
                return;
            float lh = 0;
            float lv = 0;
            RefreshCharacterOpt();
            if (curOpt.HasFlag(Enum_CharacterOpt.Enum_Up))
                lv += 1;
            if (curOpt.HasFlag(Enum_CharacterOpt.Enum_Down))
                lv -= 1;
            if (curOpt.HasFlag(Enum_CharacterOpt.Enum_Left))
                lh -= 1;
            if (curOpt.HasFlag(Enum_CharacterOpt.Enum_Right))
                lh += 1;
            Sys_Input.Instance.KeyboardInputMoveAction(lh, lv);
#endif

        }

        /// <summary>
        /// 当前是否为输入状态
        /// </summary>
        private bool IsFocusedInput()
        {
            UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
            if (eventSystem.currentSelectedGameObject == null)
                return false;
            if (eventSystem.currentSelectedGameObject.GetComponent<UnityEngine.UI.InputField>() != null)
                return true;
            return false;
        }
        #endregion
    }

    public class HotKeyData
    {
        public int id;
        public string name;
        public HotKeyFunType funcType;
        public uint uiId;
        public uint funOpenId;
        public List<uint> funOpenList;
        // 主键
        public EventModifiers modifiersEnum;
        // 键值
        public KeyCode keyCode;
        // 界面显示的键值
        private string m_keyViewCharacter;
        public string KeyViewCharacter {
            get
            {
                return m_keyViewCharacter;
            }
            set
            {
                RefreshKeyEvent(value);
                m_keyViewCharacter = value;
            }
        }

        public void ResetData(HotKeyData data)
        {
            this.name = data.name;
            modifiersEnum = data.modifiersEnum;
            keyCode = data.keyCode;
            KeyViewCharacter = data.KeyViewCharacter;
        }

        private void RefreshKeyEvent(string value)
        {
            if (string.IsNullOrEmpty(value) | id == 0)
                return;

            Dictionary<string, HotKeyEventData> dic = Sys_SettingHotKey.Instance.KeyEventDic;
            if (!string.IsNullOrEmpty(m_keyViewCharacter) && dic.ContainsKey(m_keyViewCharacter))
            {
                HotKeyEventData data = dic[m_keyViewCharacter];
                HotKeyEventData eventData = PoolManager.Fetch<HotKeyEventData>();
                //HotKeyEventData eventData = new HotKeyEventData();
                eventData.id = data.id;
                eventData.ResetData();

                dic.Remove(m_keyViewCharacter);
                PoolManager.Recycle(data);
                dic[value] = eventData;
            }
            else
            {
                HotKeyEventData eventData = PoolManager.Fetch<HotKeyEventData>();
                eventData.id = id;
                eventData.ResetData();
                dic[value] = eventData;
            }
        }
    }

    public class HotKeyEventData
    {
        public int id;
        public HotKeyFunType funcType;
        public uint uiId;
        public uint funOpenId;
        public List<uint> funOpenList;

        public void ResetData()
        {
            CSVHotKey.Data dataCsv = CSVHotKey.Instance.GetConfData((uint)id);
            if (dataCsv == null)
                return;
            this.funcType = (HotKeyFunType)dataCsv.type;
            uiId = dataCsv.ui_id;
            //funOpenId = dataCsv.fun_id;
            funOpenList = dataCsv.fun_id;
        }
    }

    public enum HotKeyFunType
    {
        Enum_CharacterOpt = 0, // 移动
        Enum_OpenUI = 1, // UI界面
        Enum_BossKey = 2, // 老板键
    }

    /// <summary>
    /// 特殊类型UI(对应表里的ID)
    /// </summary>
    public enum Enum_OpenUI
    {
        Enum_None,
        Enum_Up = 1,
        Enum_Down = 2,
        Enum_Left = 3,
        Enum_Right = 4,
        Enum_PetBook = 10,
        Enum_Setting = 11,
        Enum_RoleAction = 12,
        Enum_AutoFight = 15,
        Enum_HotKeySetting = 16,
        Enum_Family = 21,
        Enum_Mall = 31,
        Enum_EnemySwitch = 33,
        Enum_ExpandChatUI = 34,
    }

    public enum Enum_CharacterOpt
    {
        Enum_None,
        Enum_Up = 1,
        Enum_Down = 2,
        Enum_Left = 4,
        Enum_Right = 8,
    }
}
