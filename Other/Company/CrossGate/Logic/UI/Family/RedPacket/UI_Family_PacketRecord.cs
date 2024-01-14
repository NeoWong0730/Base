using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using Lib.Core;
using System;
using UnityEngine;
using Framework;

namespace Logic
{
    /// <summary> 家族红包记录 </summary>
    public class UI_Family_PacketRecord : UIBase
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
        protected override void OnClose()
        {
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        protected override void OnLateUpdate(float dt, float usdt)
        {
            OnLateUpdate();
        }
        #endregion
        #region 组件
        Button btn_Close;
        Text titlePanel;

        GameObject imageNormal;
        InfinityGrid infinityGridLeft;
        Text titleLeft;
        GameObject imageEmpty;
        Text imageEmptyContent;
        Button imageEmptyBtnTopUp;
        Text textMyPacketTitle;
        Text textSumTotalTitle;
        Text textSumTotal;
        Text textPacketValueTitle;
        Text textPacketValue;
        Button btnSendPacket;
        Button checkRank;
        Text textRule;

        InfinityGrid infinityGridRight;
        Text titleRight;
        Text txtCommon_Text;
        UI_CurrencyTitle ui_CurrencyTitle;

        InfinityIrregularGrid infinityIrregularGrid;
        #endregion
        #region 数据
        bool isDirty;
        #endregion
        #region 组件查找、事件
        private void OnParseComponent()
        {
            btn_Close = transform.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>();
            titlePanel = transform.Find("Animator/View_Title09/Text_Title").GetComponent<Text>();

            Transform viewLeft = transform.Find("Animator/View_Left");
            imageNormal = viewLeft.Find("Image_Normal").gameObject;
            infinityGridLeft = imageNormal.transform.Find("Scroll View").GetComponent<InfinityGrid>();
            titleLeft = viewLeft.Find("Title").GetComponent<Text>();
            imageEmpty = viewLeft.Find("Image_Empty").gameObject;
            imageEmptyContent = imageEmpty.transform.Find("Text").GetComponent<Text>();
            imageEmptyBtnTopUp = imageEmpty.transform.Find("Btn_01").GetComponent<Button>();
            textMyPacketTitle= imageEmpty.transform.Find("Image_BG/Title").GetComponent<Text>();
            textSumTotalTitle = viewLeft.Find("BG1/Text_Title").GetComponent<Text>();
            textSumTotal = viewLeft.Find("BG1/Text_Num").GetComponent<Text>();
            textPacketValueTitle = viewLeft.Find("BG2/Text_Title").GetComponent<Text>();
            textPacketValue = viewLeft.Find("BG2/Text_Num").GetComponent<Text>();
            btnSendPacket = viewLeft.Find("Btn_01").GetComponent<Button>();
            checkRank = viewLeft.Find("Btn_02").GetComponent<Button>();
            textRule = viewLeft.Find("Rule/Scroll View/Viewport/Content/Text_Rule").GetComponent<Text>();

            Transform viewRight = transform.Find("Animator/View_Right");
            titleRight = viewRight.Find("Title").GetComponent<Text>();
            infinityGridRight = viewRight.Find("Image_BG/Scroll View").GetComponent<InfinityGrid>();
            infinityIrregularGrid= viewRight.Find("Record/Scroll View").GetComponent<InfinityIrregularGrid>();
            txtCommon_Text=viewRight.Find("Record/_txtCommon").GetComponent<Text>();
            ui_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            ui_CurrencyTitle.SetData(new List<uint>() {1,2,3});

            btn_Close.onClick.AddListener(()=> CloseSelf());
            imageEmptyBtnTopUp.onClick.AddListener(()=> GoCharge());//前往充值
            btnSendPacket.onClick.AddListener(()=> UIManager.OpenUI(EUIID.UI_Family_GivePacket));//发送红包
            checkRank.onClick.AddListener(() =>
            {
                if(Sys_FunctionOpen.Instance.IsOpen(50901,true))
                    UIManager.OpenUI(EUIID.UI_Rank, false, new OpenUIRankParam() { initType = 8 });
            });//查看排行

            infinityGridLeft.onCreateCell += OnCreateCellLeft;
            infinityGridLeft.onCellChange += onCellChangeLeft;
            infinityGridRight.onCreateCell += OnCreateCellRight;
            infinityGridRight.onCellChange += onCellChangeRight;

            infinityIrregularGrid.SetCapacity(99);
            infinityIrregularGrid.MinSize = 28;
            infinityIrregularGrid.onCreateCell += OnCreateCellBottom;
            infinityIrregularGrid.onCellChange += OnCellChangeBottom;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMyTotalRedPacket, UpdateMyTotalRedPacket, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMySystemRedPacket, UpdateMySystemRedPacket, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateShowRedPacket, UpdateShowRedPacket, toRegister);
            Sys_Family.Instance.eventEmitter.Handle<bool>(Sys_Family.EEvents.UpdateHistoryRedPacket, UpdateHistoryRedPacket, toRegister);
        }
        //刷新系统红包
        private void UpdateMySystemRedPacket()
        {
            RefreshSystemRedPacket();
        }
        //刷新红包已发总金币和总个数
        private void UpdateMyTotalRedPacket()
        {
            RefreshCurAllPacketMoneyAndNum();
        }
        //刷新八个红包
        private void UpdateShowRedPacket()
        {
            RefreshPacketCell(1);
        }
        //刷新红包日志
        private void UpdateHistoryRedPacket(bool isRefresh)
        {
            if(isRefresh)
                RefreshPacketCell(1);
            RefreshPacketCell(3);
            isDirty = true;
        }
        #endregion

