using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;

public class UI_MessageBag_Layout
{
    public Button btn_Close;
    public CP_ToggleRegistry tg_MessageTable;
    public GameObject go_MessageItem;
    public InfinityGrid messageGrid;

    public void Init(Transform transform)
    {
        btn_Close = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
        tg_MessageTable = transform.Find("Animator/Content/View_Toggle/Toggles").GetComponent<CP_ToggleRegistry>();
        go_MessageItem = transform.Find("Animator/Content/Message").gameObject;
        messageGrid = transform.Find("Animator/Content/Scroll View").GetComponent<InfinityGrid>();
    }
    public void RegisterEvents(IListener listener)
    {
        btn_Close.onClick.AddListener(listener.OnBtnCloseClicked);
    }
    public interface IListener
    {
        void OnBtnCloseClicked();
    }


}
public class UI_MessageBag:UIBase, UI_MessageBag_Layout.IListener
{
    private UI_MessageBag_Layout layout = new UI_MessageBag_Layout();
    private EMessageBagType messageType = EMessageBagType.Team;
    private Sys_MessageBag.MessageData messageData;
    private List<Sys_MessageBag.MessageContent> currList= new  List<Sys_MessageBag.MessageContent>();
    private List<UI_MessageBag_Ceil> entryList = new List<UI_MessageBag_Ceil>();
    private List<GameObject> toggleList = new List<GameObject>();
    //private UI_MessageBag_RedPoint redPoint;

    #region 系统函数

    protected override void OnLateUpdate(float dt, float usdt)
    {
        if (entryList.Count==0)
        {
            return;
        }
        for (int i=0;i<entryList.Count;i++)
        {
            entryList[i].OnRefresh();//刷新倒计时
        }
        RefreshMessageBagRedPoint();
    }
    protected override void OnOpen(object arg)
    {
        if (arg == null)
            return;
        messageType = (EMessageBagType)((uint)arg);

    }
    protected override void OnLoaded()
    {
        
        layout.Init(transform);
        layout.RegisterEvents(this);
        Sys_MessageBag.Instance.ClickType = Sys_MessageBag.Instance.JumpType;
        layout.messageGrid.CellCount = 20;
        layout.tg_MessageTable.onToggleChange += OnToggleChange;
        layout.messageGrid.onCreateCell += OnCreateCell;
        layout.messageGrid.onCellChange += OnCellChange;
        Transform toggleTran = layout.tg_MessageTable.gameObject.transform;
        toggleList.Clear();
        for (int i = 0; i < toggleTran.childCount; i++)
        {
            GameObject go = toggleTran.GetChild(i).gameObject;
            toggleList.Add(go);
        }
        RefreshMessageBagRedPoint();
        //redPoint = gameObject.AddComponent<UI_MessageBag_RedPoint>();
        //redPoint?.Init(this);

    }

    protected override void OnShow()
    {
        messageType = Sys_MessageBag.Instance.ClickType;
        layout.tg_MessageTable.SwitchTo((int)messageType);
        RefreshGroup();
    }

    protected override void OnDestroy()
    {        
        currList.Clear();
        toggleList.Clear();
        messageData = null;
        ClearEntryList();
    }
    protected override void ProcessEventsForEnable(bool toRegister)
    {
        Sys_MessageBag.Instance.eventEmitter.Handle(Sys_MessageBag.EEvents.OnMessageDateUpdate,OnPanelRefresh, toRegister);
    }
    #endregion

    #region Function

    public void InitCurrList()
    {
        currList.Clear();
        if (messageType == 0)
        {
            TeamTutorCombine();
        }
        else
        {
            CommonListInit();
        }
    }

    public void CommonListInit()
    {
        int listCount = messageData.GetCount();
        if (listCount == 0)
        {
            return;
        }
        if (listCount > 20)
        {
            listCount = 20;
        }
        for (int i = 0; i < listCount; i++)
        {
            Sys_MessageBag.MessageContent mContent = messageData.messageList[messageData.GetCount() - 1 - i];
            currList.Add(mContent);

        }
    }

