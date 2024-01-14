using Lib.Core;
using Logic.Core;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;

namespace Logic
{
    public class UI_FamilyWorkshop : UIBase
    {
        public enum SortState
        {
            e_Intensify,
            e_Skill,
            e_Time,
        }

        private Text m_TitleName;
        private Button m_PublishButton;
        private Button m_IntensifyButton;
        private Button m_SkillButton;
        private Button m_RemainTimeButton;
        private Button m_CloseButton;
        private CP_ToggleRegistry m_CP_ToggleRegistry_Lable;
        private InfinityGrid m_InfinityGrid;
        private Dictionary<GameObject, ConsignGrid> m_ConsignGrids = new Dictionary<GameObject, ConsignGrid>();
        private int m_CurSelectConsignIndex;
        private int m_CurLable;
        private SortState m_CurSortState;
        private GameObject m_RedPoint;
        private GameObject m_ViewEmpty;

        private Button m_RuleButton;
        private Timer m_RuleShowTimer;

        protected override void OnInit()
        {
            m_CurLable = 1;
            m_CurSelectConsignIndex = 0;
            m_CurSortState = SortState.e_Time;
            Sys_Family.Instance.DefultSort();
            Sys_Family.Instance.DefultSelfConsignSort();
        }


        protected override void OnLoaded()
        {
            m_RedPoint = transform.Find("Animator/Image_BG1/Menu/ListItem1/Image_Red").gameObject;
            m_ViewEmpty = transform.Find("Animator/View_empty").gameObject;
            m_TitleName = transform.Find("Animator/Image_BG1/View_Activity/Title/Text_Name").GetComponent<Text>();
            m_PublishButton = transform.Find("Animator/Button").GetComponent<Button>();
            m_IntensifyButton = transform.Find("Animator/Image_BG1/View_Activity/Title/Text_Thing/Button").GetComponent<Button>();
            m_SkillButton = transform.Find("Animator/Image_BG1/View_Activity/Title/Text_Skill/Button").GetComponent<Button>();
            m_RemainTimeButton = transform.Find("Animator/Image_BG1/View_Activity/Title/Text_Time/Button").GetComponent<Button>();
            m_CloseButton = transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();
            m_CP_ToggleRegistry_Lable = transform.Find("Animator/Image_BG1/Menu").GetComponent<CP_ToggleRegistry>();
            m_InfinityGrid = transform.Find("Animator/Image_BG1/Scroll_Rank").GetComponent<InfinityGrid>();
            m_RuleButton = transform.Find("Animator/View_Title08/BtnHelp_665").GetComponent<Button>();
            m_CP_ToggleRegistry_Lable.onToggleChange = OnLableChanged;

            m_InfinityGrid.onCreateCell = OnCreateCell_Collect;
            m_InfinityGrid.onCellChange = OnCellChange_Collect;

            m_IntensifyButton.onClick.AddListener(OnIntensifyButtonClicked);
            m_SkillButton.onClick.AddListener(OnSkillButtonClicked);
            m_RemainTimeButton.onClick.AddListener(OnRemainTimeButtonClicked);
            m_PublishButton.onClick.AddListener(OnPublishButtonClicked);
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            m_RuleButton.onClick.AddListener(OnRuleButtonClicked);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle<int>(Sys_Family.EEvents.OnDeleConsignEntry, OnDeleConsignEntry, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnPublishSuccess, OnPublishSuccess, toRegister);
        }

        protected override void OnShow()
        {
            UpdateInfoUI();
            if (Sys_Family.Instance.consignFirstOpen == 0)
            {
                m_RuleShowTimer?.Cancel();
                m_RuleShowTimer = Timer.Register(0.5f, CheckCommonCource);
            }
        }


        private void CheckCommonCource()
        {
            Sys_CommonCourse.Instance.OpenCommonCourse(12, 1201, 120101);
            Sys_Family.Instance.GuildSetConsignFirstOpenReq();
        }


        private void OnRefreshRedPoint()
        {
            m_RedPoint.SetActive(Sys_Family.Instance.HasConsignReward());
        }

        private void OnPublishSuccess()
        {
            if (m_CurLable == 1)
            {
                if (m_CurSortState == SortState.e_Skill)
                {
                    Sys_Family.Instance.SkillSort();
                }
                else if (m_CurSortState == SortState.e_Intensify)
                {
                    Sys_Family.Instance.IntensifySort();
                }
                else
                {
                    Sys_Family.Instance.DefultSort();
                }
            }
            else if (m_CurLable == 2)
            {
                Sys_Family.Instance.DefultSelfConsignSort();
            }
            UpdateInfoUI();
        }

