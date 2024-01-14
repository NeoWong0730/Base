using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Mainbattle_Function_Layout : UIComponent
    {
        public GameObject titleGo;
        public GameObject gridGo;
        public Button openBtn;
        public CP_TransformContainer familyResBattleHider;


        protected override void Loaded()
        {
            familyResBattleHider = transform.GetComponent<CP_TransformContainer>();
            titleGo = transform.Find("Animator/Open/View_Content/VerticalGrid/Image_Title01").gameObject;
            gridGo = transform.Find("Animator/Open/View_Content/VerticalGrid/Grid01").gameObject;
            openBtn = transform.Find("Animator/Open/Btn_Open").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            openBtn.onClick.AddListener(listener.OnopenBtnClicked);
        }

        public interface IListener
        {
            void OnopenBtnClicked();
        }
    }

    public class UI_Mainbattle_Function_Type : UIComponent
    {
        private GameObject buttonItem;
        private GameObject titleGo;
        private uint typeId; 

        private List<uint> menuIdList;

        public UI_Mainbattle_Function_Type(uint _typeId,GameObject _titleGo) : base()
        {
            typeId = _typeId;
            titleGo = _titleGo;
        }

        protected override void Loaded()
        {
            base.Loaded();
            buttonItem = transform.Find("Btn_Function01").gameObject;
            menuIdList = new List<uint>();
        }

        public void RefreshShow()
        {
            SetMenuList();
        }

        private void SetMenuList()
        {
            menuIdList.Clear();
            var dic = new List<CSVBattleMenuFunction.Data>(CSVBattleMenuFunction.Instance.GetAll());
            dic.OrderBy(x => x.OrderId);
            //var dic = CSVBattleMenuFunction.Instance.GetDictData().OrderBy(x => x.Value.OrderId).ToDictionary(x => x.Key, x => x.Value);
            foreach (var item in dic)
            {
                if (item.typeId == typeId&& Sys_FunctionOpen.Instance.IsOpen(item.functionId,false))
                {
                    if ((Sys_FamilyResBattle.Instance.InFamilyBattle&& item.ResourcebattleShow == 1)|| (!Sys_FamilyResBattle.Instance.InFamilyBattle&& item.isBattle == 1))
                    {
                        menuIdList.Add(item.id);
                    }                   
                }
            }
            if (menuIdList.Count == 0)
            {
                titleGo.SetActive(false);
                buttonItem.SetActive(false);
            }
            else
            {
                FrameworkTool.CreateChildList(buttonItem.transform.parent, menuIdList.Count);
                for (int i = 0; i < menuIdList.Count; ++i)
                {
                    GameObject go = buttonItem.transform.parent.GetChild(i).gameObject;
                    UI_FuctionItem menu = new UI_FuctionItem(menuIdList[i]);
                    uint functionId = CSVBattleMenuFunction.Instance.GetConfData(menuIdList[i]).functionId;       
                    go.transform.name = CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId.ToString();
                    menu.Init(go.transform);
                    menu.RefreshItem();
                }
            }
        }
    }

    public class UI_FuctionItem : UIComponent
    {
        private Text title;
        private Image icon;
        private Button btn;

        private uint menuId;

        public UI_FuctionItem(uint _menuId) : base()
        {
            menuId = _menuId;
        }

        protected override void Loaded()
        {
            base.Loaded();
            icon = transform.Find("Image").GetComponent<Image>();
            title = transform.Find("Text").GetComponent<Text>();
            btn = transform.GetComponent<Button>();
            btn.onClick.AddListener(OnBtnClicked);
        }

        public void RefreshItem()
        {
            uint functionId = CSVBattleMenuFunction.Instance.GetConfData(menuId).functionId;
            if ((EUIID)CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId == EUIID.UI_FirstCharge)
            {
                transform.gameObject.SetActive(Sys_OperationalActivity.Instance.CheckFirstChargeIsShow());
            }
            ImageHelper.SetIcon(icon, CSVBattleMenuFunction.Instance.GetConfData(menuId).iconId);
            title.text = LanguageHelper.GetTextContent(CSVBattleMenuFunction.Instance.GetConfData(menuId).lanId);
        }

        private void OnBtnClicked()
        {
            uint functionId= CSVBattleMenuFunction.Instance.GetConfData(menuId).functionId;
            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnFunctionMenuClose);
            UIManager.CloseUI(EUIID.UI_FunctionMenu);
            if ((EUIID)CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId == EUIID.UI_Mall)
            {
                UIManager.OpenUI(EUIID.UI_Mall, false, new MallPrama() { mallId = 101 });
            }
            else if ((EUIID)CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId == EUIID.UI_Team_Member)
            {
                UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);
            }
            else if ((EUIID)CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId == EUIID.UI_Family)
            {
                if (Sys_Family.Instance.familyData.isInFamily)
                {
                    UIManager.OpenUI((EUIID)CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId);
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_Family_FamilyList);
                }
            }
            else if ((EUIID)CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId == EUIID.UI_FirstCharge)
            {
                Sys_OperationalActivity.Instance.OpenFirstCharge();
            }
            else if((EUIID)CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId == EUIID.UI_WarriorGroup)
            {
                if (Sys_WarriorGroup.Instance.MyWarriorGroup.GroupUID == 0)
                {
                    //UIManager.OpenUI(EUIID.UI_DescWarriorGroup);
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_WarriorGroup);
                }
            }
            else if ((EUIID)CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId == EUIID.UI_Bag)
            {
                UIManager.OpenUI(EUIID.UI_Bag, false, 1);
            }
            else
            {
                UIManager.OpenUI((EUIID)CSVFunctionOpen.Instance.GetConfData(functionId).FunctionUiId);
            }
        }
    }

    public class UI_Mainbattle_Function : UIBase,UI_Mainbattle_Function_Layout.IListener
    {
        private UI_Mainbattle_Function_Layout layout = new UI_Mainbattle_Function_Layout();
        private UI_Mainbattle_RedPoint redPoint;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight)
            {
                UIManager.CloseUI(EUIID.UI_FunctionMenu, true);
            }
            else
            {
                AddTypeList();
            }
            redPoint = gameObject.AddComponent<UI_Mainbattle_RedPoint>();
            redPoint?.Init(this);

            HandleFamilyResBattleBtns();
        }

        protected override void OnHide()
        {            
            DefaultItem();
        }

        protected override void ProcessEvents(bool toRegister) {
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndEnter, OnEndEnter, toRegister);
        }

        private void OnEndEnter(uint _, int __) {
            HandleFamilyResBattleBtns();
        }

        private void HandleFamilyResBattleBtns() {
            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                layout.familyResBattleHider.ShowHideBySetActive(false);
            }
        }

        private void AddTypeList()
        {
            var dic = new List<CSVBattleMenuType.Data>(CSVBattleMenuType.Instance.GetAll());
            dic.OrderBy(x => x.orderNum);

            //var dic = CSVBattleMenuType.Instance.GetDictData().OrderBy(x => x.Value.orderNum).ToDictionary(x => x.Key, x => x.Value);
            foreach (var item in dic)
            {
                GameObject titlego = GameObject.Instantiate<GameObject>(layout.titleGo, layout.titleGo.transform.parent);
                GameObject gridgo = GameObject.Instantiate<GameObject>(layout.gridGo, layout.gridGo.transform.parent);
                Text typeName = titlego.transform.Find("Text").GetComponent<Text>();
                typeName.text = LanguageHelper.GetTextContent(CSVBattleMenuType.Instance.GetConfData(item.id).lanId);
                UI_Mainbattle_Function_Type type = new UI_Mainbattle_Function_Type(item.id, titlego);
                type.Init(gridgo.transform);
                type.RefreshShow();
            }
            layout.titleGo.SetActive(false);
            layout.gridGo.SetActive(false);
            ForceRebuildLayout(layout.titleGo.transform.parent.gameObject);
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

        private void DefaultItem()
        {
            layout.titleGo.SetActive(true);
            layout.gridGo.SetActive(true);
            FrameworkTool.DestroyChildren(layout.titleGo.transform.parent.gameObject, layout.titleGo.transform.name,layout.gridGo.transform.name);
        }
        #region ButtonClick

        public void OnopenBtnClicked()
        {
            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnFunctionMenuClose);
            UIManager.CloseUI(EUIID.UI_FunctionMenu);
        }
        #endregion
    }
}