    public void TeamTutorCombine()
    {
        messageData = Sys_MessageBag.Instance.GetMessageData(EMessageBagType.Tutor);
        int tutorCount = messageData.GetCount();
        if (tutorCount == 0)
        {
            messageData = Sys_MessageBag.Instance.GetMessageData(0);
            CommonListInit();
            return;
        }else if (tutorCount>=20)
        {
            tutorCount = 20;
            for (int i = 0; i < tutorCount; i++)
            {
                Sys_MessageBag.MessageContent mContent = messageData.messageList[messageData.GetCount() - 1 - i];
                currList.Add(mContent);

            }
            return;
        }else if (tutorCount < 20)
        {
            for (int i = 0; i < tutorCount; i++)
            {
                Sys_MessageBag.MessageContent mContent = messageData.messageList[messageData.GetCount() - 1 - i];
                currList.Add(mContent);

            }
            messageData = Sys_MessageBag.Instance.GetMessageData(0);
            int teamCount = messageData.GetCount();
            if (teamCount == 0)
            {
                return;
            }
            if (teamCount >= 20 - tutorCount)
            {
                teamCount = 20 - tutorCount;
            }
            for (int i = 0; i < teamCount; i++)
            {
                Sys_MessageBag.MessageContent mContent = messageData.messageList[messageData.GetCount() - 1 - i];
                currList.Add(mContent);

            }
        }

    }
    private void RefreshMessageBagRedPoint()
    {
        for (int i=0;i< toggleList.Count;i++)
        {
            bool isShow = false;
            if (i==0)
            {
                isShow = Sys_MessageBag.Instance.IsMessageBagRedPoint(0) || Sys_MessageBag.Instance.IsMessageBagRedPoint(3);
            }
            else if(i==1||i == 2)
            {
                isShow = Sys_MessageBag.Instance.IsMessageBagRedPoint(i);
            }
            else
            {
                isShow = Sys_MessageBag.Instance.IsMessageBagRedPoint(i+1);
            }
            toggleList[i].transform.Find("Image_Red").gameObject.SetActive(isShow);
        }
    }
    
    private void RefreshGroup()
    {
        ClearEntryList();
        //redPoint.RefreshAllRedPoints();
        RefreshMessageBagRedPoint();
        messageData = Sys_MessageBag.Instance.GetMessageData(messageType);
        InitCurrList();
        layout.messageGrid.CellCount = currList.Count;
        layout.messageGrid.ForceRefreshActiveCell();
    }

    private void OnToggleChange(int current, int old)
    {
        if ((int)messageType == current)
            return;

        messageType = (EMessageBagType)current;
        Sys_MessageBag.Instance.ClickType = (EMessageBagType)current;
        RefreshGroup();

    }

    private void OnCreateCell(InfinityGridCell cell)
    {
        UI_MessageBag_Ceil entry = new UI_MessageBag_Ceil();
        entry.BindGameObject(cell.mRootTransform.gameObject);
        entry.AddRefreshListener(OnRefreshGrid);
        cell.BindUserData(entry);
        

    }

    private void OnCellChange(InfinityGridCell cell, int index)
    {
        if (currList.Count==0)
        {
            return;
        }
        UI_MessageBag_Ceil entry = cell.mUserData as UI_MessageBag_Ceil;
        entry.SetDate(currList[index]);
        entryList.Add(entry);
        
    }
    public void OnBtnCloseClicked()
    {
        UIManager.CloseUI(EUIID.UI_MessageBag);

    }

    private void OnRefreshGrid(UI_MessageBag_Ceil msgCeil)
    {
        RefreshGroup();
    }

    private void OnPanelRefresh()
    {
        RefreshGroup();
    }
    public void ClearEntryList()
    {
        if (entryList.Count == 0)
        {
            return;
        }
        for (int i = 0; i < entryList.Count; i++)
        {
            entryList[i].OnDestroy();
        }
        entryList.Clear();
    }
    #endregion

}


