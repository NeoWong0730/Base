using DG.Tweening;
using Logic.Core;
using Packet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Table;
using System;

namespace Logic
{
    public class UI_ChatSimplify : UIBase, UI_ChatSimplify_Layout.IListener
    {
        UI_ChatSimplify_Layout Layout = new UI_ChatSimplify_Layout();

        Sys_Chat.ChatChannelData mChatChannelData;
        private int nMaxReadCount = 0;
        private int nRemovedCount = 0;
        private bool isDirty = true;
        private bool isLockDirty = true;

        private bool bViewLocked = true;
        private int nShowLayer = 0;
        private int[] showSizes = new int[] { 148, 238, 328 };//{ 88, 148, 208, 268, 328 };

        private UI_ChatVoiceButton _voiceButton = new UI_ChatVoiceButton();
        Tweener tweener;

        UI_Chat_RoleAction uI_Chat_RoleAction = new UI_Chat_RoleAction();

        protected override void OnInit()
        {
            //按照30帧跑 120/30 33ms
            SetIntervalFrame(4);
        }

        protected override void OnLoaded()
        {
            Layout.Parse(gameObject);
            Layout.RegisterEvents(this);

            Layout.sv_content_InfinityIrregularGrid.SetCapacity(Sys_Chat.Instance.mSimplifyDisplay.mMessages.Capacity);
            Layout.sv_content_InfinityIrregularGrid.MinSize = 28;
            Layout.sv_content_InfinityIrregularGrid.onCreateCell += OnCreateCell;
            Layout.sv_content_InfinityIrregularGrid.onCellChange += OnCellChange;            

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.callback.AddListener(Oncontent_Clicked);
            entry.eventID = EventTriggerType.PointerClick;

            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.callback.AddListener(Oncontent_Drag);
            entry2.eventID = EventTriggerType.BeginDrag;

            EventTrigger.Entry entry3 = new EventTrigger.Entry();
            entry3.callback.AddListener(Oncontent_EndDrag);
            entry3.eventID = EventTriggerType.EndDrag;

            Layout.sv_content_EventTrigger.triggers.Add(entry);
            Layout.sv_content_EventTrigger.triggers.Add(entry2);
            Layout.sv_content_EventTrigger.triggers.Add(entry3);

            UI_ChatSimplify_RedPoint redPoint = gameObject.AddComponent<UI_ChatSimplify_RedPoint>();
            if (redPoint != null)
            {
                redPoint.Init(this);
            }

            _voiceButton.BindGameObject(Layout.btn_voice_RectTransform, 0.5f);
            _voiceButton.SetData((int)ChatType.World);
            _voiceButton.onClick += OnVoiceButtionClick;

            Layout.rt_voiceChannel_RectTransform.gameObject.SetActive(false);
            Layout.btn_voice_RectTransform.gameObject.SetActive(true);
            Layout.txt_World.gameObject.SetActive(true);
            Layout.txt_Guild.gameObject.SetActive(false);
            Layout.txt_Team.gameObject.SetActive(false);

            uI_Chat_RoleAction.uI_ChatSimplify_Layout = Layout;
            uI_Chat_RoleAction.Parse(gameObject);
        }
        protected override void OnShow()
        {
            Vector2 size = Layout.rtBG_RectTransform.sizeDelta;
            //Layout.rtBG_RectTransform.DOSizeDelta(new Vector2(size.x, 112), 0.2f);
            Layout.rtBG_RectTransform.sizeDelta = new Vector2(size.x, showSizes[nShowLayer]);
            Layout.btn_arrow_Button.transform.localScale = new Vector3(1, nShowLayer == showSizes.Length - 1 ? -1 : 1, 1);

            Layout.rt_Setting_Image.gameObject.SetActive(false);

            if (Sys_Mail.Instance.FirstMailAdd)
            {
                OnNoticeMailAdd();
                Sys_Mail.Instance.FirstMailAdd = false;
            }
            else
            {
                RefreshMailNotice();
            }
            Layout.tog_System_Toggle.SetIsOnWithoutNotify(Sys_Chat.Instance.GetSimplifyDisplayActive(ChatType.Person));
            Layout.tog_World_Toggle.SetIsOnWithoutNotify(Sys_Chat.Instance.GetSimplifyDisplayActive(ChatType.World));
            Layout.tog_Local_Toggle.SetIsOnWithoutNotify(Sys_Chat.Instance.GetSimplifyDisplayActive(ChatType.Local));
            Layout.tog_Guild_Toggle.SetIsOnWithoutNotify(Sys_Chat.Instance.GetSimplifyDisplayActive(ChatType.Guild));
            Layout.tog_Team_Toggle.SetIsOnWithoutNotify(Sys_Chat.Instance.GetSimplifyDisplayActive(ChatType.Team));
            Layout.tog_LookForTeam_Toggle.SetIsOnWithoutNotify(Sys_Chat.Instance.GetSimplifyDisplayActive(ChatType.LookForTeam));
            Layout.tog_ChannelCareer_Toggle.SetIsOnWithoutNotify(Sys_Chat.Instance.GetSimplifyDisplayActive(ChatType.Career));
            Layout.tog_ChannelBraveTeam_Toggle.SetIsOnWithoutNotify(Sys_Chat.Instance.GetSimplifyDisplayActive(ChatType.BraveGroup));

            mChatChannelData = Sys_Chat.Instance.mSimplifyDisplay;
            _SetViewLock(false);
            isDirty = true;
            nRemovedCount = mChatChannelData.nRemovedCount;
            nMaxReadCount = 0;
            Layout.sv_content_InfinityIrregularGrid.Clear();
            Layout.sv_content_InfinityIrregularGrid.SetLockNormalizedPosition(!bViewLocked, 0);

            Sys_Family.Instance.CheckRedPoint();
            Layout.rt_RedTips.gameObject.SetActive(Sys_Family.Instance.curIsHaveRedPacket);
            Layout.btn_videoStation.gameObject.SetActive(!Sys_Role.Instance.isCrossSrv&&Sys_FunctionOpen.Instance.IsOpen(52200));

            RefreshPlanButton();
        }
        protected override void OnUpdate()
        {
            if (isDirty)
            {
                //先移除多余的                    
                int needRemoveCount = mChatChannelData.nRemovedCount - nRemovedCount;
                Layout.sv_content_InfinityIrregularGrid.RemoveTopRange(needRemoveCount);
                nRemovedCount = mChatChannelData.nRemovedCount;

                int i = 0;
                while (i < 256 && Layout.sv_content_InfinityIrregularGrid.CellCount < mChatChannelData.mMessages.Count)
                {
                    ++i;
                    Layout.sv_content_InfinityIrregularGrid.Add(CalculateSize(mChatChannelData.GetMessageByIndex(Layout.sv_content_InfinityIrregularGrid.CellCount)));
                }

                if (Layout.sv_content_InfinityIrregularGrid.CellCount >= mChatChannelData.mMessages.Count)
                {
                    isDirty = false;
                    isLockDirty = true;
                }
            }

            if (isLockDirty)
            {
                _RefreshLockCount();
            }
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                MessageBagButtonShow();
            }
            else
            {
                Layout.btn_MessageBag.gameObject.SetActive(false);
            }

        }
        protected override void OnHide()
        {
            Layout.txtTemplate_EmojiText.text = null;

            UIManager.CloseUI(EUIID.UI_ChatInput);
            Sys_Chat.Instance.IsVaildRecord = false;
            Sys_Chat.Instance.StopRecode();
        }
        protected override void OnDestroy()
        {
            _voiceButton.Clear();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Chat.Instance.eventEmitter.Handle<Sys_Chat.ChatContent>(Sys_Chat.EEvents.SimplifyMessageAdd, OnSimplifyMessageAdd, toRegister);
            Sys_Mail.Instance.eventEmitter.Handle(Sys_Mail.EEvents.OnNoticeMailAdd, OnNoticeMailAdd, toRegister);
            Sys_Mail.Instance.eventEmitter.Handle(Sys_Mail.EEvents.OnNoticeMailOver, OnNoticeMailOver, toRegister);
            Sys_Chat.Instance.eventEmitter.Handle(Sys_Chat.EEvents.VoicePlayStateChange, OnVoicePlayStateChange, toRegister);
            Sys_Chat.Instance.eventEmitter.Handle<ulong>(Sys_Chat.EEvents.MessageContentChange, OnMessageContentChange, toRegister);
#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
            Sys_SettingHotKey.Instance.eventEmitter.Handle(Sys_SettingHotKey.Events.OpenRoleActionUI, uI_Chat_RoleAction.OpenRoleAction, toRegister);
#endif
            ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnBeforeEnterFightEffect, uI_Chat_RoleAction.OnAfterEnterFightEffect, toRegister);
            Sys_MessageBag.Instance.eventEmitter.Handle<int>(Sys_MessageBag.EEvents.OnButtonShow, OnMessageBag, toRegister);

            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnRefreshRedPacketPoint, OnRefreshRedPacketPoint, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdateLevel, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateCrossSrvState, OnUpdateCrossSrvState, toRegister);
        }

        void OnUpdateCrossSrvState()
        {
            Layout.btn_videoStation.gameObject.SetActive(!Sys_Role.Instance.isCrossSrv && Sys_FunctionOpen.Instance.IsOpen(52200));
        }

        void OnUpdateLevel()
        {
            RefreshPlanButton();
            Layout.btn_videoStation.gameObject.SetActive(!Sys_Role.Instance.isCrossSrv && Sys_FunctionOpen.Instance.IsOpen(52200));
        }

        void RefreshPlanButton()
        {
            Layout.btn_Plan.gameObject.SetActive(CSVCheckseq.Instance.GetConfData(12101).IsValid());
        }

        private void OnVoicePlayStateChange()
        {            
            IReadOnlyList<InfinityGridCell> cells = Layout.sv_content_InfinityIrregularGrid.GetCells();
            for (int i = cells.Count - 1; i >= 0; --i)
            {
                UI_ChatEntry2 entry = cells[i].mUserData as UI_ChatEntry2;
                entry.RefreshState();
            }
        }

        private void OnMessageContentChange(ulong uid)
        {
            IReadOnlyList<InfinityGridCell> cells = Layout.sv_content_InfinityIrregularGrid.GetCells();
            for (int i = cells.Count - 1; i >= 0; --i)
            {                
                UI_ChatEntry2 entry = cells[i].mUserData as UI_ChatEntry2;                
                entry.RefreshContent();
            }
        }

        private void OnNoticeMailAdd()
        {
            Layout.mail_Red.gameObject.SetActive(true);
        }

        private void OnNoticeMailOver()
        {
            //Layout.btn_mail_Button.gameObject.SetActive(false);
        }

        private void OnRefreshRedPacketPoint()
        {
            Layout.rt_RedTips.gameObject.SetActive(Sys_Family.Instance.curIsHaveRedPacket);
        }
        #region messagebag

        private void OnMessageBag(int type)
        {
            Sys_MessageBag.Instance.isClick = false;
            Layout.go_MessageRedPoint.SetActive(true);
            Sys_MessageBag.Instance.MesaageBagTextShow(Layout.txt_MessageType,type);
        }

        private void MessageBagButtonShow()
        {
            Layout.btn_MessageBag.gameObject.SetActive(Sys_MessageBag.Instance.IsMessageButtonShow());
            Layout.go_MessageRedPoint.SetActive(!Sys_MessageBag.Instance.isClick);
            Sys_MessageBag.Instance.MessageBagButtonShow(Layout.txt_MessageCount, Layout.txt_MessageType);
        }

        #endregion 
        private void OnSimplifyMessageAdd(Sys_Chat.ChatContent chatContent)
        {
            isDirty = true;
        }

        private void RefreshMailNotice()
        {
            Layout.mail_Red.gameObject.SetActive(!Sys_Mail.Instance.ReadAllMail() || Sys_Mail.Instance.CanGetAttach());
        }

        private int CalculateSize(Sys_Chat.ChatContent chatContent)
        {
            Layout.txtTemplate_EmojiText.text = chatContent.sSimplifyUIContent;
            int h = (int)Layout.txtTemplate_EmojiText.preferredHeight;
            h += 6;//3 + 3
            if (chatContent.mBaseInfo != null)//策划又不要魔力精灵了 要的话把baseInfo.nHeroID > 0去掉
            {
                h += 25;//22+3
            }
            return h;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_ChatEntry2 entry = new UI_ChatEntry2();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_ChatEntry2 entry = cell.mUserData as UI_ChatEntry2;
            entry.SetData(mChatChannelData.GetMessageByIndex(index));
            _SetReadIndex(index);
        }

        private void OnCellCollect(InfinityGridCell cell)
        {
            UI_ChatEntry2 entry = cell.mUserData as UI_ChatEntry2;
            entry.SetData(null);
        }        

        public void OnFriend_ButtonClicked()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(30101, true))
                return;

            Sys_Mail.Instance.mailEnterType = 0;
            if (Sys_Society.Instance.sendOpenReqFlag)
            {
                Sys_Society.Instance.ReqGetSocialRoleInfo();
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Society);
            }
        }

        public void Onarrow_ButtonClicked()
        {
            ++nShowLayer;
            if (nShowLayer >= showSizes.Length)
            {
                nShowLayer = 0;
            }

            Vector2 size = Layout.rtBG_RectTransform.sizeDelta;
            tweener?.Complete();
            tweener = Layout.rtBG_RectTransform.DOSizeDelta(new Vector2(size.x, showSizes[nShowLayer]), 0.2f);
            //Layout.rtBG_RectTransform.sizeDelta = new Vector2(size.x, showSizes[nShowLayer]);
            Layout.btn_arrow_Button.transform.localScale = new Vector3(1, nShowLayer == showSizes.Length - 1 ? -1 : 1, 1);

            if (!bViewLocked)
            {
                Layout.sv_content_InfinityIrregularGrid.NormalizedPosition = Vector2.zero;
            }
        }

        public void Onsetting_ButtonClicked()
        {
            Layout.rt_Setting_Image.gameObject.SetActive(true);
        }

        public void Ontip_ButtonClicked()
        {
            _SetViewLock(false);
        }

        public void Oncontent_Clicked(BaseEventData eventData)
        {
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (pointerEventData.button != PointerEventData.InputButton.Left)
                return;
            if (!pointerEventData.dragging)
            {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                if (AspectRotioController.IsExpandState)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1012022));
                    return;
                }
