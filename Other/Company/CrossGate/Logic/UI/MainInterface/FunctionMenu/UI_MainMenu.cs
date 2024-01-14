using UnityEngine;
using System.Collections;
using Logic.Core;
using System.Collections.Generic;
using Lib.Core;
using Table;
using System.Linq;
using UnityEngine.UI;
using System;

namespace Logic
{
    public class UI_MainMenu : UIBase
    {
        GameObject go_Black;
        public GameObject Grid01;
        int m_visualGridCount;
        RectTransform Image_Bg;
        Dictionary<uint, Action> functionRedPoint;
        List<UI_MainMenuItem> menuItemList;

        protected override void OnLoaded()
        {
            Grid01 = transform.Find("Animator/Open/Image_Bg/VerticalGrid/Grid01").gameObject;
            Image_Bg= transform.Find("Animator/Open/Image_Bg").GetComponent<RectTransform>();
            go_Black = transform.Find("Black").gameObject;
            go_Black.SetActive(false);
            functionRedPoint = new Dictionary<uint, Action>();
            menuItemList = new List<UI_MainMenuItem>();
            //Image_Bg.pivot = new Vector2(1, 0);
        }

        protected override void OnShow()
        {
            Sys_Input.Instance.bIsOpenMainMenu = true;
            ShowMenuList();
        }

        protected override void OnHide()
        {
            Sys_Input.Instance.bIsOpenMainMenu = false;
        }

