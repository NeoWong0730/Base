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


//家族求助界面
public class UI_MerchantFleet_FamilyHelp : UIBase
{
    private CP_ToggleRegistry tg_Registry;
    private InfinityGrid m_InfinityGrid;
    private Button btn_Close;
    private Button btn_Publish;
    private GameObject go_Empty;
    private Text txt_lastHelpCount;
    private Text txt_seekHelpCount;
    private int nowType = 1;//1-全部 2-自己
    protected DateTime tickTime;
    private List<TradeFamilyHelpCeil> m_List=new List<TradeFamilyHelpCeil>();
    #region 系统函数
    protected override void OnLoaded()
    {
        tg_Registry = transform.Find("Animator/Image_BG1/Menu").GetComponent<CP_ToggleRegistry>();
        m_InfinityGrid = transform.Find("Animator/Image_BG1/Scroll_Rank").GetComponent<InfinityGrid>();
        go_Empty = transform.Find("Animator/View_empty").gameObject;
        btn_Close = transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();
        btn_Publish = transform.Find("Animator/Button").GetComponent<Button>();
        txt_lastHelpCount = transform.Find("Animator/Text/Text_Num").GetComponent<Text>();
        txt_seekHelpCount = transform.Find("Animator/Text1/Text_Num").GetComponent<Text>();
        btn_Close.onClick.AddListener(OnCloseButtonClicked);
        btn_Publish.onClick.AddListener(OnPublicButtonClicked);
        m_InfinityGrid.CellCount = Sys_MerchantFleet.Instance.MerchantHelpInfoList.Count;
        m_InfinityGrid.onCreateCell += OnCreateCell;
        m_InfinityGrid.onCellChange += OnCellChange;
        tg_Registry.onToggleChange += OnToggleChange;

    }
    protected override void ProcessEventsForEnable(bool toRegister)
    {
        Sys_MerchantFleet.Instance.eventEmitter.Handle(Sys_MerchantFleet.EEvents.UpdateFamilyMerchantHelp, Refresh, toRegister);
    }
    protected override void OnShow()
    {
        Sys_MerchantFleet.Instance.OnMerchantHelpListReq();
    }
    protected override void OnHide()
    {
        for (int i=0;i< m_List.Count;i++)
        {
            m_List[i].OnDestory();
        }
        m_List.Clear();
    }
    protected override void OnUpdate()
    {
        
    }
    #endregion
    private void OnCreateCell(InfinityGridCell cell)
    {
        TradeFamilyHelpCeil entry = new TradeFamilyHelpCeil();
        entry.BindGameObject(cell.mRootTransform.gameObject);
        cell.BindUserData(entry);
    }
    private void OnCellChange(InfinityGridCell cell, int index)
    {
        TradeFamilyHelpCeil entry = cell.mUserData as TradeFamilyHelpCeil;
        entry.OnDestory();
        entry.SetData(nowType,index);
        m_List.Add(entry);
    }
    private void OnToggleChange(int current, int old)
    {
        if (nowType == current)
            return;

        nowType = current;
        Refresh();
    }
    private void PanelShow()
    {
        int helpCount = int.Parse(CSVParam.Instance.GetConfData(1553).str_value);
        txt_lastHelpCount.text = LanguageHelper.GetTextContent(2028661, (helpCount-Sys_MerchantFleet.Instance.HelpCount).ToString());
        helpCount = int.Parse(CSVParam.Instance.GetConfData(1552).str_value);
        txt_seekHelpCount.text = LanguageHelper.GetTextContent(2028661, (helpCount - Sys_MerchantFleet.Instance.SeekHelpCount).ToString());
    }
    private void Refresh()
    {
        PanelShow();
        if (nowType == 1)
        {
            m_InfinityGrid.CellCount = Sys_MerchantFleet.Instance.MerchantHelpInfoList.Count;
        }
        else
        {
            m_InfinityGrid.CellCount = Sys_MerchantFleet.Instance.MerchantHelpSelfList.Count;
        }
        go_Empty.SetActive(m_InfinityGrid.CellCount==0);
        m_InfinityGrid.ForceRefreshActiveCell();
       
    }

    private void OnCloseButtonClicked()
    {
        UIManager.CloseUI(EUIID.UI_MerchantFleet_FamilyHelp);
    }

