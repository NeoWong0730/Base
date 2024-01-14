using DG.Tweening;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_Chat : UIBase, UI_Chat_Layout.IListener, IChatInputEvent
    {
        public static int gTop = 26;                
        public static int gContentMinHeight = 28;

        public static int gSpace = 10;
        public static int gContentTop = 20;
        public static int gContentBottom = 20;
        public static int gContentLeft = 20;
        public static int gContentRight = 20;
        public static int gContentMax = 287;//234;
        public static int gVoiceMin = 100;


        UI_Chat_Layout Layout = new UI_Chat_Layout();

        ChatType eChatType = ChatType.World;
        Sys_Chat.ChatChannelData mChatChannelData;
        int nRemovedCount = 0;
        int nMaxReadCount = 0;

        bool isDirtyChannel = true;
        bool isDirty = true;
        bool isLockDirty = true;

        bool bViewLocked = true;        

        private List<UI_ChatRoomPlayer> _chatRoomPlayers = new List<UI_ChatRoomPlayer>(5);
        private UI_ChatVoiceButton _voiceButton = new UI_ChatVoiceButton();

        private int nCaretPosition = -1;

        protected override void OnInit()
        {
            //按照30帧跑 120/30 33ms
            SetIntervalFrame(4);
        }

        protected override void OnLoaded()
        {            
            Layout.Parse(gameObject);
            Layout.RegisterEvents(this);

            Layout.tg_Channel_CP_ToggleRegistry.onToggleChange += OnToggleChange;

            Layout.sv_Content_InfinityIrregularGrid.SetCapacity(256);
            Layout.sv_Content_InfinityIrregularGrid.MinSize = 30;
            Layout.sv_Content_InfinityIrregularGrid.onCreateCell += OnCreateCell;
            Layout.sv_Content_InfinityIrregularGrid.onCellChange += OnCellChange;            
            ScrollRectEventTrigger contentSRET = Layout.sv_Content_InfinityIrregularGrid.ScrollView as ScrollRectEventTrigger;
            contentSRET.onBeginDrag.AddListener(Oncontent_Drag);
            contentSRET.onEndDrag.AddListener(Oncontent_EndDrag);

            Layout.ipt_Word_InputField.onEndEdit.AddListener(OnEndEdit);

            OnInputStateChange(false);

            _voiceButton.BindGameObject(Layout.trigger_RectTransform);
            _voiceButton.SetData(-1);
        }

        private void Oncontent_Drag(BaseEventData arg0)
        {
            _SetViewLock(true);
            //_RefreshChannel();
        }

        private void Oncontent_EndDrag(BaseEventData arg0)
        {
            if (Layout.sv_Content_InfinityIrregularGrid.NormalizedPosition.y <= 0)
            {
                _SetViewLock(false);
            }
            //_RefreshChannel();
        }


        //protected override void OnOpen(object arg)
        //{
        //    if (arg == null)
        //        return;
        //
        //    Sys_Chat.Instance.eChatType = (ChatType)((uint)arg);
        //}

        protected override void OnShow()
        {
            eChatType = Sys_Chat.Instance.eChatType;            
            Layout.tg_Channel_CP_ToggleRegistry.SwitchTo((int)eChatType);
            Sys_Chat.Instance.eCurrentInput = Sys_Chat.EInputType.Chat;
            
            Layout.sv_Content_InfinityIrregularGrid.Clear();

            isDirtyChannel = true;
            isDirty = true;

            ImageHelper.SetTexture(Layout.rimg_BG, Sys_Head.Instance.clientHead.chatBackIconId);

            //设置模板的长度
            Layout.txtCommon_Text.rectTransform.sizeDelta = new Vector2(gContentMax - 40, 280);

            _RefreshSystemShow();

            Layout.ipt_Word_InputField.SetTextWithoutNotify(Sys_Chat.Instance.mInputCache.GetContent());
            //Layout.ipt_Word_InputField.text = Sys_Chat.Instance.mInputCache.GetContent();   
            ChangeExpandLayer();
            _RefreshRedPacketRedPoint();
        }

        protected override void OnHide()
        {
            UIManager.CloseUI(EUIID.UI_ChatInput);
            Sys_Chat.Instance.IsVaildRecord = false;
            Sys_Chat.Instance.StopRecode();
        }

        protected override void OnClose()
        {
            EUIID parentId = EUIID.UI_MainInterface;
            if (UIManager.IsOpenState(EUIID.UI_FamilyBoss))
                parentId = EUIID.UI_FamilyBoss;
#if GM_PROPAGATE_VERSION && UNITY_STANDALONE_WIN
            if (Sys_Chat.Instance.isActionHideUI)
                return;
#endif
            UIManager.OpenUI(EUIID.UI_ChatSimplify, false, null, parentId);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Chat.Instance.eventEmitter.Handle<ChatType>(Sys_Chat.EEvents.MessageAdd, OnMessageAdd, toRegister);
            Sys_Chat.Instance.eventEmitter.Handle(Sys_Chat.EEvents.InputChange, OnInputChange, toRegister);

            Sys_Chat.Instance.eventEmitter.Handle(Sys_Chat.EEvents.EndpointsUpdate, _RefreshRoomPlayers, toRegister);
            Sys_Chat.Instance.eventEmitter.Handle(Sys_Chat.EEvents.EnterRoom, _RefreshRoom, toRegister);
            Sys_Chat.Instance.eventEmitter.Handle(Sys_Chat.EEvents.ExitRoom, _RefreshRoom, toRegister);

            Sys_Chat.Instance.eventEmitter.Handle(Sys_Chat.EEvents.VoicePlayStateChange, OnVoicePlayStateChange, toRegister);
            Sys_Chat.Instance.eventEmitter.Handle<ulong>(Sys_Chat.EEvents.MessageContentChange, OnMessageContentChange, toRegister);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.TeamClear, OnTeamStateChange, toRegister);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.HaveTeam, OnTeamStateChange, toRegister);

            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnUsingUpdate, OnUsingUpdate, toRegister);
            Sys_SettingHotKey.Instance.eventEmitter.Handle(Sys_SettingHotKey.Events.EnterKeySendMsg, OnEnterKey_SendMessage, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnRefreshRedPacketPoint, OnRefreshRedPacketPoint, toRegister);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, toRegister);
        }

        private void OnRefreshRedPacketPoint()
        {
            _RefreshRedPacketRedPoint();
        }

        private void OnCompletedFunctionOpen(Sys_FunctionOpen.FunctionOpenData data)
        {
            _RefreshRedPacket();
            _RefreshRedPacketRedPoint();
        }

        private void OnUsingUpdate()
        {            
            ImageHelper.SetTexture(Layout.rimg_BG, Sys_Head.Instance.clientHead.chatBackIconId);
        }

        private void OnTeamStateChange()
        {
            if (eChatType == ChatType.Team)
            {
                _RefreshRoom();
            }
        }

        private void OnInputChange()
        {            
            Layout.ipt_Word_InputField.SetTextWithoutNotify(Sys_Chat.Instance.mInputCache.GetContent());                        
        }

        private void OnMessageAdd(ChatType type)
        {
            if (ChatType.System == eChatType)
            {
                if (Sys_Chat.Instance.eSystemChannelShow == ChatType.System)
                {
                    isDirty = type == ChatType.Notice || type == ChatType.Horn || type == ChatType.Person || type == ChatType.System;
                }
                else
                {
                    isDirty = type == Sys_Chat.Instance.eSystemChannelShow;
                }
            }
            else if (type == eChatType)
            {
                isDirty = true;
            }
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_ChatEntry1 entry = new UI_ChatEntry1();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_ChatEntry1 entry = cell.mUserData as UI_ChatEntry1;

            entry.SetData(mChatChannelData.GetMessageByIndex(index));

            _SetReadIndex(index);
        }

        private void OnCellCollect(InfinityGridCell cell)
        {
            UI_ChatEntry1 entry = cell.mUserData as UI_ChatEntry1;
            entry.SetData(null);
        }

        private void OnVoicePlayStateChange()
        {
            IReadOnlyList<InfinityGridCell> cells = Layout.sv_Content_InfinityIrregularGrid.GetCells();
            for (int i = cells.Count - 1; i >= 0; --i)
            {
                UI_ChatEntry1 entry = cells[i].mUserData as UI_ChatEntry1;
                entry.RefreshState();
            }
        }

        private void OnMessageContentChange(ulong uid)
        {
            IReadOnlyList<InfinityGridCell> cells = Layout.sv_Content_InfinityIrregularGrid.GetCells();
            for (int i = cells.Count - 1; i >= 0; --i)
            {
                UI_ChatEntry1 entry = cells[i].mUserData as UI_ChatEntry1;                
                entry.RefreshContent(uid);
            }
        }

        private void OnToggleChange(int current, int old)
        {
            if ((int)eChatType == current)
                return;

            eChatType = (ChatType)current;
            Sys_Chat.Instance.eChatType = eChatType;

            isDirtyChannel = true;
        }

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Chat);
        }

        public void OnEmoji_ButtonClicked()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (!AspectRotioController.IsExpandState)
            {
                OnInputStateChange(true);
            }
