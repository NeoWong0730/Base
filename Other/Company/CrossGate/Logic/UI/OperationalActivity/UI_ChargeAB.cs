using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Table;
using System;
using Logic.Core;

namespace Logic
{
    public class UI_ChargeAB : UI_OperationalActivityBase
    {
        private int index;
        private List<uint> listIds = new List<uint>();

        private Button btnLeft;
        private Button btnRight;
        private Text txtTime;
        private UI_ChargeABCell cellLeft;
        private UI_ChargeABCell cellRight;

        private Timer timer;
        private float countDownTime = 0;
        private bool isReq;
        #region 系统函数
        protected override void Loaded()
        {

        }
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void OnDestroy()
        {
            timer?.Cancel();
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            if (!isReq)
            {
                Sys_OperationalActivity.Instance.ReqChargeABData();
                isReq = true;
            }
            index = Sys_OperationalActivity.Instance.GetChargeABOpenIndex();
            UpdateView();
        }
        public override void Hide()
        {
            timer?.Cancel();
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateChargeABData, OnUpdateChargeABData, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }
        public override void SetOpenValue(uint openValue)
        {

        }
        #endregion
        #region func
        private void Parse()
        {
            btnLeft = transform.Find("Arrow_Left/Btn_Left").GetComponent<Button>();
            btnLeft.onClick.AddListener(OnBtnLeftClick);
            btnRight = transform.Find("Arrow_Right/Btn_Right").GetComponent<Button>();
            btnRight.onClick.AddListener(OnBtnRightClick);
            cellLeft = new UI_ChargeABCell();
            cellLeft.Init(transform.Find("View_left"));
            cellRight = new UI_ChargeABCell();
            cellRight.Init(transform.Find("View_Right"));
            txtTime = transform.Find("Text_Time").GetComponent<Text>();
        }
        private void UpdateView()
        {
            StartTimer();
            listIds = Sys_OperationalActivity.Instance.listIds;
            cellLeft.UpdateCellView(listIds[index * 2]);
            uint rightId = index * 2 + 1 < listIds.Count ? listIds[index * 2 + 1] : 0;
            cellRight.UpdateCellView(rightId);
            //按钮显示
            btnLeft.gameObject.SetActive(index > 0);
            btnRight.gameObject.SetActive(index < (listIds.Count - 1) / 2);
        }
        private void StartTimer()
        {
            uint nowtime = Sys_Time.Instance.GetServerTime();
            var targetTime = Sys_OperationalActivity.Instance.GetChargeABActivityOverTimestamp();
            countDownTime = targetTime - nowtime;
            timer?.Cancel();
            timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
        }
        #endregion
        #region event
        private void OnBtnLeftClick()
        {
            if (index > 0)
            {
                index--;
                UpdateView();
            }
        }
        private void OnBtnRightClick()
        {
            if (index < (listIds.Count - 1) / 2)
            {
                index++;
                UpdateView();
            }
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            //刷新福利界面
            Sys_OperationalActivity.Instance.eventEmitter.Trigger(Sys_OperationalActivity.EEvents.OnChargeABActivityEnd);
        }
        private void OnTimerUpdate(float time)
        {
            if (countDownTime >= time && txtTime != null)
            {
                txtTime.text = LanguageHelper.GetTextContent(2027004, LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_4));
            }
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            UpdateView();
        }
        private void OnUpdateChargeABData()
        {
            UpdateView();
        }
        #endregion

        public class UI_ChargeABCell
        {
            private uint id;
            private bool canGet;
            private bool isGet;

            private Transform transform;

            private Text txtChargeDesc;
            private Text txtChargeNum;
            private UI_ChargeABRewardCell cellA;
            private UI_ChargeABRewardCell cellB;
            private Button btnA;
            private Button btnB;
            private Text txtBtnA;
            private Text txtBtnB;

