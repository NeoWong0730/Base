using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;

namespace Logic
{
    public struct ExpTable
    {
        public uint expId;
        public uint expValue;
        public uint type;
        public ExpTable(uint _id, uint _value,uint _type)
        {
            expId = _id;
            expValue = _value;
            type = _type;
        }

    }
    public class UI_ExpRetrieve_Ceil
    {
        private Button btn_go;
        private Button btn_get;
        private Text txt_Name;
        private GameObject go_Reward;
        private Text txt_Reward;
        private uint tableId;
        private CSVexpRetrieve.Data eData;
        private uint expValue;

        public void BindGameObject(Transform transform)
        {
            btn_go = transform.Find("Btn_02").GetComponent<Button>();
            btn_get = transform.Find("Btn_01").GetComponent<Button>();
            txt_Name = transform.Find("Image_bg/Text").GetComponent<Text>();
            go_Reward = transform.Find("Reward").gameObject;
            txt_Reward = transform.Find("Text_Reward").GetComponent<Text>();
            btn_go.onClick.AddListener(OnGoButtonClicked);
            btn_get.onClick.AddListener(OnGetButtonClicked);
        }
        public void SetData(ExpTable _exp)
        {
            tableId = _exp.expId;
            expValue = _exp.expValue;
            eData = CSVexpRetrieve.Instance.GetConfData(tableId);
            RetrieveName();
            ButtonSet();
            RetrieveTypeSet();
        }
        private void RetrieveName()
        {
            txt_Name.text = LanguageHelper.GetTextContent(eData.ActiveName);
        }

        private void ButtonSet()
        {
            btn_go.gameObject.SetActive(eData.Type == 2);
            btn_get.gameObject.SetActive(eData.Type == 1);
        }
        
        private void RetrieveTypeSet()
        {
            go_Reward.SetActive(eData.Type == 1);
            txt_Reward.gameObject.SetActive(eData.Type == 2);
            if (eData.Type == 1)
            {
                go_Reward.transform.Find("Text_Value").GetComponent<Text>().text = "+"+expValue;
            }else if (eData.Type == 2)
            {
                txt_Reward.text = LanguageHelper.GetTextContent(5405,((float)expValue/10000.0f).ToString()); 
            }
        }

        public void OnGoButtonClicked()
        {
            UIDailyActivitesParmas uiDaily = new UIDailyActivitesParmas();
            uiDaily.IsSkipDetail = true;
            uiDaily.SkipToID = eData.Activity_id;
            UIManager.CloseUI(EUIID.UI_OperationalActivity);
            UIManager.OpenUI(EUIID.UI_DailyActivites, false, uiDaily);
            UIManager.HitButton(EUIID.UI_OperationalActivity, "GoActivity_" + eData.Activity_id.ToString(), EOperationalActivity.ExpRetrieve.ToString());
        }
        public void OnGetButtonClicked()
        {
            Sys_OperationalActivity.Instance.OnGetCompensationExpReq(tableId);
            UIManager.HitButton(EUIID.UI_OperationalActivity, "GetExp_" + tableId.ToString(), EOperationalActivity.ExpRetrieve.ToString());
        }
    }
    public class UI_ExpRetrieve : UI_OperationalActivityBase
    {
        #region 界面显示
        private InfinityGrid PanelScrollGrid;
        List<ExpTable> expList = new List<ExpTable>();
        private Timer m_timer;
        #endregion
        #region 系统函数
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void Show()
        {
            base.Show();
            Sys_OperationalActivity.Instance.SetExpRetrieveFirstRedPoint();
            Sys_OperationalActivity.Instance.expRetriveRedPoint = false;
            OnExpRetrieve();
        }
        public override void Hide()
        {
            base.Hide();
            m_timer?.Cancel();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateExpRetrieveData, OnExpRetrieve, toRegister);
        }
        #endregion
        #region Function

        private void Parse()
        {
            PanelScrollGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            InitExpList();
            PanelScrollGrid.CellCount = expList.Count;
            PanelScrollGrid.onCreateCell += OnCreateCell;
            PanelScrollGrid.onCellChange += OnCellChange;
        }
        private void InitExpList()
        {
            expList.Clear();
            foreach (var item in Sys_OperationalActivity.Instance.expDic)
            {
                CSVexpRetrieve.Data cData = CSVexpRetrieve.Instance.GetConfData(item.Key);
                expList.Add(new ExpTable(item.Key,item.Value,cData.Type));
            }
            expList.Sort(
                delegate (ExpTable a, ExpTable b)
               {
                return a.type.CompareTo(b.type);
               }
                 );
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_ExpRetrieve_Ceil entry = new UI_ExpRetrieve_Ceil();
            entry.BindGameObject(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_ExpRetrieve_Ceil entry = cell.mUserData as UI_ExpRetrieve_Ceil;
            entry.SetData(expList[index]);
        }
        private int GetJumpIndex()
        {
            int _i = 0;
            for (int i=0;i<expList.Count;i++)
            {
                if (expList[i].expId==Sys_OperationalActivity.Instance.temporaryId)
                {
                    _i = i;
                    break;
                }
            }
            return _i;
        }
        private void OnExpRetrieve()
        {
            InitExpList();
            PanelScrollGrid.CellCount = expList.Count;
            PanelScrollGrid.ForceRefreshActiveCell();
            if (Sys_OperationalActivity.Instance.JumpIndex >=0)
            {
                m_timer?.Cancel();
                m_timer = Timer.Register(0.1f, OnTimeOver);
            }
            
        }

        private void OnTimeOver()
        {
          PanelScrollGrid.MoveToIndex(GetJumpIndex());
          Sys_OperationalActivity.Instance.JumpIndex = -1;

        }
        #endregion

    }

}