#else
            OnInputStateChange(true);
#endif
            UIManager.OpenUI(EUIID.UI_ChatInput, false, this, EUIID.UI_Chat);
        }

        public void OnLaBa_ButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Horn);
        }

        public void OnSend_ButtonClicked()
        {
            int rlt = Sys_Chat.Instance.SendContent(eChatType, Sys_Chat.Instance.mInputCache);
            if (rlt == Sys_Chat.Chat_Success)
            {
                Sys_Chat.Instance.mInputCache.Clear();
                Layout.ipt_Word_InputField.SetTextWithoutNotify(Sys_Chat.Instance.mInputCache.GetContent());                

                _SetViewLock(false);
            }
            else
            {
                Sys_Chat.Instance.PushErrorTip(rlt);
            }

            UIManager.CloseUI(EUIID.UI_ChatInput);
        }

        public void OnWord_InputFieldValueChanged(string arg)
        {
            Sys_Chat.Instance.mInputCache.SetContent(arg);
        }

        //protected override void OnUpdate()
        //{
        //    //if (isDirtyChannel)
        //    //{
        //    //    _RefreshChannel();
        //    //    //RefreshContent(true);
        //    //
        //    //    isDirtyChannel = false;
        //    //    isDirty = false;
        //    //}
        //    //else if (isDirty)
        //    //{
        //    //    //RefreshContent(false);
        //    //    isDirty = false;
        //    //}
        //}

        protected override void OnUpdate()
        {
            if (isDirtyChannel)
            {
                _RefreshChannel();
                _SetViewLock(false);
            }

            if (isDirty)
            {
                //先移除多余的
                int needRemoveCount = mChatChannelData.nRemovedCount - nRemovedCount;
                Layout.sv_Content_InfinityIrregularGrid.RemoveTopRange(needRemoveCount);
                nRemovedCount = mChatChannelData.nRemovedCount;

                int i = 0;
                while (i < 256 && Layout.sv_Content_InfinityIrregularGrid.CellCount < mChatChannelData.mMessages.Count)
                {
                    ++i;
                    Layout.sv_Content_InfinityIrregularGrid.Add(CalculateSize_Normal(mChatChannelData.mMessages[Layout.sv_Content_InfinityIrregularGrid.CellCount]));
                }

                if (Layout.sv_Content_InfinityIrregularGrid.CellCount >= mChatChannelData.mMessages.Count)
                {
                    isDirty = false;
                    isLockDirty = true;
                }
            }

            if (isLockDirty)
            {
                _RefreshLockCount();
            }

            if (eChatType == ChatType.Team)
            {
                for (int i = 0; i < _chatRoomPlayers.Count; ++i)
                {
                    _chatRoomPlayers[i].OnLateUpdate();
                }
            }
        }

        private void _RefreshChannel()
        {
            isDirtyChannel = false;

            _RefreshRedPacket();

            Layout.rt_Text_Tips.gameObject.SetActive(eChatType == ChatType.World || eChatType == ChatType.Guild || eChatType == ChatType.Career || eChatType == ChatType.BraveGroup);
            //输入框状态
            Layout.rtInput_RectTransform.gameObject.SetActive(eChatType == ChatType.World || eChatType == ChatType.Local || eChatType == ChatType.Guild || eChatType == ChatType.Team || eChatType == ChatType.Career || eChatType == ChatType.BraveGroup);
            Layout.rtTitle_System_RectTransform.gameObject.SetActive(eChatType == ChatType.System);
            
            Layout.sv_Content_InfinityIrregularGrid.gameObject.SetActive(true);

            if (eChatType != ChatType.Team)
            {
                //offsetMin ： 对应Left、Bottom
                //offsetMax ： 对应Right、Top

                RectTransform rt = Layout.rt_Tip_Button.transform as RectTransform;                
                RectTransform svRT = Layout.sv_Content_InfinityIrregularGrid.transform as RectTransform;

                if (eChatType == ChatType.System)
                {
                    rt.offsetMin = new Vector2(rt.offsetMin.x, 4);
                    rt.offsetMax = new Vector2(rt.offsetMax.x, 4 + 32);

                    svRT.offsetMin = new Vector2(svRT.offsetMin.x, 4);
                    svRT.offsetMax = new Vector2(svRT.offsetMax.x, 4);
                }
                else
                {
                    rt.offsetMin = new Vector2(rt.offsetMin.x, 66);
                    rt.offsetMax = new Vector2(rt.offsetMax.x, 66 + 32);

                    svRT.offsetMin = new Vector2(svRT.offsetMin.x, 66);
                    svRT.offsetMax = new Vector2(svRT.offsetMax.x, 4);
                }                
            }            

            Layout.trigger_RectTransform.gameObject.SetActive(eChatType == ChatType.World || eChatType == ChatType.Guild || eChatType == ChatType.Team || eChatType == ChatType.Career || eChatType == ChatType.BraveGroup);
            _RefreshRoom();
            _RefreshRoomPlayers();

            //切换页签后重新初始化Content的数据
            mChatChannelData = Sys_Chat.Instance.GetChatChannelData(eChatType == ChatType.System ? Sys_Chat.Instance.eSystemChannelShow : eChatType);            
            nRemovedCount = mChatChannelData.nRemovedCount;
            nMaxReadCount = 0;
            Layout.sv_Content_InfinityIrregularGrid.SetLockNormalizedPosition(!bViewLocked, 0);
            //Layout.sv_Content_InfinityIrregularGrid.Clear();
            Layout.sv_Content_InfinityIrregularGrid.CellCount = 0;
            Layout.sv_Content_InfinityIrregularGrid.ForceRefreshActiveCell();

            isDirty = true;
        }

        private int CalculateSize_Normal(Sys_Chat.ChatContent content)
        {
            if (content.DisplayType() == 0)
            {
                Layout.txtSystem_EmojiText.text = content.sUIContent;
                int h = (int)Layout.txtSystem_EmojiText.preferredHeight + 6;
                if(content.nTimePoint > 0)
                {
                    h += 26;
                }
                return h;
            }
            else
            {
                //float h = _textGenerator.GetPreferredHeight(content, _textGenerationSettings);
                int h = gTop + gContentTop;
                bool hasVoice = false;

                if (!string.IsNullOrEmpty(content.sFileID))
                {
                    h += gContentMinHeight;
                    hasVoice = true;
                }

                if (!string.IsNullOrWhiteSpace(content.sUIContent))
                {
                    if (hasVoice)
                    {
                        h += gSpace;
                    }
                    
                    Layout.txtCommon_Text.text = content.sUIContent;
                    h += Mathf.Max((int)Layout.txtCommon_Text.preferredHeight, gContentMinHeight);
                }

                h += gContentBottom;

                if (content.nTimePoint > 0)
                {
                    h += 26;
                }

                return h;
            }
        }

        public void OnLaBa_ToggleValueChanged(bool arg)
        {
            if (arg)
            {
                Layout.tog_GeRen_Toggle.SetIsOnWithoutNotify(false);
            }
            Sys_Chat.Instance.SetSystemChannelShow(Layout.tog_LaBa_Toggle.isOn ? ChatType.Horn : (Layout.tog_GeRen_Toggle.isOn ? ChatType.Person : ChatType.System));
            isDirtyChannel = true;
        }

        public void OnGeRen_ToggleValueChanged(bool arg)
        {
            if (arg)
            {
                Layout.tog_LaBa_Toggle.SetIsOnWithoutNotify(false);
            }
            Sys_Chat.Instance.SetSystemChannelShow(Layout.tog_LaBa_Toggle.isOn ? ChatType.Horn : (Layout.tog_GeRen_Toggle.isOn ? ChatType.Person : ChatType.System));
            isDirtyChannel = true;
        }

        public void OnLock_ToggleValueChanged(bool arg)
        {
            _SetViewLock(arg);
        }

        public void OnInputStateChange(bool bOpen)
        {
            //float scale = 1;

            if (bOpen)
            {
                Layout.rt_Dialogue_RectTransform.DOLocalMoveY(310, 0.2f);
                //Layout.rt_Dialogue_RectTransform.offsetMin = new Vector2(Layout.rt_Dialogue_RectTransform.offsetMin.x, 310 * scale);
                //Layout.rt_Dialogue_RectTransform.offsetMax = new Vector2(Layout.rt_Dialogue_RectTransform.offsetMax.x, 310 * scale);
            }
            else
            {
                Layout.rt_Dialogue_RectTransform.DOLocalMoveY(5, 0.2f);
                //Layout.rt_Dialogue_RectTransform.offsetMin = new Vector2(Layout.rt_Dialogue_RectTransform.offsetMin.x, 5 * scale);
                //Layout.rt_Dialogue_RectTransform.offsetMax = new Vector2(Layout.rt_Dialogue_RectTransform.offsetMax.x, 0 * scale);
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (AspectRotioController.IsExpandState)
                 Layout.rt_Dialogue_RectTransform.DOLocalMoveY(0, 0.2f);
#endif
            }
        }

        private void OnEndEdit(string arg0)
        {
            nCaretPosition = Layout.ipt_Word_InputField.caretPosition;
        }

        public void OnChatInputClose()
        {
            OnInputStateChange(false);
        }

        public int GetCaretPosition()
        {
            return nCaretPosition;
        }

        public void SetCaretPosition(int caretPosition)
        {
            nCaretPosition = caretPosition;
        }

        private void _RefreshSystemShow()
        {
            Layout.tog_GeRen_Toggle.SetIsOnWithoutNotify(Sys_Chat.Instance.eSystemChannelShow == ChatType.Person);
            Layout.tog_LaBa_Toggle.SetIsOnWithoutNotify(Sys_Chat.Instance.eSystemChannelShow == ChatType.Horn);
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Layout.btn_PcExpand.gameObject.SetActive(true);
#else
            Layout.btn_PcExpand.gameObject.SetActive(false);
#endif
        }

        private void _RefreshLockCount()
        {
            isLockDirty = false;

            if (bViewLocked)
            {
                int count = mChatChannelData.GetAllCount() - nMaxReadCount;

                if (count > 0)
                {
                    Layout.rt_Tip_Button.gameObject.SetActive(true);
                    string s = count > 99 ? "99+" : count.ToString();
                    //"您有{0}新消息"
                    Layout.txt_Tip_Text.text = LanguageHelper.GetTextContent(2007909, s);
                }
                else
                {
                    Layout.rt_Tip_Button.gameObject.SetActive(false);
                }
            }
        }

        private void _SetViewLock(bool islock)
        {
            if (bViewLocked != islock)
            {
                bViewLocked = islock;

                Layout.tog_Lock_Toggle.SetIsOnWithoutNotify(bViewLocked);

                if (bViewLocked)
                {
                    Layout.img_mark_Image.rectTransform.localPosition = Layout.txt_off_Text.rectTransform.localPosition;
                    isLockDirty = true;
                }
                else
                {
                    Layout.rt_Tip_Button.gameObject.SetActive(false);
                    Layout.img_mark_Image.rectTransform.localPosition = Layout.txt_on_Text.rectTransform.localPosition;
                }
                
                Layout.sv_Content_InfinityIrregularGrid.SetLockNormalizedPosition(!bViewLocked, 0);
            }
        }

        private void _SetReadIndex(int index)
        {
            int newCount = Mathf.Max(index + nRemovedCount + 1, nMaxReadCount);
            if (newCount != nMaxReadCount)
            {
                nMaxReadCount = newCount;
                isLockDirty = bViewLocked;
            }
        }

        public void OnTip_ButtonClicked()
        {
            _SetViewLock(false);
        }

        public void OnEnterRoom_ButtonClicked()
        {
            Sys_Chat.Instance.EnterRoom();
            _RefreshRoom();
        }

        private void _RefreshRoomPlayers()
        {
            int roleCount = Sys_Chat.Instance.mInRoomRoles.Count;
            int showCount = 5;

            UI_ChatRoomPlayer roomPlayer;
            for (int i = 0; i < showCount; ++i)
            {
                if (i < _chatRoomPlayers.Count)
                {
                    roomPlayer = _chatRoomPlayers[i];
                }
                else
                {
                    roomPlayer = new UI_ChatRoomPlayer();
                    roomPlayer.BindGameObject(GameObject.Instantiate(Layout.rt_onePlayer.gameObject, Layout.rt_RoomPlayerRoot));
                    _chatRoomPlayers.Add(roomPlayer);
                }

                if (i < roleCount)
                {
                    roomPlayer.SetData(Sys_Chat.Instance.mInRoomRoles[i], true);
                }
                else
                {
                    //额外的一个
                    roomPlayer.SetData(0, true);
                }
            }
        }

        private void _RefreshMic()
        {
            if (Sys_Chat.Instance.GetMicState())
            {
                Layout.txt_OpenMic.text = LanguageHelper.GetTextContent(10897);//"关闭麦克风";
            }
            else
            {
                Layout.txt_OpenMic.text = LanguageHelper.GetTextContent(10880);// "开启麦克风";
            }
        }

        private void _RefreshRoom()
        {
            if (Sys_Chat.Instance.eChatType != ChatType.Team)
            {
                Layout.rt_View_Voice.gameObject.SetActive(false);
                return;
            }

            RectTransform rt = Layout.rt_Tip_Button.transform as RectTransform;
            RectTransform svRT = Layout.sv_Content_InfinityIrregularGrid.transform as RectTransform;

            if (Sys_Chat.Instance.isRoomEntered)
            {
                rt.offsetMin = new Vector2(rt.offsetMin.x, 240);
                rt.offsetMax = new Vector2(rt.offsetMax.x, 240 + 32);

                svRT.offsetMin = new Vector2(svRT.offsetMin.x, 240);
                svRT.offsetMax = new Vector2(svRT.offsetMax.x, 4);
            }
            else
            {
                rt.offsetMin = new Vector2(rt.offsetMin.x, 100);
                rt.offsetMax = new Vector2(rt.offsetMax.x, 100 + 32);

                svRT.offsetMin = new Vector2(svRT.offsetMin.x, 100);
                svRT.offsetMax = new Vector2(svRT.offsetMax.x, 4);
            }

            if (Sys_Team.Instance.HaveTeam)
            {
                Layout.rt_View_Voice.gameObject.SetActive(true);
            }
            else
            {
                Layout.rt_View_Voice.gameObject.SetActive(false);
                return;
            }

            Layout.rt_RoomPlayerRoot.gameObject.SetActive(Sys_Chat.Instance.isRoomEntered);
            Layout.rt_Room.gameObject.SetActive(Sys_Chat.Instance.isRoomEntered);

            Layout.btn_EnterRoom.gameObject.SetActive(!Sys_Chat.Instance.isRoomEntered);
            //Layout.btn_ExitRoom.gameObject.SetActive(Sys_Chat.Instance.isRoomEntered);


            Layout.btn_EnterRoom.interactable = !Sys_Chat.Instance.isWaitRoomEnterOrExit;
            Layout.btn_ExitRoom.interactable = !Sys_Chat.Instance.isWaitRoomEnterOrExit;

            if (!Sys_Chat.Instance.isRoomEntered)
            {
                if (Sys_Chat.Instance.isWaitRoomEnterOrExit)
                {
                    Layout.txt_EnterRoom.text = LanguageHelper.GetTextContent(10875);// "正在进入聊天室";
                }
                else
                {
                    if (Sys_Chat.Instance.mInRoomRoles.Count > 0)
                    {
                        Layout.txt_EnterRoom.text = LanguageHelper.GetTextContent(10876); //"进入聊天室";
                    }
                    else
                    {
                        Layout.txt_EnterRoom.text = LanguageHelper.GetTextContent(10877); //"开启聊天室";
                    }
                }
            }
            else
            {
                if (Sys_Chat.Instance.isWaitRoomEnterOrExit)
                {
                    Layout.txt_ExitRoom.text = LanguageHelper.GetTextContent(10878); //"正在退出聊天室";
                }
                else
                {
                    Layout.txt_ExitRoom.text = LanguageHelper.GetTextContent(10879); //"退出聊天室";
                }
            }

            _RefreshMic();
        }
        //刷新红包按钮
        private void _RefreshRedPacket()
        {
            bool needRedPacket = eChatType == ChatType.Guild && Sys_FunctionOpen.Instance.IsOpen(30407);
            Layout.btn_Money.gameObject.SetActive(needRedPacket);
        }

        private void _RefreshRedPacketRedPoint()
        {
            Sys_Family.Instance.CheckRedPoint();
            bool needRedPoint = Sys_FunctionOpen.Instance.IsOpen(30407) && Sys_Family.Instance.curIsHaveRedPacket;
            Layout.rt_RedTips.gameObject.SetActive(needRedPoint);
            Layout.rt_GuildRedTips.gameObject.SetActive(needRedPoint);
        }

        public void OnExitRoom_ButtonClicked()
        {
            Sys_Chat.Instance.ExitRoom();
            _RefreshRoom();
        }

        public void OnOpenMic_ButtonClicked()
        {
            Sys_Chat.Instance.EnableMic(!Sys_Chat.Instance.GetMicState());

            _RefreshMic();
        }

        public void OnSetting_ButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Setting, false, Tuple.Create<ESettingPage, ESetting>(ESettingPage.Settings, ESetting.Audio));
        }

        public void OnChangeHead_ButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Head);
        }

        private void OnEnterKey_SendMessage()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
                return;
            if (!string.IsNullOrEmpty(Sys_Chat.Instance.mInputCache.GetContent())
                || eventSystem.currentSelectedGameObject == Layout.ipt_Word_InputField.gameObject)
            {
                OnSend_ButtonClicked();
                Layout.ipt_Word_InputField.ActivateInputField();
            }
        }

        public void OnPcExpand_ButtonClicked()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Sys_PCExpandChatUI.Instance.ChangeScreenResolution(!AspectRotioController.IsExpandState);
#endif
        }

        public void ChangeExpandLayer()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (AspectRotioController.IsExpandState)
                transform.Setlayer(ELayerMask.ExpandUI);
#endif
        }

        public void OnMoney_ButtonClicked()
        {
            if (Sys_Family.Instance.familyData.isInFamily)
                UIManager.OpenUI(EUIID.UI_Family_PacketRecord);
            else
                UIManager.OpenUI(EUIID.UI_ApplyFamily);
        }
    }
}