            public void Init(Transform trans)
            {
                transform = trans;
                txtChargeDesc = transform.Find("Text_Num1").GetComponent<Text>();
                txtChargeNum = transform.Find("Text_Num2").GetComponent<Text>();
                cellA = new UI_ChargeABRewardCell();
                cellA.Init(transform.Find("Item01"), 1);
                cellB = new UI_ChargeABRewardCell();
                cellB.Init(transform.Find("Item01 (1)"), 2);
                btnA = transform.Find("Button1").GetComponent<Button>();
                btnA.onClick.AddListener(OnBtnAClick);
                btnB = transform.Find("Button2").GetComponent<Button>();
                btnB.onClick.AddListener(OnBtnBClick);
                txtBtnA = transform.Find("Button1/Text").GetComponent<Text>();
                txtBtnB = transform.Find("Button2/Text").GetComponent<Text>();
            }
            public void UpdateCellView(uint _id)
            {
                id = _id;
                CSVRechargeSelection.Data data = CSVRechargeSelection.Instance.GetConfData(id);
                if (data != null)
                {
                    transform.gameObject.SetActive(true);
                    uint needChargeNum = data.Recharge;
                    txtChargeDesc.text = LanguageHelper.GetTextContent(2027002, needChargeNum.ToString());//累计充值{0}魔币
                    uint curChargeNum = Sys_OperationalActivity.Instance.GetChargeABTotalChargeNum();
                    uint showChargeNum = Math.Min(curChargeNum, needChargeNum);
                    bool isFull = curChargeNum >= needChargeNum;//充满
                    uint languageId = isFull ? 2027006u : 2027007u;//绿 : 白 {0}/{1}
                    txtChargeNum.text = LanguageHelper.GetTextContent(languageId, showChargeNum.ToString(), needChargeNum.ToString());
                    uint getState = Sys_OperationalActivity.Instance.CheckChargeABRewardGetState(data.Sort);
                    canGet = isFull && getState == 0;
                    isGet = getState > 0;
                    cellA.UpdateCellView(data.reward[0], getState);
                    cellB.UpdateCellView(data.reward[1], getState);
                    ImageHelper.SetImageGray(btnA.GetComponent<Image>(), false, true);
                    ImageHelper.SetImageGray(btnB.GetComponent<Image>(), false, true);
                    if (isFull)
                    {
                        if (getState > 0)
                        {
                            //已领取
                            txtBtnA.text = LanguageHelper.GetTextContent(getState == 1 ? 4702u : 4726u);//4726 领取 | 4702 已领取
                            txtBtnB.text = LanguageHelper.GetTextContent(getState == 2 ? 4702u : 4726u);
                            ImageHelper.SetImageGray(btnA.GetComponent<Image>(), true, true);
                            ImageHelper.SetImageGray(btnB.GetComponent<Image>(), true, true);
                        }
                        else
                        {
                            txtBtnB.text = txtBtnA.text = LanguageHelper.GetTextContent(4726);//领取
                        }
                    }
                    else
                    {
                        txtBtnB.text = txtBtnA.text = LanguageHelper.GetTextContent(4730);//前往充值
                    }
                }
                else
                {
                    transform.gameObject.SetActive(false);
                }
            }

            private void OnBtnAClick()
            {
                OnGetBtnClick(1);
            }
            private void OnBtnBClick()
            {
                OnGetBtnClick(2);
            }
            private void OnGetBtnClick(uint index)
            {
                if (!isGet)
                {

                    if (canGet)
                    {
                        CSVRechargeSelection.Data data = CSVRechargeSelection.Instance.GetConfData(id);
                        Sys_OperationalActivity.Instance.ReqChargeABGetReward(data.Sort, index);
                    }
                    else
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
                }
            }
        }

        public class UI_ChargeABRewardCell
        {
            private uint index;

            private Transform transform;
            private GameObject goIsGet;
            private Transform itemParent1;
            private Transform itemParent2;
            private List<PropItem> listItemFull = new List<PropItem>();
            private List<PropItem> listItemUnFull = new List<PropItem>();


            public void Init(Transform trans, uint _index)
            {
                index = _index;
                transform = trans;
                goIsGet = transform.Find("Image_Get").gameObject;
                itemParent1 = transform.Find("Grid01");
                listItemFull.Clear();
                for (int i = 0; i < itemParent1.childCount; i++)
                {
                    var goItem = itemParent1.GetChild(i).gameObject;
                    PropItem itemCell = new PropItem();
                    itemCell.BindGameObject(goItem);
                    listItemFull.Add(itemCell);
                }
                itemParent2 = transform.Find("Grid02");
                listItemUnFull.Clear();
                for (int i = 0; i < itemParent2.childCount; i++)
                {
                    var goItem = itemParent2.GetChild(i).gameObject;
                    PropItem itemCell = new PropItem();
                    itemCell.BindGameObject(goItem);
                    listItemUnFull.Add(itemCell);
                }
            }
            public void UpdateCellView(uint dropId, uint getState)
            {
                goIsGet.SetActive(getState == index);
                var rewardItems = CSVDrop.Instance.GetDropItem(dropId);
                if (rewardItems.Count > 3)
                {
                    itemParent1.gameObject.SetActive(true);
                    itemParent2.gameObject.SetActive(false);
                    for (int i = 0; i < listItemFull.Count; i++)
                    {
                        var propItem = listItemFull[i];
                        if (i < rewardItems.Count)
                        {
                            propItem.transform.gameObject.SetActive(true);
                            var itemData = new PropIconLoader.ShowItemData(rewardItems[i].id, rewardItems[i].count, true, false, false, false, false, true);
                            propItem.SetData(itemData, EUIID.UI_OperationalActivity);
                        }
                        else
                        {
                            propItem.transform.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    itemParent1.gameObject.SetActive(false);
                    itemParent2.gameObject.SetActive(true);
                    for (int i = 0; i < listItemUnFull.Count; i++)
                    {
                        var propItem = listItemUnFull[i];
                        if (i < rewardItems.Count)
                        {
                            propItem.transform.gameObject.SetActive(true);
                            var itemData = new PropIconLoader.ShowItemData(rewardItems[i].id, rewardItems[i].count, true, false, false, false, false, true);
                            propItem.SetData(itemData, EUIID.UI_OperationalActivity);
                        }
                        else
                        {
                            propItem.transform.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}
