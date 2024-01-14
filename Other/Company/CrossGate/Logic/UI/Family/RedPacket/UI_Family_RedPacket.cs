using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary> 家族红包 </summary>
    public class UI_Family_RedPacket : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void OnOpen(object arg)
        {

        }
        protected override void OnHide()
        {
            CanelTimer();
        }
        protected override void OnClose()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btn_close;
        Animator open;
        Animator openRed;
        GameObject imgHeadOpen;
        GameObject imgContent;
        Image imgContentHead;
        Text imgContentName;
        Text imgContentBlessing;
        //GameObject Fx_RedPacket;

        GameObject imgBody;
        GameObject imgHeadClose;
        Button btnOpenNormal;
        Button btnOpenVoice;

        GameObject content;
        GameObject contentTips;
        Image contentHead;
        Text contentName;
        Text contentText;
        Text contentBlessing;
        Text contentTextNum;

        GameObject identifying;
        Animator voiceOpen;
        Animator voiceTips;
        GameObject voice;
        Text identifyingText;
        Transform identifyingTip;

        GameObject opened;
        Text textEnd;
        Image textEndIcon;
        Text messageText;
        Image messageIcon;
        Text messageText2;
        Button btnSendPackage;

        InfinityGrid infinity;
        #endregion

        #region 数据
        Dictionary<GameObject, RedPacketRecordCell> packetCellDic = new Dictionary<GameObject, RedPacketRecordCell>();
        private Timer updateTimer;
        RedPacketData curRedPacketData;
        Lib.Core.EventTrigger eventTrigger;
        #endregion
        #region 组件查找、事件
        private void OnParseComponent()
        {
            btn_close = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            open = transform.Find("Animator").GetComponent<Animator>();
            imgHeadOpen = transform.Find("Animator/Open/Image/Img_Head_Open").gameObject;
            imgContent = transform.Find("Animator/Open/Image/Img_Content").gameObject;
            imgContentHead = imgContent.transform.Find("Head").GetComponent<Image>();
            imgContentName = imgContent.transform.Find("Name").GetComponent<Text>();
            imgContentBlessing = imgContent.transform.Find("Blessing").GetComponent<Text>();
            openRed = transform.Find("Animator/Open").GetComponent<Animator>();
            imgBody = transform.Find("Animator/Open/Image/Image_Body").gameObject;
            imgHeadClose = transform.Find("Animator/Open/Image/Img_Head_Close").gameObject;
            btnOpenNormal = transform.Find("Animator/Open/Image/Btn_Open_Normal").GetComponent<Button>();
            btnOpenVoice = transform.Find("Animator/Open/Image/Btn_Open_Voice").GetComponent<Button>();
            //Fx_RedPacket = transform.Find("Animator/Open/Image/Fx_RedPacket").gameObject;

            content = transform.Find("Animator/Open/Content").gameObject;
            contentTips = content.transform.Find("Tips").gameObject;//语音红包提示字
            contentHead = content.transform.Find("Head").GetComponent<Image>();
            contentName = content.transform.Find("Name").GetComponent<Text>();
            contentText = content.transform.Find("Text").GetComponent<Text>();
            contentBlessing = content.transform.Find("Blessing").GetComponent<Text>();
            contentTextNum = content.transform.Find("Text_Num").GetComponent<Text>();

            identifying = transform.Find("Animator/Open/Identifying").gameObject;
            voice = identifying.transform.Find("Voice").gameObject;
            voiceOpen = voice.GetComponent<Animator>();
            identifyingText = identifying.transform.Find("Text2").GetComponent<Text>();
            identifyingTip = identifying.transform.Find("Tip");
            voiceTips = identifyingTip.GetComponent<Animator>();

            opened = transform.Find("Animator/Open/Opened").gameObject;
            messageText = opened.transform.Find("Message/Text").GetComponent<Text>();
            messageIcon = opened.transform.Find("Message/Image_Icon").GetComponent<Image>();
            messageText2 = opened.transform.Find("Message/Text2").GetComponent<Text>();
            btnSendPackage = opened.transform.Find("Btn_01").GetComponent<Button>();
            textEnd = opened.transform.Find("End").GetComponent<Text>();
            textEndIcon = opened.transform.Find("End/Image_Icon").GetComponent<Image>();

            infinity = opened.transform.Find("Scroll View").GetComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += onCellChange;

            btn_close.onClick.AddListener(() => CloseSelf());
            btnOpenNormal.enabled = false;
            btnOpenVoice.enabled = false;
            btnOpenNormal.onClick.AddListener(OpenRedPacketClick);

            UI_LongPressButton uI_LongPressButton = btnOpenVoice.gameObject.GetNeedComponent<UI_LongPressButton>();
            uI_LongPressButton.interval = 0f;
            uI_LongPressButton.onClickDown.AddListener(SetIdentifyingData);
            uI_LongPressButton.onStartPress.AddListener(onStartPress);
            uI_LongPressButton.onRelease.AddListener(OnPointerUp);
            eventTrigger = btnOpenVoice.GetNeedComponent<Lib.Core.EventTrigger>();
            eventTrigger.onDragStart += OnDragStart;
            eventTrigger.onDrag += _OnDrag;

            btnSendPackage.onClick.AddListener(() => {
                UIManager.OpenUI(EUIID.UI_Family_GivePacket);
                CloseSelf();
            });

            HideAll();
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnOpendRedPacketBack, OnSnatchRedEnvelopeBack, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnVoiceRecordIsPass, OnVoiceRecordPass, toRegister);
        }
        /// <summary>
        /// 语音红包录制是否通过
        /// </summary>
        private void OnVoiceRecordPass()
        {
            voiceTips.Play("Empty", -1, 0);
            voiceTips.speed = 0;
            ///识别通过
            if (Sys_Family.Instance.voiceIsCanSend)
            {
                //请求抢红包
                Sys_Family.Instance.OpenEnvelopeReq(curRedPacketData);
            }
            else//识别未通过
            {
                SetIdentifyingTipState(11952);
            }
        }
        /// <summary>
        /// 查询红包返回
        /// </summary>
        private void OnQueryRedPacketBack()
        {
            curRedPacketData = Sys_Family.Instance.curSelectRedPacketData;
            if (curRedPacketData.state == ERedPacketState.Unclaimed)
            {
                open.enabled = true;
                SetTimer();
            }
            else
            {
                open.enabled = false;
                SetState();
                openRed.Play("OpenRed", -1, 0);
            }
        }
        /// <summary>
        /// 打开红包返回
        /// </summary>
        private void OnSnatchRedEnvelopeBack()
        {
            curRedPacketData = Sys_Family.Instance.curSelectRedPacketData;
            //Fx_RedPacket.SetActive(true);
            SetState();
            openRed.Play("OpenRed", -1, 0);
        }
        #endregion

        #region 界面显示
        private void InitView()
        {
            btnSendPackage.transform.Find("Text_01").GetComponent<Text>().text=LanguageHelper.GetTextContent(2023955);
            //Fx_RedPacket.SetActive(false);
            OnQueryRedPacketBack();
        }
        private void SetContentData()
        {
            content.SetActive(true);
            contentTips.SetActive(curRedPacketData.type==ERedPacketType.Voice);
            ImageHelper.SetIcon(contentHead, CharacterHelper.getHeadID(curRedPacketData.heroId, curRedPacketData.headId));
            contentName.text = curRedPacketData.sendName;
            uint tipsId;
            if (curRedPacketData.type == ERedPacketType.Voice)
            {
                contentTips.SetActive(true);
                contentTips.transform.Find("Text").GetComponent<Text>().text = LanguageHelper.GetTextContent(2023953);
                tipsId = 11959;
            }
            else
            {
                contentTips.SetActive(false);
                tipsId = 11960;
            }
            contentText.text = LanguageHelper.GetTextContent(tipsId);
            contentBlessing.text = curRedPacketData.content;
            contentTextNum.text = LanguageHelper.GetTextContent(2023973, Sys_Family.Instance.todayMoney.ToString(), Sys_Family.Instance.GetRedPacketLimit().ToString());
        }
        private void SetImgContent()
        {
            imgContent.SetActive(true);
            ImageHelper.SetIcon(imgContentHead, CharacterHelper.getHeadID(curRedPacketData.heroId, curRedPacketData.headId));
            imgContentName.text = curRedPacketData.sendName;
            imgContentBlessing.text = curRedPacketData.content;
        }
        private void SetPacketOpendData()
        {
            opened.SetActive(true);
            string message_1;
            string message_2;
            //自己抢到了
            if (curRedPacketData.currencyValue != 0)
            {
                textEndIcon.gameObject.SetActive(true);
                textEnd.text = curRedPacketData.currencyValue.ToString();
                ImageHelper.SetIcon(textEndIcon, CSVItem.Instance.GetConfData(2).icon_id);
            }
            else
            {
                textEndIcon.gameObject.SetActive(false);
                textEnd.text = LanguageHelper.GetTextContent((uint)(curRedPacketData.isPastDue ? 11948 : 11947));
            }
            //红包未被抢完
            if (curRedPacketData.subRedPacketDataList.Count < curRedPacketData.currencyCopies)
            {
                string str = LanguageHelper.GetTextContent(11949);
                str = string.Format(str, curRedPacketData.currencyCopies, curRedPacketData.currencyAllValue," ",(curRedPacketData.currencyCopies - curRedPacketData.subRedPacketDataList.Count), curRedPacketData.currencyCopies);
                message_1 = str.Split(' ')[0];
                message_2 = str.Split(' ')[1];
            }
            else
            {
                string str = LanguageHelper.GetTextContent(11950);
                str = string.Format(str, curRedPacketData.currencyCopies, curRedPacketData.currencyAllValue, " ", curRedPacketData.costTime);
                message_1 = str.Split(' ')[0];
                message_2 = str.Split(' ')[1];
            }
            messageText.text = message_1;
            messageText2.text =  message_2;
            ImageHelper.SetIcon(messageIcon, CSVItem.Instance.GetConfData(2).icon_id);
            LayoutRebuilder.ForceRebuildLayoutImmediate(opened.transform.Find("Message").transform.GetComponent<RectTransform>());

            infinity.CellCount= curRedPacketData.subRedPacketDataList.Count;
            infinity.ForceRefreshActiveCell();
            infinity.MoveToIndex(0);
        }
        private void SetIdentifyingData()
        {
            content.SetActive(false);
            identifying.SetActive(true);
            identifyingText.text = curRedPacketData.content;
            identifyingTip.gameObject.SetActive(false);
        }
        private void onStartPress()
        {
            voice.SetActive(true);
            voiceOpen.Play("Open", -1, 0);
            voiceOpen.speed = 1;
            SetIdentifyingTipState(11963);
            isCanCancel = false;
            //红包未被抢完
            if (curRedPacketData.subRedPacketDataList.Count < curRedPacketData.currencyCopies)
            {
                //开始语音录入
                if (!Sys_Chat.Instance.IsSDKInited)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000982));
                    return;
                }
                int rlt = Sys_Chat.Instance.CheckCanSend(Packet.ChatType.FamilyRedPacket);
                if (rlt == Sys_Chat.Chat_Success)
                {
                    Sys_Chat.Instance.StartRecode(Packet.ChatType.FamilyRedPacket);
                }
                else
                {
                    Sys_Chat.Instance.PushErrorTip(rlt);
                    identifyingTip.gameObject.SetActive(false);
                }
            }
        }
        private void OnPointerUp()
        {
            voiceOpen.Play("Empty", -1, 0);
            voiceOpen.speed = 0;
            SetIdentifyingTipState(0, false);
            voice.SetActive(false);

            //红包已被抢完 无需语音验证，直接去抢
            if (curRedPacketData.subRedPacketDataList.Count >= curRedPacketData.currencyCopies)
            {
                if (isCanCancel)
                {
                    identifyingTip.gameObject.SetActive(false);
                    return;
                }
                Sys_Family.Instance.OpenEnvelopeReq(curRedPacketData);
            }
            else
            {
                if (isCanCancel)
                    identifyingTip.gameObject.SetActive(false);
                Sys_Chat.Instance.IsVaildRecord = !isCanCancel;
                Sys_Chat.Instance.StopRecode();
            }
        }
        Vector3 startPos;
        bool isCanCancel;
        private void OnDragStart(GameObject go)
        {
            isCanCancel = false;
            startPos = new Vector3(Input.mousePosition.x,Screen.height/2, Input.mousePosition.z);
        }
        private void _OnDrag(GameObject go, Vector2 delta)
        {
            Vector3 curPos = Input.mousePosition;
            if (curPos.y - startPos.y >= 70)
            {
                isCanCancel = true;
            }
            else
            {
                isCanCancel = false;
            }
        }

        /// <summary>
        /// 设置语音状态提示
        /// </summary>
        private void SetIdentifyingTipState(uint languageId=0, bool isSingle = true)
        {
            identifyingTip.gameObject.SetActive(true);
            if (isSingle)
            {
                voiceTips.speed = 0;
                identifyingTip.GetChild(0).GetComponent<Text>().text = string.Empty;
                identifyingTip.GetChild(1).GetComponent<Text>().text = string.Empty;
                identifyingTip.GetChild(2).GetComponent<Text>().text = LanguageHelper.GetTextContent(languageId);
                identifyingTip.GetChild(3).GetComponent<Text>().text = string.Empty;
                identifyingTip.GetChild(4).GetComponent<Text>().text = string.Empty;
                identifyingTip.GetChild(5).GetComponent<Text>().text = string.Empty;
            }
            else
            {
                identifyingTip.GetChild(0).GetComponent<Text>().text = LanguageHelper.GetTextContent(11953);
                identifyingTip.GetChild(1).GetComponent<Text>().text = LanguageHelper.GetTextContent(11954);
                identifyingTip.GetChild(2).GetComponent<Text>().text = LanguageHelper.GetTextContent(11955);
                identifyingTip.GetChild(3).GetComponent<Text>().text = LanguageHelper.GetTextContent(11956);
                identifyingTip.GetChild(4).GetComponent<Text>().text = LanguageHelper.GetTextContent(11956);
                identifyingTip.GetChild(5).GetComponent<Text>().text = LanguageHelper.GetTextContent(11956);
                voiceTips.Play("Open",-1,0);
                voiceTips.speed = 1;
            }
        }
        public void HideAllBtn()
        {
            btnOpenNormal.gameObject.SetActive(false);
            btnOpenVoice.gameObject.SetActive(false);
        }
        public void HideAll()
        {
            content.SetActive(false);
            identifying.SetActive(false);
            opened.SetActive(false);
            voice.SetActive(false);
        }
        private void SetState()
        {
            imgBody.SetActive(true);
            if (curRedPacketData.state == ERedPacketState.Unclaimed)            //未打开
            {
                imgHeadClose.SetActive(true);
                imgHeadOpen.SetActive(false);

                if (curRedPacketData.type == ERedPacketType.Normal)
                {
                    btnOpenNormal.gameObject.SetActive(true);
                    btnOpenVoice.gameObject.SetActive(false);
                    btnOpenNormal.enabled = true;
                }
                else
                {
                    btnOpenVoice.gameObject.SetActive(true);
                    btnOpenNormal.gameObject.SetActive(false);
                    btnOpenVoice.enabled = true;
                }
                SetContentData();
            }
            else if(curRedPacketData.state == ERedPacketState.Opened)            //已打开
            {
                HideAllBtn();
                content.SetActive(false);
                imgHeadClose.SetActive(false);
                imgHeadOpen.SetActive(true);
                SetImgContent();
                SetPacketOpendData();
            }
        }
        private void OpenRedPacketClick()
        {
            //普通红包点击播放打开动画
            if (curRedPacketData.type == ERedPacketType.Normal)
            {
                //点击请求抢红包
                Sys_Family.Instance.OpenEnvelopeReq(curRedPacketData);
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            RedPacketRecordCell entry = new RedPacketRecordCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
            packetCellDic.Add(cell.mRootTransform.gameObject, entry);
        }
        private void onCellChange(InfinityGridCell cell, int index)
        {
            RedPacketRecordCell entry = cell.mUserData as RedPacketRecordCell;
            entry.SetData(curRedPacketData.subRedPacketDataList[index], curRedPacketData);
        }
        #endregion

        #region Funtion
        private void SetTimer()
        {
            updateTimer = Timer.Register(0f, OnUpdateData, null, true);
        }
        private void OnUpdateData()
        {
            if (IsCompleteAnimator(open))
            {
                CanelTimer();
                SetState();
            }
        }
        private bool IsCompleteAnimator(Animator ani)
        {
            AnimatorStateInfo startInfo = ani.GetCurrentAnimatorStateInfo(0);
            return startInfo.normalizedTime >= 0.95f;
        }
        public void CanelTimer()
        {
            updateTimer?.Cancel();
            updateTimer = null;
        }
        #endregion
    }

    public class RedPacketRecordCell
    {
        Transform trans;
        Image head;
        Text name;
        Image currencyIcon;
        Text currencyValue;
        public void Init(Transform trans)
        {
            this.trans = trans;
            head = trans.Find("Image_BG/Head").GetComponent<Image>();
            name = trans.Find("Name").GetComponent<Text>();
            currencyIcon = trans.Find("Common_Cost01/Image_Icon").GetComponent<Image>();
            currencyValue = trans.Find("Common_Cost01/Text_Num").GetComponent<Text>();
        }
        public void SetData(SubRedPacketData data,RedPacketData curPacketData)
        {
            int state=Sys_Family.Instance.GetCurRedPacketBsetOrWorst(curPacketData, data.roleName);
            string str = state==0? data.roleName:state==1? string.Format("{0}{1}", data.roleName, LanguageHelper.GetTextContent(11945)): string.Format("{0}{1}", data.roleName, LanguageHelper.GetTextContent(11946));
            ImageHelper.SetIcon(head, CharacterHelper.getHeadID(data.heroId, data.headId));
            name.text = str;
            ImageHelper.SetIcon(currencyIcon, CSVItem.Instance.GetConfData(2).icon_id);
            currencyValue.text = data.currencyValue.ToString();
        }
    }
}