#endif
                //CloseSelf(true);
                UIManager.CloseUI(EUIID.UI_ChatSimplify, true, false);
                UIManager.OpenUI(EUIID.UI_Chat, false, null, EUIID.UI_MainInterface);
            }
        }

        private void Oncontent_Drag(BaseEventData arg0)
        {
            _SetViewLock(true);
        }

        private void Oncontent_EndDrag(BaseEventData arg0)
        {
            if (Layout.sv_content_InfinityIrregularGrid.NormalizedPosition.y <= 0)
            {
                _SetViewLock(false);
            }
        }

        public void OnClose_ButtonClicked()
        {
            Layout.rt_Setting_Image.gameObject.SetActive(false);
        }

        public void OnSystem_ToggleValueChanged(bool arg)
        {
            Sys_Chat.Instance.SetSimplifyDisplayActive(ChatType.Person, arg);
        }

        public void OnWorld_ToggleValueChanged(bool arg)
        {
            Sys_Chat.Instance.SetSimplifyDisplayActive(ChatType.World, arg);
        }

        public void OnLocal_ToggleValueChanged(bool arg)
        {
            Sys_Chat.Instance.SetSimplifyDisplayActive(ChatType.Local, arg);
        }

        public void OnGuild_ToggleValueChanged(bool arg)
        {
            Sys_Chat.Instance.SetSimplifyDisplayActive(ChatType.Guild, arg);
        }

        public void OnTeam_ToggleValueChanged(bool arg)
        {
            Sys_Chat.Instance.SetSimplifyDisplayActive(ChatType.Team, arg);
        }

        public void OnLookForTeam_ToggleValueChanged(bool arg)
        {
            Sys_Chat.Instance.SetSimplifyDisplayActive(ChatType.LookForTeam, arg);
        }

        public void OnChannelCareer_ToggleValueChanged(bool arg)
        {
            Sys_Chat.Instance.SetSimplifyDisplayActive(ChatType.Career, arg);
        }

        public void OnChannelBraveTeam_ToggleValueChanged(bool arg)
        {
            Sys_Chat.Instance.SetSimplifyDisplayActive(ChatType.BraveGroup, arg);
        }

        public void OnMail_ButtonClicked()
        {
            //Sys_Mail.Instance.mailEnterType = 1;
            //UIManager.OpenUI(EUIID.UI_Society);
            //Layout.btn_mail_Button.gameObject.SetActive(false);
        }

        public void OnWorld_ButtonClicked()
        {
            Layout.rt_voiceChannel_RectTransform.gameObject.SetActive(false);
            Layout.btn_voice_RectTransform.gameObject.SetActive(true);
            Layout.txt_World.gameObject.SetActive(true);
            Layout.txt_Guild.gameObject.SetActive(false);
            Layout.txt_Team.gameObject.SetActive(false);

            _voiceButton.SetData((int)ChatType.World);
        }

        public void OnGuild_ButtonClicked()
        {
            Layout.rt_voiceChannel_RectTransform.gameObject.SetActive(false);
            Layout.btn_voice_RectTransform.gameObject.SetActive(true);
            Layout.txt_World.gameObject.SetActive(false);
            Layout.txt_Guild.gameObject.SetActive(true);
            Layout.txt_Team.gameObject.SetActive(false);

            _voiceButton.SetData((int)ChatType.Guild);
        }

        public void OnTeam_ButtonClicked()
        {
            Layout.rt_voiceChannel_RectTransform.gameObject.SetActive(false);
            Layout.btn_voice_RectTransform.gameObject.SetActive(true);
            Layout.txt_World.gameObject.SetActive(false);
            Layout.txt_Guild.gameObject.SetActive(false);
            Layout.txt_Team.gameObject.SetActive(true);

            _voiceButton.SetData((int)ChatType.Team);
        }

        private void OnVoiceButtionClick()
        {
            Layout.rt_voiceChannel_RectTransform.gameObject.SetActive(true);
            Layout.btn_voice_RectTransform.gameObject.SetActive(false);

            switch (_voiceButton._chatType)
            {
                case (int)ChatType.World:
                    Layout.btn_world_RectTransform.transform.SetSiblingIndex(0);
                    break;
                case (int)ChatType.Guild:
                    Layout.btn_home_RectTransform.transform.SetSiblingIndex(0);
                    break;
                case (int)ChatType.Team:
                    Layout.btn_team_RectTransform.transform.SetSiblingIndex(0);
                    break;
                default:
                    break;
            }
        }

        public void OnMessageBag_ButtonClicked()
        {
            Sys_MessageBag.Instance.isClick = true;
            Layout.go_MessageRedPoint.SetActive(false);
            UIManager.OpenUI(EUIID.UI_MessageBag);

        }

        public void OnMoney_ButtonClicked()
        {
            Sys_Chat.Instance.eChatType = ChatType.Guild;
            UIManager.OpenUI(EUIID.UI_Chat);
        }

        public void OnMonePlan_ButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Plan);
        }

        private void _SetViewLock(bool islock)
        {
            if (bViewLocked != islock)
            {
                bViewLocked = islock;

                if (bViewLocked)
                {
                    isLockDirty = true;
                }
                else
                {
                    Layout.btn_tip_Button.gameObject.SetActive(false);
                }

                Layout.sv_content_InfinityIrregularGrid.SetLockNormalizedPosition(!bViewLocked, 0);
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

        private void _RefreshLockCount()
        {
            isLockDirty = false;
            if (bViewLocked)
            {
                int count = mChatChannelData.GetAllCount() - nMaxReadCount;

                if (count > 0)
                {
                    Layout.btn_tip_Button.gameObject.SetActive(true);
                    string s = count > 99 ? "99+" : count.ToString();
                    //string.Format("您有{0}新消息", s);
                    Layout.txt_tip_Text.text = LanguageHelper.GetTextContent(2007909, s);
                }
                else
                {
                    Layout.btn_tip_Button.gameObject.SetActive(false);
                }
            }
        }

        public void OnVideoStation_ButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Video);
        }
    }
}