        private void UpdateInfoUI()
        {
            if (m_CurLable == 1)
            {
                m_InfinityGrid.CellCount = Sys_Family.Instance.consignInfos.Count;
                TextHelper.SetText(m_TitleName, 590002002);
            }
            else if (m_CurLable == 2)
            {
                m_InfinityGrid.CellCount = Sys_Family.Instance.consignSelfInfos.Count;
                TextHelper.SetText(m_TitleName, 590002031);
            }
            m_InfinityGrid.ForceRefreshActiveCell();
            OnRefreshRedPoint();
            m_ViewEmpty.SetActive(m_InfinityGrid.CellCount == 0);
        }

        private void OnRuleButtonClicked()
        {
            Sys_CommonCourse.Instance.OpenCommonCourse(12, 1201, 120101);            if (Sys_Family.Instance.consignFirstOpen == 0)
            {
                Sys_Family.Instance.GuildSetConsignFirstOpenReq();
            }        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_FamilyWorkshop);
        }

        private void OnPublishButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_FamilyWorkshop_entrust);
        }

        private void OnIntensifyButtonClicked()
        {
            if (m_CurLable == 2)
                return;
            if (m_CurSortState == SortState.e_Intensify)
                return;
            Sys_Family.Instance.IntensifySort();
            m_InfinityGrid.ForceRefreshActiveCell();
            m_CurSortState = SortState.e_Intensify;
        }

        private void OnSkillButtonClicked()
        {
            if (m_CurLable == 2)
                return;
            if (m_CurSortState == SortState.e_Skill)
                return;
            Sys_Family.Instance.SkillSort();
            m_InfinityGrid.ForceRefreshActiveCell();
            m_CurSortState = SortState.e_Skill;
        }

        private void OnRemainTimeButtonClicked()
        {
            if (m_CurLable == 2)
                return;
            if (m_CurSortState == SortState.e_Time)
                return;
            Sys_Family.Instance.DefultSort();
            m_InfinityGrid.ForceRefreshActiveCell();
            m_CurSortState = SortState.e_Time;
        }

        private void OnLableChanged(int current, int old)
        {
            if (current == old)
            {
                return;
            }
            m_CurLable = current;
            if (m_CurLable == 1)
            {
                Sys_Family.Instance.DefultSort();
            }
            if (m_CurLable == 2)
            {
                Sys_Family.Instance.DefultSelfConsignSort();
            }
            UpdateInfoUI();
            m_CurSelectConsignIndex = 0;
        }

        private void OnCreateCell_Collect(InfinityGridCell cell)
        {
            ConsignGrid consignGrid = new ConsignGrid();
            consignGrid.BindGameObject(cell.mRootTransform.gameObject);
            consignGrid.AddClickListener(OnCeilConsignSelected);
            cell.BindUserData(consignGrid);
            m_ConsignGrids.Add(cell.mRootTransform.gameObject, consignGrid);
        }

        private void OnCellChange_Collect(InfinityGridCell cell, int index)
        {
            ConsignGrid consignGrid = cell.mUserData as ConsignGrid;
            if (m_CurLable == 1)
            {
                consignGrid.SetData_1(Sys_Family.Instance.consignInfos[index], index);
            }
            else if (m_CurLable == 2)
            {
                consignGrid.SetData_2(Sys_Family.Instance.consignSelfInfos[index], index);
            }

            if (index != m_CurSelectConsignIndex)
            {
                consignGrid.Release();
            }
            else
            {
                consignGrid.Select();
            }
        }

        private void OnCeilConsignSelected(ConsignGrid consignGrid)
        {
            m_CurSelectConsignIndex = consignGrid.dataIndex;
            foreach (var item in m_ConsignGrids)
            {
                if (item.Value.dataIndex == m_CurSelectConsignIndex)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }
        }

        private void OnDeleConsignEntry(int lable)
        {
            if (lable != m_CurLable)
            {
                return;
            }
            if (lable == 1)
            {
                m_InfinityGrid.CellCount = Sys_Family.Instance.consignInfos.Count;
            }
            else if (lable == 2)
            {
                m_InfinityGrid.CellCount = Sys_Family.Instance.consignSelfInfos.Count;
            }
            m_InfinityGrid.ForceRefreshActiveCell();
            OnRefreshRedPoint();
        }

        public class ConsignGrid
        {
            private Transform m_Trans;
            private Image m_SelectImage;
            private GameObject m_SelectObj;
            private Text m_Consigner;
            private Text m_ItemName;
            private Text m_Intensify;
            private Text m_Skill;
            private Text m_RemainTime;//协助和求助显示的是订单剩余时间  领取显示的是销毁剩余时间
            private GameObject m_PropGo;
            private PropItem m_PropItem;
            private Action<ConsignGrid> onClick;
            public GuildConsignInfo guildConsignInfo;
            private GuildConsignSelfInfo m_GuildConsignSelfInfo;
            private Button m_Help;      //协助
            private Button m_Look;      //求助
            private Button m_Get;       //领取
            private GameObject m_RedPoint;
            public int dataIndex;
            private int m_Type;         //0:全部 1:自己
            public bool self;

            public void BindGameObject(GameObject go)
            {
                m_Trans = go.transform;

                m_PropGo = m_Trans.Find("ListItem").gameObject;
                m_SelectImage = m_Trans.Find("Image").GetComponent<Image>();
                m_SelectObj = m_Trans.Find("Select").gameObject;
                m_Consigner = m_Trans.Find("Text_Name").GetComponent<Text>();
                m_ItemName = m_Trans.Find("ListItem/Text_Name").GetComponent<Text>();
                m_Intensify = m_Trans.Find("Text_Thing").GetComponent<Text>();
                m_Skill = m_Trans.Find("Text_Skill").GetComponent<Text>();
                m_RemainTime = m_Trans.Find("Text_Time").GetComponent<Text>();
                m_Help = m_Trans.Find("Btn_Help").GetComponent<Button>();
                m_Look = m_Trans.Find("Btn_Look").GetComponent<Button>();
                m_Get = m_Trans.Find("Btn_Receive").GetComponent<Button>();
                m_RedPoint = m_Trans.Find("Btn_Receive/GameObject").gameObject;
                m_PropItem = new PropItem();
                m_PropItem.BindGameObject(m_PropGo);

                Lib.Core.EventTrigger eventTrigger = Lib.Core.EventTrigger.Get(m_SelectImage);
                eventTrigger.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
                m_Help.onClick.AddListener(OnHelpButtonClicked);
                m_Look.onClick.AddListener(OnLookButtonClicked);
                m_Get.onClick.AddListener(OnGetButtonClicked);
            }

            public void SetData_1(GuildConsignInfo _guildConsignInfo, int _dataIndex)
            {
                m_Type = 1;
                guildConsignInfo = _guildConsignInfo;
                m_GuildConsignSelfInfo = null;
                dataIndex = _dataIndex;
                RefreshEntry_1();
            }

            public void SetData_2(GuildConsignSelfInfo _guildConsignSelfInfo, int _dataIndex)
            {
                m_Type = 2;
                m_GuildConsignSelfInfo = _guildConsignSelfInfo;
                guildConsignInfo = null;
                dataIndex = _dataIndex;
                RefreshEntry_2();
            }

            private void OnHelpButtonClicked()
            {
                OpenFamilyConsignDetailParm openFamilyConsignDetailParm = new OpenFamilyConsignDetailParm();
                openFamilyConsignDetailParm.type = 1;
                openFamilyConsignDetailParm.guildConsignInfo = guildConsignInfo;
                openFamilyConsignDetailParm.name = guildConsignInfo.Name.ToStringUtf8();
                openFamilyConsignDetailParm.fromType = 1;
                UIManager.OpenUI(EUIID.UI_FamilyWorkshop_Detail, false, openFamilyConsignDetailParm);
            }

            private void OnLookButtonClicked()
            {
                OpenFamilyConsignDetailParm openFamilyConsignDetailParm = new OpenFamilyConsignDetailParm();
                openFamilyConsignDetailParm.type = 2;
                if (m_Type == 1)
                {
                    openFamilyConsignDetailParm.guildConsignInfo = guildConsignInfo;
                    openFamilyConsignDetailParm.name = guildConsignInfo.Name.ToStringUtf8();
                }
                else
                {
                    openFamilyConsignDetailParm.guildConsignSelfInfo = m_GuildConsignSelfInfo;
                    openFamilyConsignDetailParm.name = Sys_Role.Instance.Role.Name.ToStringUtf8();
                }
                openFamilyConsignDetailParm.fromType = m_Type;
                UIManager.OpenUI(EUIID.UI_FamilyWorkshop_Detail, false, openFamilyConsignDetailParm);
            }

            private void OnGetButtonClicked()
            {
                Sys_Family.Instance.ReceiveBuildItemReq(m_GuildConsignSelfInfo.UId);
            }

            private void RefreshEntry_1()
            {
                m_Get.gameObject.SetActive(false);
                self = guildConsignInfo.Name == Sys_Role.Instance.Role.Name;
                m_Help.gameObject.SetActive(!self);
                m_Look.gameObject.SetActive(self);
                TextHelper.SetText(m_Consigner, guildConsignInfo.Name.ToStringUtf8());
                CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(guildConsignInfo.FormulaId);
                if (null != cSVFormulaData)
                {
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(cSVFormulaData.view_item);
                    if (null != cSVItemData)
                    {
                        PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                               (_id: cSVFormulaData.view_item,
                               _count: 0,
                               _bUseQuailty: true,
                               _bBind: false,
                               _bNew: false,
                               _bUnLock: false,
                               _bSelected: false,
                               _bShowCount: false,
                               _bShowBagCount: false,
                               _bUseClick: false,
                               _onClick: null,
                               _bshowBtnNo: false);
                        m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_FamilyWorkshop, showItem));
                        TextHelper.SetText(m_ItemName, cSVItemData.name_id);
                    }

                    uint skillId = cSVFormulaData.type;
                    TextHelper.SetText(m_Skill, 2010143, LanguageHelper.GetTextContent(CSVLifeSkill.Instance.GetConfData(skillId).name_id), cSVFormulaData.level_skill.ToString());
                }
                if (guildConsignInfo.IntensifyBuild)
                {
                    TextHelper.SetText(m_Intensify, 590002024);//是
                }
                else
                {
                    TextHelper.SetText(m_Intensify, 590002025);//否
                }
                uint remainTime = 0;
                if (guildConsignInfo.EndTick >= Sys_Time.Instance.GetServerTime())
                {
                    remainTime = guildConsignInfo.EndTick - Sys_Time.Instance.GetServerTime();
                    //由于服务器1分钟计算一次有没有过期 可能会导致 刷新的时候 该值小于0，而出现显示错误,所以给个随机的(0,60)的数字表示就行
                    if (remainTime < 60)
                    {
                        remainTime = 1;
                    }
                }
                else
                {
                    remainTime = 1;
                }
                string timeToString = LanguageHelper.TimeToString(remainTime, LanguageHelper.TimeFormat.Type_6);
                TextHelper.SetText(m_RemainTime, timeToString);
                m_RedPoint.SetActive(false);
            }

            private void RefreshEntry_2()
            {
                uint remainTime = 0;
                if (m_GuildConsignSelfInfo.EndTick >= Sys_Time.Instance.GetServerTime())
                {
                    remainTime = m_GuildConsignSelfInfo.EndTick - Sys_Time.Instance.GetServerTime();
                    //由于服务器1分钟计算一次有没有过期 可能会导致 刷新的时候 该值小于0，而出现显示错误,所以给个随机的(0,60)的数字表示就行
                    if (remainTime < 60)
                    {
                        remainTime = 1;
                    }
                }
                else
                {
                    remainTime = 1;
                }
                TextHelper.SetText(m_RemainTime, LanguageHelper.TimeToString(remainTime, LanguageHelper.TimeFormat.Type_2));

                if (m_GuildConsignSelfInfo.HelperName.IsEmpty)//委托还没有被做
                {
                    TextHelper.SetText(m_Consigner, 590002012);//无
                    m_Help.gameObject.SetActive(false);
                    m_Look.gameObject.SetActive(true);
                    m_Get.gameObject.SetActive(false);
                }
                else
                {
                    TextHelper.SetText(m_Consigner, m_GuildConsignSelfInfo.HelperName.ToStringUtf8());
                    m_Help.gameObject.SetActive(false);
                    m_Look.gameObject.SetActive(false);
                    m_Get.gameObject.SetActive(true);
                }
                CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(m_GuildConsignSelfInfo.FormulaId);
                if (null != cSVFormulaData)
                {
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(cSVFormulaData.view_item);
                    if (null != cSVItemData)
                    {
                        PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                               (_id: cSVFormulaData.view_item,
                               _count: 0,
                               _bUseQuailty: true,
                               _bBind: false,
                               _bNew: false,
                               _bUnLock: false,
                               _bSelected: false,
                               _bShowCount: false,
                               _bShowBagCount: false,
                               _bUseClick: false,
                               _onClick: null,
                               _bshowBtnNo: false);
                        m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_FamilyWorkshop, showItem));
                        TextHelper.SetText(m_ItemName, cSVItemData.name_id);
                    }
                    uint skillId = cSVFormulaData.type;
                    TextHelper.SetText(m_Skill, 2010143, LanguageHelper.GetTextContent(CSVLifeSkill.Instance.GetConfData(skillId).name_id), cSVFormulaData.level_skill.ToString());
                }

                if (m_GuildConsignSelfInfo.IntensifyBuild)
                {
                    TextHelper.SetText(m_Intensify, "是");
                }
                else
                {
                    TextHelper.SetText(m_Intensify, "否");
                }
                m_RedPoint.SetActive(!m_GuildConsignSelfInfo.HelperName.IsEmpty);
            }

            public void AddClickListener(Action<ConsignGrid> _onClick)
            {
                onClick = _onClick;
            }

            private void OnGridClicked(BaseEventData baseEventData)
            {
                onClick.Invoke(this);
            }

            public void Release()
            {
                m_SelectObj.gameObject.SetActive(false);
            }

            public void Select()
            {
                m_SelectObj.gameObject.SetActive(true);
            }
        }
    }
}