        #region 界面显示
        private void InitView()
        {
            titlePanel.text = LanguageHelper.GetTextContent(2023950);
            titleLeft.text = LanguageHelper.GetTextContent(2023956);
            imageEmptyContent.text = LanguageHelper.GetTextContent(11933);
            imageEmptyBtnTopUp.transform.Find("Text_01").GetComponent<Text>().text= LanguageHelper.GetTextContent(2023957);
            textMyPacketTitle.text = LanguageHelper.GetTextContent(2023958);
            textSumTotalTitle.text = LanguageHelper.GetTextContent(2023960);
            textPacketValueTitle.text = LanguageHelper.GetTextContent(2023961);
            checkRank.transform.Find("Text").GetComponent<Text>().text = LanguageHelper.GetTextContent(2023963);
            btnSendPacket.transform.Find("Text_01").GetComponent<Text>().text = LanguageHelper.GetTextContent(2023962);
            textRule.text = LanguageHelper.GetTextContent(11934);
            titleRight.text= LanguageHelper.GetTextContent(2023964);

            Sys_Family.Instance.CheckSystemRedPacketActive();
            Sys_Family.Instance.CheckRedPoint();
            isDirty = true;
            RefreshSystemRedPacket();
            RefreshCurAllPacketMoneyAndNum();
            RefreshPacketCell(1);
            RefreshPacketCell(3);
        }
        private void RefreshSystemRedPacket()
        {
            //检测是否有可发送的系统红包
            if (Sys_Family.Instance.IsCanSendSystemRedPacket())
            {
                imageNormal.SetActive(true);
                imageEmpty.SetActive(false);
                RefreshPacketCell(0);
            }
            else
            {
                imageNormal.SetActive(false);
                imageEmpty.SetActive(true);
            }
        }
        /// <summary>
        /// 刷新当前已发送的总金币数量和发放红包份数
        /// </summary>
        private void RefreshCurAllPacketMoneyAndNum()
        {
            textSumTotal.text = Sys_Family.Instance.totalSendMoney.ToString();
            textPacketValue.text = Sys_Family.Instance.totalSendCount.ToString();
        }
        private void OnCreateCellLeft(InfinityGridCell cell)
        {
            CanSendPacketCell entry = new CanSendPacketCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void onCellChangeLeft(InfinityGridCell cell, int index)
        {
            CanSendPacketCell entry = cell.mUserData as CanSendPacketCell;
            entry.SetData(Sys_Family.Instance.systemRedPacketList[index]);//索引数据
        }
        private void OnCreateCellRight(InfinityGridCell cell)
        {
            CurrentPacketCell entry = new CurrentPacketCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void onCellChangeRight(InfinityGridCell cell, int index)
        {
            CurrentPacketCell entry = cell.mUserData as CurrentPacketCell;
            entry.SetData(Sys_Family.Instance.showRedPacketList[index]);//索引数据
        }
        private void OnCreateCellBottom(InfinityGridCell cell)
        {
            RecordCell entry = new RecordCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChangeBottom(InfinityGridCell cell, int index)
        {
            RecordCell entry = cell.mUserData as RecordCell;
            entry.SetData(Sys_Family.Instance.historyRedPacketList[index]);//索引数据
        }
        /// <summary>
        /// 刷新列表 type为0时刷新自己拥有的系统红包，为1时刷新展示的八个红包
        /// </summary>
        /// <param name="type"></param>
        public void RefreshPacketCell(int type)
        {
            if (type == 0)
            {
                infinityGridLeft.CellCount = Sys_Family.Instance.systemRedPacketList.Count;
                infinityGridLeft.ForceRefreshActiveCell();
            }
            else if (type == 1)
            {
                infinityGridRight.CellCount = Sys_Family.Instance.showRedPacketList.Count;
                infinityGridRight.ForceRefreshActiveCell();
                infinityGridRight.MoveToIndex(0);
            }
            else
            {
                infinityIrregularGrid.ForceRefreshActiveCell();
            }
        }
        #endregion

        #region Function
        private void GoCharge()
        {
            //跳转 商城-充值 界面
            MallPrama mallPrama = new MallPrama
            {
                mallId = 101,
                shopId = 1001,
                isCharge = true
            };
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
        }
        private void OnLateUpdate()
        {
            if (isDirty)
            {
                int count = Sys_Family.Instance.historyRedPacketList.Count;
                int i = 0;
                infinityIrregularGrid.Clear();
                while (i < 99 && infinityIrregularGrid.CellCount < count)
                {
                    ++i;
                    infinityIrregularGrid.Add(CalculateSize_Normal(Sys_Family.Instance.historyRedPacketList[infinityIrregularGrid.CellCount]));
                }
                if (infinityIrregularGrid.CellCount >= count)
                {
                    isDirty = false;
                }
            }
        }

        private int gContentMinHeight = 28;
        private int gSpace = 15;
        private int CalculateSize_Normal(RedPacketHistoryData data)
        {
            int h =0;
            if (!string.IsNullOrWhiteSpace(data.content))
            {
                h += gSpace;
                txtCommon_Text.text = data.content;
                h += Mathf.Max((int)txtCommon_Text.preferredHeight, gContentMinHeight);
            }
            return h;
        }
        #endregion
    }
    public class CanSendPacketCell
    {
        GameObject unsentObj;
        GameObject sentObj;
        Text content;
        Button btnSend;
        Text textTime;
        SystemRedPacketData data;
        int expireTime;
        public void Init(Transform trans)
        {
            unsentObj = trans.Find("Image_Icon1").gameObject;
            sentObj = trans.Find("Image_Icon2").gameObject;
            content = trans.Find("Text").GetComponent<Text>();
            btnSend = trans.Find("Btn_01_Small").GetComponent<Button>();
            textTime = trans.Find("Text_Time").GetComponent<Text>();

            btnSend.onClick.AddListener(SendPacket);//发送红包
            btnSend.transform.Find("Text_01").GetComponent<Text>().text = LanguageHelper.GetTextContent(2023959);
        }
        private void SendPacket()
        {
            //红包已过期
            if ((int)(data.expireTime - TimeManager.GetServerTime()) <= 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11943));
                return;
            }
            else
            {
                Sys_Family.Instance.OnSendRedPacketReq(data.packetId, LanguageHelper.GetTextContent(uint.Parse(CSVParam.Instance.GetConfData(1138).str_value)), data.packetData.Item_Num, 0, 0, true);
            }
        }
        public void SetData(SystemRedPacketData data)
        {
            this.data = data;
            SetPackState(data.state);
            content.text = LanguageHelper.GetTextContent(11964, data.packetData.parameter1.ToString(), data.packetData.Item_Num.ToString());
        }
        private void SetPackState(ESystemRedPacketState state)
        {
            expireTime = (int)(data.expireTime - TimeManager.GetServerTime());
            //未发送
            if (state == ESystemRedPacketState.Unsent)
            {
                unsentObj.SetActive(true);
                sentObj.SetActive(false);
                btnSend.interactable = true;
                if (expireTime > 0)//未过期
                {
                    int hour = expireTime > 3600 ? Mathf.FloorToInt((float)Math.Round(expireTime / 3600.0f, 1)) : 1;
                    textTime.text = LanguageHelper.GetTextContent(11940, hour.ToString());
                }
                else//已过期
                {
                    textTime.text = LanguageHelper.GetTextContent(11942);
                }
            }
            else if (state == ESystemRedPacketState.Send)
            {
                unsentObj.SetActive(false);
                sentObj.SetActive(true);
                btnSend.interactable = false;
                if (expireTime > 0)//未过期
                {
                    textTime.text = LanguageHelper.GetTextContent(11941);
                }
                else//已过期
                {
                    textTime.text = LanguageHelper.GetTextContent(11942);
                }
            }
        }
    }
    public class CurrentPacketCell
    {
        Transform trans;
        GameObject packetOpenObj;
        GameObject packetCloseObj;
        Text textName;
        Text textBlessing;
        Button btnClick;
        ShowRedPacketData data;
        public void Init(Transform trans)
        {
            this.trans = trans;
            packetOpenObj = trans.Find("Image_Open").gameObject;
            packetCloseObj = trans.Find("Image_Close").gameObject;
            textName = trans.Find("Text_Name").GetComponent<Text>();
            textBlessing = trans.Find("Blessing").GetComponent<Text>();
            btnClick = trans.GetComponent<Button>();

            RedPacketData curRedPacketData = new RedPacketData();
            btnClick.onClick.AddListener(OpenRedPacket);

        }
        public void SetData(ShowRedPacketData data)
        {
            this.data = data;
            SetPackState();
            textName.text = LanguageHelper.GetTextContent(11938, data.sendName);
            textBlessing.text = data.content;
        }
        private void SetPackState()
        {
            if (data.state == ERedPacketState.Unclaimed)
            {
                packetOpenObj.SetActive(false);
                packetCloseObj.SetActive(true);
            }
            else
            {
                packetOpenObj.SetActive(true);
                packetCloseObj.SetActive(false);
            }
        }
        private void OpenRedPacket()
        {
            Sys_Family.Instance.QueryEnvelopeInfoReq(data.packetId);
        }
    }
    public class RecordCell
    {
        Transform trans;
        Transform endTrans;
        Transform detailTrans;
        Text contentEnd;
        Text contentDetail;
        Button btnCheck;
        Text btnTitle;
        Image img;
        RedPacketHistoryData data;
        public void Init(Transform trans)
        {
            this.trans = trans;
            endTrans = trans.Find("EndMessage");
            detailTrans = trans.Find("ViewDetail");
            contentEnd = endTrans.GetComponent<Text>();
            contentDetail = detailTrans.GetComponent<Text>();
            btnCheck = detailTrans.Find("Text_Btn").GetComponent<Button>();
            btnTitle = detailTrans.Find("Text_Btn").GetComponent<Text>();
            img = detailTrans.Find("Text_Btn/Image").GetComponent<Image>();

            btnTitle.text = LanguageHelper.GetTextContent(11936);
            btnCheck.onClick.AddListener(OpenRedPacket);
        }
        public void SetData(RedPacketHistoryData data)
        {
            this.data = data;
            //抢完了
            if (data.finishTime != 0)
            {
                endTrans.gameObject.SetActive(true);
                detailTrans.gameObject.SetActive(false);
  
                contentEnd.text = data.content ;
            }
            else
            {
                endTrans.gameObject.SetActive(false);
                detailTrans.gameObject.SetActive(true);
                contentDetail.text = data.content;
                uint paramId = data.state == ERedPacketState.Opened ? 1277u : 1276u;
                ColorUtility.TryParseHtmlString(CSVParam.Instance.GetConfData(paramId).str_value, out Color _color);
                btnTitle.color = _color;
                img.color = _color;
            }
        }
        private void OpenRedPacket()
        {
            Sys_Family.Instance.QueryEnvelopeInfoReq(data.packetId);
        }
    }
}