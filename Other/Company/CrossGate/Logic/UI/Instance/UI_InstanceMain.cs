using Logic;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Table;
using UnityEngine;
using UnityEngine.UI;

/// <summary> 副本菜单界面 </summary>
public class UI_InstanceMain : UIComponent {
    #region 界面组件

    /// <summary> 任务名 </summary>
    private Text text_Name;

    /// <summary> 内容 </summary>
    private Text text_Message;

    /// <summary> 特效资源 </summary>
    private GameObject go_Effect;
    //private Button btnUI;

    private Text text_TitleNum;

    private Text m_TexBtnLeave;

    #endregion

    #region 数据

    /// <summary> 任务 </summary>
    private TaskEntry taskEntry;

    #endregion

    #region 系统函数

    protected override void Loaded() {
        base.Loaded();
        ProcessEventsForAwake(true);
        OnParseComponent();
    }

    public override void OnDestroy() {
        base.OnDestroy();
    }

    protected override void ProcessEventsForAwake(bool toRegister) {
        Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnReceived, OnReceived, toRegister);
        Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnSubmited, OnSubmited, toRegister);
        Sys_Task.Instance.eventEmitter.Handle(Sys_Task.EEvents.OnRefreshAll, OnRefreshAll, toRegister);
        Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnRefreshed, OnRefreshed, toRegister);
        Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnFinished, OnFinished, toRegister);
        Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnTraced, OnRefreshed, toRegister);
    }

    #endregion

    #region 初始化

    /// <summary>
    /// 检测组件 
    /// </summary>
    private void OnParseComponent() {
        text_Name = transform.Find("OnedungeonsScroll/Text_Title").GetComponent<Text>();
        text_Message = transform.Find("OnedungeonsScroll/Text_Message1").GetComponent<Text>();
        go_Effect = transform.Find("OnedungeonsScroll/Fx_ui_Select_02").gameObject;
        go_Effect.gameObject.SetActive(false);
        transform.Find("OnedungeonsScroll/Button_Leave").GetComponent<Button>().onClick.AddListener(OnClick_Exit);

        m_TexBtnLeave = transform.Find("OnedungeonsScroll/Button_Leave/Text").GetComponent<Text>();
        //btnUI = transform.Find("OnedungeonsScroll/Button_UI").GetComponent<Button>();
        //btnUI.onClick.AddListener(OnClick_UI);
        transform.Find("OnedungeonsScroll/Image").GetComponent<Button>().onClick.AddListener(OnBtnTaskClicked);

        text_TitleNum = transform.Find("OnedungeonsScroll/Text_TitleNum").GetComponent<Text>();
    }

    #endregion

    #region 界面显示

    /// <summary>
    /// 刷新界面
    /// </summary>
    public void RefreshView() {
        //this.btnUI.gameObject.SetActive(false);

        uint InstanceId = Sys_Instance.Instance.curInstance.InstanceId;
        List<TaskEntry> tasks = Sys_Task.Instance.GetDungonReceivedTask(InstanceId);

       

        RefreshInstanceStage();

        if (tasks.Count > 0) {
            taskEntry = tasks[0];

            TextHelper.SetTaskText(text_Name, taskEntry.csvTask.taskName, taskEntry.id.ToString());
            bool isFinish = taskEntry.IsFinish();
            if (isFinish) {
                uint submitNpcId = taskEntry.csvTask.submitNpc;
                string mapName = string.Empty;
                string npcName = string.Empty;
                bool hasSubmitNpc = submitNpcId != 0;
                CSVNpc.Data npcData = CSVNpc.Instance.GetConfData(submitNpcId);
                if (npcData != null) {
                    npcName = LanguageHelper.GetNpcTextContent(npcData.name);

                    uint mapID = npcData.mapId;
                    CSVMapInfo.Data csvMap = CSVMapInfo.Instance.GetConfData(mapID);
                    if (csvMap != null) {
                        mapName = LanguageHelper.GetTextContent(csvMap.name);
                    }
                }

                if (hasSubmitNpc) {
                    TextHelper.SetTaskText(text_Message, 1601000001, mapName, npcName);
                }
                else {
                    TextHelper.SetTaskText(text_Message, 1601000002);
                }
            }
            else {
                if (!taskEntry.csvTask.conditionType) {
                    text_Message.text = taskEntry.currentTaskContent;
                }
                else {
                    TextHelper.SetTaskText(text_Message, taskEntry.csvTask.taskContent[0], (taskEntry.TotalProgress * 100).ToString(), "100");
                }
            }

            //this.btnUI.gameObject.SetActive(InstanceId == Sys_HundredPeopleArea.Instance.activityid);
        }
        else {
            taskEntry = null;
            TextHelper.SetText(text_Name, 1006038);

            bool isSpecial = Sys_HundredPeopleArea.Instance.IsInstance;
            if (isSpecial) {
                TextHelper.SetText(text_Message, 1006198);
            }
            else {
                TextHelper.SetText(text_Message, 1006039);
            }
        }
    }

    private void RefreshInstanceStage() {

       

        uint InstanceId = Sys_Instance.Instance.curInstance.InstanceId;

        var instanceiddata = CSVInstance.Instance.GetConfData(InstanceId);

        if (instanceiddata != null && instanceiddata.Type == 7u)
        {
            text_TitleNum.text = string.Empty;

            TextHelper.SetText(m_TexBtnLeave, 14032u);
        }
        else
        {
            var stagelist = Sys_Instance.Instance.getDailyByInstanceID(InstanceId);

            var listcount = stagelist.Count;

            int realcount = 0;
            uint lastid = 0;
            for (int i = 0; i < listcount; i++)
            {
                if (stagelist[i].Layerlevel != lastid)
                {
                    realcount += 1;
                    lastid = stagelist[i].Layerlevel;
                }
            }

            var data = CSVInstanceDaily.Instance.GetConfData(Sys_Instance.Instance.curInstance.StageID);

            var stagetLevel = data == null ? 0 : (data.LayerStage - 1) * 10 + data.Layerlevel;

            text_TitleNum.text = stagetLevel.ToString() + "/" + realcount.ToString();

            TextHelper.SetText(m_TexBtnLeave, 14058u);
        }


    }

    #endregion

    #region 响应事件

    /// <summary>
    /// 离开副本
    /// </summary>
    public void OnClick_Exit() {
        if (Sys_Instance.Instance.isManyDungeons) {
            uint langueID = Sys_Team.Instance.isCaptain() ? (uint) 2009629 : (uint) 2009632;

            OpenDialog(LanguageHelper.GetTextContent(langueID));

            return;
        }

        Sys_Instance.Instance.InstanceExitReq();
    }

    //确认离开
    private void OnLeave() {
        UIManager.CloseUI(EUIID.UI_PromptBox, false);


        Sys_Instance.Instance.InstanceExitReq();
    }

    //取消
    private void OnCancle() {
        UIManager.CloseUI(EUIID.UI_PromptBox, false);
    }

    private void OpenDialog(string langue) {
        PromptBoxParameter.Instance.Clear();

        PromptBoxParameter.Instance.SetConfirm(true, OnLeave, 2002121);

        PromptBoxParameter.Instance.SetCancel(true, OnCancle, 2009626);

        PromptBoxParameter.Instance.content = langue;

        UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
    }

    /// <summary>
    /// 点击背景板调用
    /// </summary>
    private void OnBtnTaskClicked() {
        if (Sys_Instance.Instance.IsInInstance) {
            bool isSpecial = false;
            CSVInstance.Data cSVInstanceData = CSVInstance.Instance.GetConfData(Sys_Instance.Instance.curInstance.InstanceId);
            if (null != cSVInstanceData) {
                switch ((EInstanceType) cSVInstanceData.Type) {
                    case EInstanceType.HundreadPeolle:
                        isSpecial = true;
                        break;
                }
            }

            if (!isSpecial) {
                Sys_Task.Instance.TryDoTask(taskEntry, true, false, true);
            }
            else {
                // 百人道场
                if (taskEntry == null || (Sys_HundredPeopleArea.Instance.passedInstanceId == Sys_Instance.Instance.curInstance.StageID)) {
                    UIManager.OpenUI(EUIID.UI_Hundred_Result, false, Sys_HundredPeopleArea.Instance.passedInstanceId);
                }
                else {
                    Sys_Task.Instance.TryDoTask(taskEntry, true, false, true);
                }
            }

            go_Effect.gameObject.SetActive(false);
            go_Effect.gameObject.SetActive(true);
        }
    }

    private void OnClick_UI() {
        UIManager.OpenUI(EUIID.UI_HundredPeopleArea, false);
    }

    private void OnRefreshAll() {
        RefreshView();
    }

    private void OnReceived(int menuId, uint id, TaskEntry taskEntry) {
        if (menuId == (int) ETaskCategory.Dungeon) {
            RefreshView();
        }
    }

    private void OnSubmited(int menuId, uint id, TaskEntry taskEntry) {
        OnRefreshed(menuId, id, taskEntry);
    }

    private void OnFinished(int menuId, uint id, TaskEntry taskEntry) {
        OnRefreshed(menuId, id, taskEntry);
    }

    private void OnRefreshed(int menuId, uint id, TaskEntry taskEntry) {
        if (menuId == (int) ETaskCategory.Dungeon) {
            if (null == this.taskEntry || this.taskEntry.id == id) {
                RefreshView();
            }
        }
    }

    #endregion
}