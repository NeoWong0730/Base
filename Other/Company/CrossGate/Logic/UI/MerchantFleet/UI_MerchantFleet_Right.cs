using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System;
using Framework;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    public class UI_MerchantFleet_Right
    {
        private Text txt_Title;
        private Text txt_TradeCount;
        private GameObject go_TypeTask;
        private GameObject go_TypeTrade;
        private GameObject go_TypeFight;
        TypeTask type_task = new TypeTask();
        TypeTrade type_trade = new TypeTrade();
        TypeFight type_fight = new TypeFight();
        private IListener listener;
        public void BindGameObject(Transform trans)
        {
            txt_Title = trans.Find("Title_Tips01/Text_Title").GetComponent<Text>();
            txt_TradeCount = trans.Find("Text_Tips1/Text_Num").GetComponent<Text>();
            go_TypeTask = trans.Find("Type1").gameObject;
            type_task.Init(go_TypeTask.transform);
            go_TypeTrade = trans.Find("Type2").gameObject;
            type_trade.Init(go_TypeTrade.transform);
            go_TypeFight = trans.Find("Type3").gameObject;
            type_fight.Init(go_TypeFight.transform);
        }
        public void Show()
        {
            DefaultType();
        }

        public void Update()
        {
            PanelShow();
        }
        public void Destory()
        {
            type_fight.Destory();
        }
        private void PanelShow()
        {
            int _total = Sys_MerchantFleet.Instance.AllMerchantACount();
            txt_TradeCount.text = Sys_MerchantFleet.Instance.MerchantTotalCount + "/" + _total;
            txt_TradeCount.color = CSVWordStyle.Instance.GetConfData(167).FontColor;
            TaskTypeShow();
        }
        private void TaskTypeShow()
        {
            DefaultType();
            if (Sys_MerchantFleet.Instance.IsAllMerchantTaskFinish())
            {
                txt_TradeCount.color = CSVWordStyle.Instance.GetConfData(75).FontColor;
                return;
            }
            switch (Sys_MerchantFleet.Instance.MerchantTradeTaskType)
            {
                case 0:
                    type_task.SetData();
                    go_TypeTask.SetActive(true);
                    break;
                case 1:
                    type_fight.AddRefreshListener(OnReceived);
                    type_fight.SetData();
                    go_TypeFight.SetActive(true);
                    break;
                case 2:
                    go_TypeTrade.SetActive(true);
                    type_trade.AddRefreshListener(OnReceived,OnError);
                    type_trade.SetData();
                    break;
            }
        }
        private void DefaultType()
        {
            go_TypeTask.SetActive(false);
            go_TypeTrade.SetActive(false);
            go_TypeFight.SetActive(false);
        }
        private void OnReceived()
        {
            listener?.OnTradeButtonClicked();
        }
        private void OnError()
        {
            go_TypeTrade.SetActive(false);
        }
        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnTradeButtonClicked();
        }
        public class TypeTask
        {
            private Text title;
            private Text content;
            private GameObject grid;
            private GameObject propItem;
            private Button btn;
            private Text btnText;
            CSVTask.Data data;
            public void Init(Transform trans)
            {
                title = trans.Find("Text_Title").GetComponent<Text>();
                content = trans.Find("Text_Task").GetComponent<Text>();
                grid = trans.Find("Grid").gameObject;
                propItem = trans.Find("Grid/PropItem").gameObject;
                btn = trans.Find("Btn_01").GetComponent<Button>();
                btnText = trans.Find("Btn_01/Text_01").GetComponent<Text>();
                btn.onClick.AddListener(OnButtonClicked);
            }
            public void SetData()
            {
                uint totalCount = Sys_MerchantFleet.Instance.OneRoundTotalCount();
                title.text = LanguageHelper.GetTextContent(2028625, Sys_MerchantFleet.Instance.MerchantTaskCount.ToString(), totalCount.ToString());//本次任务({0}/{1})
                btnText.text = LanguageHelper.GetTextContent(2028627);

                if (Sys_MerchantFleet.Instance.TaskId != 0)
                {
                    data = CSVTask.Instance.GetConfData(Sys_MerchantFleet.Instance.TaskId);
                    if (data != null)
                    {
                        content.text = LanguageHelper.GetTaskTextContent(data.taskDescribe);
                    }
                    List<ItemIdCount> _list = Sys_MerchantFleet.Instance.TaskDropItem(Sys_MerchantFleet.Instance.TaskId);
                    propItem.SetActive(true);
                    Sys_MerchantFleet.Instance.InitPropItem(propItem, _list, EUIID.UI_MerchantFleet,false);
                }
                else
                {
                    propItem.SetActive(false);
                    content.text = LanguageHelper.GetTextContent(2028660);
                }


                //ImageHelper.SetImageGray(btn.GetComponent<Image>(), Sys_MerchantFleet.Instance.IsMerchantTaskAccept);
            }
            private void OnButtonClicked()
            {
                if (Sys_MerchantFleet.Instance.IsMerchantTaskAccept)
                {
                    if (data != null && data.receiveNpc != 0)
                    {
                        ActionCtrl.Instance.MoveToTargetNPCAndInteractive(data.receiveNpc);
                        UIManager.CloseUI(EUIID.UI_MerchantFleet);
                    }
                    return;
                }
                Sys_MerchantFleet.Instance.OnMerchantAccpetTaskReq();
            }
        }
        public class TypeTrade
        {
            private Button btn_help;
            private Button btn_quick;
            private Text txt_quick;
            private Button btn_commit;
            private Button btn_sure;
            private Text txt_commit;
            private Text txt_seekHelpCount;
            private GameObject commitProp;
            private GameObject awardProp;
            private Image costImg;
            private Text costText;
            private CSVMerchantFleetTask.Data data;
            private Action onOver;
            private Action onError;
            private int index;
            private int quickCount;
            public void Init(Transform trans)
            {
                btn_help = trans.Find("Btn_Help").GetComponent<Button>();
                btn_quick = trans.Find("Btn_02").GetComponent<Button>();
                txt_quick = trans.Find("Btn_02/Text_01").GetComponent<Text>();
                btn_commit = trans.Find("Btn_01").GetComponent<Button>();
                txt_commit = trans.Find("Btn_01/Text_01").GetComponent<Text>();
                txt_seekHelpCount = trans.Find("Text1/Num").GetComponent<Text>();
                btn_sure = trans.Find("Btn_03").GetComponent<Button>();
                commitProp = trans.Find("Grid_Need/PropItem").gameObject;
                awardProp = trans.Find("Grid/PropItem").gameObject;
                costImg = trans.Find("Cost_Coin").GetComponent<Image>();
                costText = trans.Find("Cost_Coin/Text_Cost").GetComponent<Text>();
                btn_help.onClick.AddListener(OnHelpButtonClicked);
                btn_quick.onClick.AddListener(OnQuickButtonClicked);
                btn_commit.onClick.AddListener(OnCommitButtonClicked);
                btn_sure.onClick.AddListener(OnSureButtonClicked);
            }
            public void SetData()
            {
                data = Sys_MerchantFleet.Instance.taskData;
                if (data == null) return;
                index = Sys_MerchantFleet.Instance.MerchantIndex;
                if (index <0|| index >= data.handItem.Count || index >= data.handReward.Count||index>=data.handCost.Count)
                {
                    DebugUtil.LogError("MerchantFleet:Sever Send TradeTaskIndex Is ERROR");
                    onError?.Invoke();
                    return;
                }
                int helpCount = int.Parse(CSVParam.Instance.GetConfData(1552).str_value);
                txt_seekHelpCount.text = LanguageHelper.GetTextContent(2028661, (helpCount - Sys_MerchantFleet.Instance.SeekHelpCount).ToString());
                SetCost();
                SetButton();
                var _list = new List<ItemIdCount>();
                var _id = data.handItem[index][0];
                var _count = data.handItem[index][1];
                _list.Add(new ItemIdCount(_id, _count));
                Sys_MerchantFleet.Instance.InitPropItem(commitProp, _list, EUIID.UI_MerchantFleet,true);

                _list = CSVDrop.Instance.GetDropItem(data.handReward[index]);
                Sys_MerchantFleet.Instance.InitPropItem(awardProp, _list, EUIID.UI_MerchantFleet,false);
            }
            private void SetCost()
            {
                var item = CSVItem.Instance.GetConfData(data.handCost[index][0]);
                ImageHelper.SetIcon(costImg, item.icon_id);
                costText.text = data.handCost[index][1].ToString();
                if (Sys_Bag.Instance.GetItemCount(data.handCost[index][0]) >= data.handCost[index][1])
                {
                    costText.color = CSVWordStyle.Instance.GetConfData(168).FontColor;
                }
                else
                {
                    costText.color = CSVWordStyle.Instance.GetConfData(75).FontColor;//道具不满足时，数字颜色为红色
                }
            }
            private void SetButton()
            {
                btn_commit.interactable =true;
                btn_quick.interactable = true;
                costImg.gameObject.SetActive(!Sys_MerchantFleet.Instance.TradeTaskHelp);
                btn_commit.gameObject.SetActive(!Sys_MerchantFleet.Instance.TradeTaskHelp);
                btn_quick.gameObject.SetActive(!Sys_MerchantFleet.Instance.TradeTaskHelp);

                quickCount = int.Parse(CSVParam.Instance.GetConfData(1548).str_value);
                txt_quick.text = LanguageHelper.GetTextContent(2028623, (quickCount - Sys_MerchantFleet.Instance.QuickFinishCount).ToString());
                if (Sys_MerchantFleet.Instance.QuickFinishCount >= quickCount)
                {
                    txt_quick.color = CSVWordStyle.Instance.GetConfData(75).FontColor;
                }
                else
                {
                    txt_quick.color = CSVWordStyle.Instance.GetConfData(14).FontColor;
                }
                txt_commit.text = LanguageHelper.GetTextContent(2028618);//贸易
                btn_sure.gameObject.SetActive(Sys_MerchantFleet.Instance.TradeTaskHelp);

            }
            public void AddRefreshListener(Action onOvered = null,Action onErrored=null)
            {
                onOver = onOvered;
                onError = onErrored;
            }
            private void OnQuickButtonClicked()
            {
                if (Sys_Bag.Instance.GetItemCount(data.handCost[index][0]) >= data.handCost[index][1])
                {
                    if (Sys_MerchantFleet.Instance.QuickFinishCount < quickCount)
                    {
                        var item = CSVItem.Instance.GetConfData(data.handCost[index][0]);
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2028641, LanguageHelper.GetTextContent(item.name_id), data.handCost[index][1].ToString());
                        PromptBoxParameter.Instance.SetConfirm(true, () =>
                        {
                            Sys_MerchantFleet.Instance.TradeResultShow(true);
                            onOver?.Invoke();
                            btn_commit.interactable = false;
                            btn_quick.interactable = false;
                        });
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);

                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028624));//本周次数已用完
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028652));//道具数量不足
                }

            }
            private void OnHelpButtonClicked()
            {//打开家族求助
                Sys_MerchantFleet.Instance.OpenFamilyHelp();
            }
            private void OnCommitButtonClicked()
            {
                if (Sys_Bag.Instance.GetItemCount(data.handItem[index][0]) >= data.handItem[index][1])
                {
                    Sys_MerchantFleet.Instance.TradeResultShow(false);
                    onOver?.Invoke();
                    btn_commit.interactable = false;
                    btn_quick.interactable = false;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028652));//道具数量不足
                }
            }
            private void OnSureButtonClicked()
            {
                Sys_MerchantFleet.Instance.OnMerchantReceviceHelpReq();//确认被帮助的贸易
            }
        }
        public class TypeFight
        {
            private Text title;
            private GameObject propItem;
            private Button btn;
            private Text needpoint;
            private Text mypoint;
            private Button btnPartner;
            private Action onOver;
            private Timer m_timer;
            private bool isfightclick = false;
            public void Init(Transform trans)
            {
                title = trans.Find("Text_Title").GetComponent<Text>();
                needpoint = trans.Find("Text_Point01/Text_Num").GetComponent<Text>();
                mypoint = trans.Find("Text_Point02/Text_Num").GetComponent<Text>();
                propItem = trans.Find("Grid/PropItem").gameObject;
                btn = trans.Find("Btn_01").GetComponent<Button>();
                btnPartner = trans.Find("Button_Partner").GetComponent<Button>();
                btn.onClick.AddListener(OnFightButtonClicked);
                btnPartner.onClick.AddListener(OnPartnerButtonClicked);
            }
            public void SetData()
            {
                isfightclick = false;
                var taskData = Sys_MerchantFleet.Instance.taskData;
                if (taskData == null)return;
                
                List<ItemIdCount> _list = CSVDrop.Instance.GetDropItem(taskData.battleReward);
                Sys_MerchantFleet.Instance.InitPropItem(propItem, _list, EUIID.UI_MerchantFleet,false);
                needpoint.text = taskData.battlePoint.ToString();
                mypoint.text = Sys_Attr.Instance.rolePower.ToString();//角色评分
                if ((ulong)taskData.battlePoint > Sys_Attr.Instance.rolePower)
                {
                    mypoint.color = CSVWordStyle.Instance.GetConfData(75).FontColor;
                }
                else
                {
                    mypoint.color = CSVWordStyle.Instance.GetConfData(167).FontColor;
                }
            }
            public void Destory()
            {
                m_timer?.Cancel();
            }
            private void OnFightButtonClicked()
            {
                if (Sys_Team.Instance.HaveTeam&&!Sys_Team.Instance.isPlayerLeave())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028656));//您当前有队伍，需要暂离前往
                    return;
                }
                if (isfightclick) return;
                Sys_MerchantFleet.Instance.InitBattleTempList();
                isfightclick = true;
                onOver?.Invoke();
                m_timer?.Cancel();
                m_timer = Timer.Register(Sys_MerchantFleet.Instance.sheepAnimationTime,OnFresh);
            }
            private void OnFresh()
            {
                Sys_MerchantFleet.Instance.OnMerchantFightTradeTaskReq();
                isfightclick = false;
                UIManager.CloseUI(EUIID.UI_MerchantFleet);
            }
            private void OnPartnerButtonClicked()
            {
                PartnerUIParam param = new PartnerUIParam();
                param.tabIndex = 1;
                UIManager.OpenUI(EUIID.UI_Partner, false, param);
            }
            public void AddRefreshListener(Action onOvered = null)
            {
                onOver = onOvered;
            }

        }
    }
}