    private void OnPublicButtonClicked()
    {
        if (Sys_MerchantFleet.Instance.IsTradeHelpPublish)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028662));//当前贸易委托已发布
            return;
        }
        if (Sys_MerchantFleet.Instance.MerchantTradeTaskType==2&&!Sys_MerchantFleet.Instance.TradeTaskHelp)
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2028640);//是否发布
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_MerchantFleet.Instance.OnMerchantSeekHelpReq();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
        else
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028663));//目前无委托发布
        }
    }

    public class TradeFamilyHelpCeil
    {
        private Text m_name;
        private Text m_time;
        private Button m_help;
        private Button m_cancel;
        private Button m_sure;
        private GameObject prop1;
        private GameObject prop2;
        public int index;//对应MerchantHelpInfoList
        private bool isMine;//是否是自己的委托
        MerchantHelpInfo singleData;
        CSVMerchantFleetTask.Data data;
        Timer m_timer;
        float m_duration;
        public void BindGameObject(GameObject go)
        {
            Transform trans = go.transform;
            m_name = trans.Find("Text_Name").GetComponent<Text>();
            m_time = trans.Find("Text_Time").GetComponent<Text>();
            m_help = trans.Find("Btn1").GetComponent<Button>();
            m_cancel = trans.Find("Btn2").GetComponent<Button>();
            m_sure = trans.Find("Btn3").GetComponent<Button>();
            prop1 = trans.Find("Grid1/PropItem").gameObject;
            prop2 = trans.Find("Grid2/PropItem").gameObject;
            m_help.onClick.AddListener(OnHelpButtonClicked);
            m_cancel.onClick.AddListener(OnCancelButtonClicked);
            m_sure.onClick.AddListener(OnSureButtonClicked);
        }
        public void OnDestory()
        {
            m_timer?.Cancel();
        }
        public void SetData(int _type,int _index)
        {
            index = _index;
            if (index>=Sys_MerchantFleet.Instance.MerchantHelpInfoList.Count)return;
            
            if (_type==1)
            {
                singleData = Sys_MerchantFleet.Instance.MerchantHelpInfoList[index];
            }
            else
            {
                singleData = Sys_MerchantFleet.Instance.MerchantHelpSelfList[index];
            }
            m_help.enabled = true;
            SetPanel();
            SetPropItem();
            SetTimer();
        }
        private void SetPanel()
        {
            m_name.text = singleData.Name.ToStringUtf8();
            isMine = (Sys_Role.Instance.Role.RoleId == singleData.RoleId);
            m_sure.gameObject.SetActive(isMine&&Sys_MerchantFleet.Instance.TradeTaskHelp);
            m_cancel.gameObject.SetActive(isMine&&!Sys_MerchantFleet.Instance.TradeTaskHelp);
            m_help.gameObject.SetActive(!isMine);
        }
        private void SetTimer()
        {
            DateTime _nowTime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            m_duration = (float)Sys_MerchantFleet.Instance.m_MondayFive.Subtract(_nowTime).TotalSeconds;
            m_timer?.Cancel();
            m_timer =Timer.Register(m_duration, OnTimerComplete, OnTimerUpdate, false, false);
        }
        private void OnTimerComplete()
        {
            m_timer?.Cancel();
        }
        private void OnTimerUpdate(float time)
        {
            if (m_time != null && m_duration >= time)
            {
                m_time.text=LanguageHelper.TimeToString((uint)(m_duration - time), LanguageHelper.TimeFormat.Type_9);
            }  
        }
        private void SetPropItem()
        {
            data = CSVMerchantFleetTask.Instance.GetConfData(singleData.InfoId);
            int _index = (int)singleData.TradeTaskIndex;
            if (_index>= data.handItem.Count|| _index >= data.handReward.Count)
            {
                DebugUtil.LogError("MerchantFleet:Sever Send TradeTaskIndex(FamilyHelp) Is ERROR.RoleId"+ singleData.RoleId);
                m_help.enabled = false;
                return;
            }
            var _list = new List<ItemIdCount>();

            _list.Add(new ItemIdCount(data.handItem[_index][0], data.handItem[_index][1]));
            Sys_MerchantFleet.Instance.InitPropItem(prop1, _list, EUIID.UI_MerchantFleet_FamilyHelp,true);

            _list = CSVDrop.Instance.GetDropItem(data.handReward[_index]);
            Sys_MerchantFleet.Instance.InitPropItem(prop2, _list, EUIID.UI_MerchantFleet_FamilyHelp,false);
        }
        private void OnHelpButtonClicked()
        {
            var itemId = data.handItem[(int)singleData.TradeTaskIndex][0];
            var itemCount = data.handItem[(int)singleData.TradeTaskIndex][1];
            var param = CSVParam.Instance.GetConfData(1553).str_value;
            if (itemCount > Sys_Bag.Instance.GetItemCount(itemId))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028652));//当前所需道具不足
            }
            else if (Sys_MerchantFleet.Instance.HelpCount >= int.Parse(param))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028653));//帮助达到上限
            }
            else
            {
                Sys_MerchantFleet.Instance.TempHelpInfo = singleData;
                var _data = CSVItem.Instance.GetConfData(itemId);
                string _itemname = LanguageHelper.GetTextContent(_data.name_id);
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2028654, _itemname, itemCount.ToString());//是否帮助
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_MerchantFleet.Instance.OnMerchantSubmitHelpReq(singleData.RoleId);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);


            }
        }
        private void OnCancelButtonClicked()
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2028655);//是否取消
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_MerchantFleet.Instance.OnMerchantCancelHelpReq();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
        private void OnSureButtonClicked()
        {
            Sys_MerchantFleet.Instance.OnMerchantReceviceHelpReq();//确认被帮助的贸易
        }
    }
}