        protected override void ProcessEvents(bool toRegister)
        {
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, OnBeginEnter, toRegister);
            Sys_Input.Instance.eventEmitter.Handle(Sys_Input.EEvents.OnCloseMainMenu, OnCloseMainMenu, toRegister);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnFunctionOpen, toRegister);
            Sys_MainMenu.Instance.eventEmitter.Handle<Sys_MainMenu.Enum_RedPointUI>(Sys_MainMenu.EEvents.OnRefreshFunctionRedPoint, OnRefreshFunctionRedPoint, toRegister);
        }
        private void OnRefreshFunctionRedPoint(Sys_MainMenu.Enum_RedPointUI eUi)
        {
            if(functionRedPoint.TryGetValue((uint)eUi, out Action value))
                value?.Invoke();
        }

        private void OnFunctionOpen(Sys_FunctionOpen.FunctionOpenData obj)
        {
            for (int i = 0; i < menuItemList.Count; i++)
            {
                menuItemList[i].RefreshItem();
            }
            ForceRebuildLayout(gameObject);
        }

        private void OnCloseMainMenu()
        {
            UIManager.CloseUI(EUIID.UI_MainMenu);
        }

        private void OnBeginEnter(uint arg1, int nID)
        {
            if (nID != (int)EUIID.UI_MainMenu && nID != (int)EUIID.UI_ForceGuide && nID != (int)EUIID.UI_UnForceGuide)
            {
                UIManager.CloseUI(EUIID.UI_MainMenu);
            }
        }

        public void ShowMenuList()
        {
            functionRedPoint.Clear();
            menuItemList.Clear();
            List<CSVBattleMenuFunction.Data> dic = (List<CSVBattleMenuFunction.Data>)CSVBattleMenuFunction.Instance.GetAll();
            var dataList = dic.FindAll(x => x.isMain == 1);
            dataList.Sort((x, y) => x.mainOrder.CompareTo(y.mainOrder));
            m_visualGridCount = dataList.Count;
            FrameworkTool.CreateChildList(Grid01.transform, m_visualGridCount);
            for (int i = 0; i < m_visualGridCount; i++)
            {
                CSVBattleMenuFunction.Data csvData = dataList[i];
                Transform tran = Grid01.transform.GetChild(i);
                tran.gameObject.name = csvData.id.ToString();
                UI_MainMenuItem ceil = AddComponent<UI_MainMenuItem>(tran);
                ceil?.SetData(csvData);
                functionRedPoint.Add(csvData.id, ceil.RefreshRedPoint);
                menuItemList.Add(ceil);
            }
        }

        private void ForceRebuildLayout(GameObject go)
        {
            ContentSizeFitter[] fitter = go.GetComponentsInChildren<ContentSizeFitter>();
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
        }
    }
    public class UI_MainMenuItem : UIComponent
    {
        private GameObject m_Tips;
        private GameObject go_RedPoint;
        private Text text_TitleName;
        private Text text_TipNum;
        private Image image_Icon;
        private Button btn_Item;

        CSVBattleMenuFunction.Data data;
        protected override void Loaded()
        {
            base.Loaded();
            image_Icon = transform.Find("Image").GetComponent<Image>();
            text_TitleName = transform.Find("Text").GetComponent<Text>();
            btn_Item = transform.GetComponent<Button>();
            m_Tips = transform.Find("Tips").gameObject;
            go_RedPoint = transform.Find("Image_Dot").gameObject;
            text_TipNum = transform.Find("Tips/Text").GetComponent<Text>();
            btn_Item.onClick.AddListener(OnBtnClicked);
        }

        public override void SetData(params object[] arg)
        {
            base.SetData();
            if (arg != null && arg.Length > 0)
            {
                data = (CSVBattleMenuFunction.Data)arg[0];
                RefreshItem();
            }
        }

        public override void Show()
        {
            base.Show();
            if (!Sys_MainMenu.Instance.list_ShowFunction.Contains(data.id))
                Sys_MainMenu.Instance.list_ShowFunction.Add(data.id);
        }
        public override void Hide()
        {
            base.Hide();
            if (Sys_MainMenu.Instance.list_ShowFunction.Contains(data.id))
                Sys_MainMenu.Instance.list_ShowFunction.Remove(data.id);
        }

        public void RefreshItem()
        {
            m_Tips.SetActive(false);
            ImageHelper.SetIcon(image_Icon, CSVBattleMenuFunction.Instance.GetConfData(data.id).iconId);
            text_TitleName.text = LanguageHelper.GetTextContent(CSVBattleMenuFunction.Instance.GetConfData(data.id).lanId);
            bool IsFamilyBattle = Sys_FamilyResBattle.Instance.InFamilyBattle;
            bool functionOpen = Sys_FunctionOpen.Instance.IsOpen(data.functionId, false);
            
            bool toShow = functionOpen;
            toShow &= (!IsFamilyBattle || (data.ResourceMainShow == 1));
            
            ShowHide(toShow);
            if (gameObject.activeSelf){
                RefreshRedPoint();
            }              
        }

        private void OnBtnClicked()
        {
            uint functionId = CSVBattleMenuFunction.Instance.GetConfData(this.data.id).functionId;
            //UIManager.CloseUI(EUIID.UI_MainMenu);
            var functionOpenData = CSVFunctionOpen.Instance.GetConfData(functionId);
            if (functionOpenData.FunctionUiId == (uint)EUIID.UI_WarriorGroup)
            {
                if (Sys_WarriorGroup.Instance.MyWarriorGroup.GroupUID == 0)
                {
                    UIManager.OpenUI(EUIID.UI_DescWarriorGroup);
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_WarriorGroup);
                }
            }
            else
            {
                UIManager.OpenUI((EUIID)functionOpenData.FunctionUiId);
                if ((EUIID)CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId == EUIID.UI_Adventure)
                {
                    UIManager.OpenUI(EUIID.UI_Adventure);
                    Sys_Adventure.Instance.ReportClickEventHitPoint("Main_Open");
                }
            }
        }

        public void RefreshRedPoint()
        {
            Sys_MainMenu.Enum_RedPointUI eRedPoint = (Sys_MainMenu.Enum_RedPointUI)data.id;
            switch (eRedPoint)
            {
                case Sys_MainMenu.Enum_RedPointUI.Adventure:
                    {
                        int num = Sys_MainMenu.FunctionRedPoint_AdventureNum.Value;
                        if (m_Tips != null)
                        {
                            if (num > 0)
                            {
                                m_Tips.SetActive(true);
                                text_TipNum.text = num.ToString();
                            }
                            else
                            {
                                m_Tips.SetActive(false);
                            }
                        }
                        SetRedPointActive(false);
                    }
                    break;
                default:
                    SetRedPointActive(Sys_MainMenu.Instance.FunctionRedPoint(eRedPoint));
                    break;
            }
        }

        private void SetRedPointActive(bool active)
        {
            go_RedPoint.SetActive(active);
        }
    }
}