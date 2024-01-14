using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;
using System;
using Packet;

namespace Logic
{
    public class UI_Plan_Layout
    {
        public class PartnerAttrItem
        {
            public AttributeRow attributeRow;
            public uint infoID;

            GameObject gameObject;
            Text name;
            Text value;

            public PartnerAttrItem(GameObject _gameObject)
            {
                gameObject = _gameObject;
                gameObject.SetActive(true);

                name = gameObject.FindChildByName("name").GetComponent<Text>();
                value = gameObject.FindChildByName("value").GetComponent<Text>();
            }

            public void Refresh(AttributeRow _attributeRow, uint _infoID)
            {
                attributeRow = _attributeRow;
                infoID = _infoID;

                CSVAttr.Data data = CSVAttr.Instance.GetConfData(attributeRow.Id);
                TextHelper.SetText(name, LanguageHelper.GetTextContent(data.name));
                TextHelper.SetText(value, (attributeRow.Value + Sys_Partner.Instance.GetChangeValue(infoID, attributeRow.Id, attributeRow.Value)).ToString());
            }
        }

        public class PartnerSkillItem
        {
            public uint skillID;
            public uint active;
            GameObject gameObject;
            Image icon;
            Image select;
            Toggle toggle;

            public PartnerSkillItem(GameObject _gameObject)
            {
                gameObject = _gameObject;
                gameObject.SetActive(true);

                icon = gameObject.FindChildByName("Image_Icon").GetComponent<Image>();
                select = gameObject.FindChildByName("Image_Select").GetComponent<Image>();
                select.gameObject.SetActive(false);
                toggle = gameObject.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(OnValueChanged);
            }

            public void Refresh(uint _skillID, uint _active)
            {
                skillID = _skillID;
                active = _active;
                ImageHelper.SetIcon(icon, CSVPassiveSkillInfo.Instance.GetConfData(skillID).icon);
                ImageHelper.SetImageGray(icon, active == 0);
            }

            void OnValueChanged(bool isOn)
            {
                if (isOn)
                {
                    UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillID, 0));
                }
            }
        }

        public class PlanItemCommon
        {
            public Sys_Plan.Plan plan;
            public Action<Sys_Plan.Plan> action;

            GameObject gameObject;
            Text title;
            Text current;
            public CP_Toggle toggle;

            public PlanItemCommon(GameObject _gameObject)
            {
                gameObject = _gameObject;            
                gameObject.SetActive(true);

                title = gameObject.FindChildByName("Text").GetComponent<Text>();
                current = gameObject.FindChildByName("Text1").GetComponent<Text>();
                toggle = gameObject.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
            }

            public void Refresh(Sys_Plan.Plan _plan, Action<Sys_Plan.Plan> _action = null)
            {
                plan = _plan;
                if (_action != null)
                    action = _action;

                TextHelper.SetText(title, Sys_Plan.Plan.GetUITitleStr(plan.PlanType));
                TextHelper.SetText(current, plan.Name);

                gameObject.name = $"Item_{(uint)plan.PlanType}";
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    action?.Invoke(plan);
                }
            }
        }

        public class PlanItemPartner
        {
            public class Icon
            {
                public Packet.Partner partnerInfo;
                public Action<Packet.Partner> action;

                GameObject gameObject;

                public Image icon;
                public Toggle toggle;

                public Icon(GameObject _gameObject)
                {
                    gameObject = _gameObject;
                    gameObject.SetActive(true);

                    icon = gameObject.FindChildByName("Image_Icon").GetComponent<Image>();
                    toggle = gameObject.GetComponent<Toggle>();
                    toggle.onValueChanged.AddListener(OnClickToggle);
                }

                public void Refresh(Packet.Partner _partnerInfo, Action<Packet.Partner> _action)
                {
                    partnerInfo = _partnerInfo;
                    action = _action;

                    ImageHelper.SetIcon(icon, CSVPartner.Instance.GetConfData(partnerInfo.InfoId).battle_headID);
                }

                void OnClickToggle(bool isOn)
                {
                    if (isOn)
                    {
                        action?.Invoke(partnerInfo);
                    }
                }
            }

            public Sys_Plan.Plan plan;
            public Action<Sys_Plan.Plan> action;

            GameObject gameObject;
            Text title;
            public CP_Toggle toggle;
            GameObject iconRoot;
            GameObject iconPrefab;

            public PlanItemPartner(GameObject _gameObject)
            {
                gameObject = _gameObject;
                gameObject.SetActive(true);

                title = gameObject.FindChildByName("Text").GetComponent<Text>();
                toggle = gameObject.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
                iconRoot = gameObject.FindChildByName("Grid");
                iconPrefab = gameObject.FindChildByName("PartnerItem01");
            }

            public void Refresh(Sys_Plan.Plan _plan, Action<Sys_Plan.Plan> _action = null)
            {
                plan = _plan;
                if (_action != null)
                    action = _action;

                TextHelper.SetText(title, Sys_Plan.Plan.GetUITitleStr(plan.PlanType));
                CreatePartnerIcons(_plan);
                gameObject.name = $"Item_{(uint)plan.PlanType}";
            }

            void CreatePartnerIcons(Sys_Plan.Plan _plan)
            {
                iconRoot.gameObject.DestoryAllChildren();

                var list = Sys_Partner.Instance.GetFmList()[(int)Sys_Partner.Instance.GetCurFmList()];
                foreach (uint infoID in list.Pa)
                {
                    var partnerInfo =  Sys_Partner.Instance.GetPartnerInfo(infoID);
                    if (partnerInfo != null)
                    {
                        GameObject obj = GameObject.Instantiate(iconPrefab);
                        Icon icon = new Icon(obj);
                        icon.Refresh(partnerInfo, (info) =>
                        {
                            Sys_Plan.Instance.eventEmitter.Trigger<uint>(Sys_Plan.EEvents.OnClickPartner, infoID);
                        });

                        obj.transform.SetParent(iconRoot.transform, false);
                    }
                }
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    action?.Invoke(plan);
                }
            }
        }

        public class CommonPlanChooseItem
        {
            public Sys_Plan.Plan plan;
            public Action<Sys_Plan.Plan> action;

            GameObject gameObject;
            Text name;
            public CP_Toggle toggle;

            public CommonPlanChooseItem(GameObject _gameObject)
            {
                gameObject = _gameObject;
                gameObject.SetActive(true);

                name = gameObject.FindChildByName("Text").GetComponent<Text>();
                toggle = gameObject.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
            }

            public void Refresh(Sys_Plan.Plan _plan, Action<Sys_Plan.Plan> _action)
            {
                plan = _plan;
                action = _action;
                gameObject.name = $"toggle_{plan.Index}";

                TextHelper.SetText(name, plan.Name);
                if (plan.Index == UI_Plan.currentChooseIndex)
                {
                    toggle.SetSelected(true, false);
                }
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    action?.Invoke(plan);
                }
            }
        }

        public class PartnerPlanChooseItem
        {
            public class EmptyIcon
            {
                GameObject gameObject;
                GameObject iconImage;
                GameObject blankImage;
                GameObject addImage;

                public EmptyIcon(GameObject _gameObject)
                {
                    gameObject = _gameObject;
                    gameObject.SetActive(true);

                    addImage = gameObject.FindChildByName("Image_Add");
                    addImage.SetActive(true);
                    blankImage = gameObject.FindChildByName("Image_Blank");
                    blankImage.SetActive(true);
                    iconImage = gameObject.FindChildByName("Image_Icon");
                    iconImage.SetActive(false);
                }
            }

            public class Icon
            {
                public Packet.Partner partnerInfo;
                public Action<Packet.Partner> action;

                GameObject gameObject;
                public Image icon;
                public Toggle toggle;

                public Icon(GameObject _gameObject)
                {
                    gameObject = _gameObject;
                    gameObject.SetActive(true);

                    icon = gameObject.FindChildByName("Image_Icon").GetComponent<Image>();
                    toggle = gameObject.GetComponent<Toggle>();
                    toggle.onValueChanged.AddListener(OnClickToggle);
                }

                void OnClickToggle(bool isOn)
                {
                    if (isOn)
                    {
                        action?.Invoke(partnerInfo);
                    }
                }

                public void Refresh(Packet.Partner _partnerInfo, Action<Packet.Partner> _action)
                {
                    partnerInfo = _partnerInfo;
                    action = _action;

                    ImageHelper.SetIcon(icon, CSVPartner.Instance.GetConfData(partnerInfo.InfoId).battle_headID);
                }               
            }

            public Sys_Plan.Plan plan;
            public Action<Sys_Plan.Plan> action;

            GameObject gameObject;
            Text name;
            public CP_Toggle toggle;
            GameObject iconRoot;
            GameObject iconPrefab;

            public PartnerPlanChooseItem(GameObject _gameObject)
            {
                gameObject = _gameObject;
                gameObject.SetActive(true);

                name = gameObject.FindChildByName("Text").GetComponent<Text>();
                toggle = gameObject.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
                iconRoot = gameObject.FindChildByName("Grid");
                iconPrefab = gameObject.FindChildByName("PartnerItem01");
            }

            public void Refresh(Sys_Plan.Plan _plan, Action<Sys_Plan.Plan> _action)
            {
                plan = _plan;
                action = _action;
                gameObject.name = $"toggle_{plan.Index}";

                TextHelper.SetText(name, plan.Name);
                if (plan.Index == UI_Plan.currentChooseIndex)
                {
                    toggle.SetSelected(true, false);
                }

                CreatePartnerIcons(_plan);
            }

            void CreatePartnerIcons(Sys_Plan.Plan _plan)
            {
                iconRoot.gameObject.DestoryAllChildren();

                var list = Sys_Partner.Instance.GetFmList()[(int)_plan.Index];
                foreach (uint infoID in list.Pa)
                {
                    var partnerInfo = Sys_Partner.Instance.GetPartnerInfo(infoID);
                    if (partnerInfo != null)
                    {
                        GameObject obj = GameObject.Instantiate(iconPrefab);
                        Icon icon = new Icon(obj);
                        icon.Refresh(partnerInfo, (info) =>
                        {
                            Sys_Plan.Instance.eventEmitter.Trigger<uint>(Sys_Plan.EEvents.OnClickPartner, infoID);
                        });

                        obj.transform.SetParent(iconRoot.transform, false);
                    }
                    else
                    {
                        GameObject obj = GameObject.Instantiate(iconPrefab);
                        EmptyIcon emptyIcon = new EmptyIcon(obj);
                        obj.transform.SetParent(iconRoot.transform, false);
                    }
                }               
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    action?.Invoke(plan);
                }
            }
        }

        public Transform transform;
        public Button closeButton;

        public GameObject planItemCommonObj;
        public GameObject PlanItemPartnerObj;
        public GameObject commonPlanChooseItemObj;
        public GameObject partnerPlanChooseItemObj;
        public GameObject itemRoot;
        public GameObject commonViewRoot;
        public GameObject partnerViewRoot;
        public GameObject commonPlanChooseItemRoot;
        public GameObject partnerPlanChooseItemRoot;
        public Button setCommonPlanButton;
        public Button setPartnerPlanButton;

        public GameObject partnerInfoRoot;
        public GameObject partnerAttrRoot;
        public GameObject partnerSkillRoot;
        public GameObject partnerAttrItemObj;
        public GameObject partnerSkillItemObj;
        public Button partnerCloseButton;

        public void Init(Transform transform)
        {
            this.transform = transform;

            planItemCommonObj = transform.gameObject.FindChildByName("Item1");
            PlanItemPartnerObj = transform.gameObject.FindChildByName("Item2");
            itemRoot = transform.gameObject.FindChildByName("TabList");

            commonViewRoot = transform.gameObject.FindChildByName("View_Right");
            partnerViewRoot = transform.gameObject.FindChildByName("View_Partner");

            commonPlanChooseItemRoot = commonViewRoot.gameObject.FindChildByName("Toggle_Choice");
            commonPlanChooseItemObj = commonViewRoot.gameObject.FindChildByName("toggle1");

            partnerPlanChooseItemRoot = partnerViewRoot.gameObject.FindChildByName("Toggle_Choice");
            partnerPlanChooseItemObj = partnerViewRoot.gameObject.FindChildByName("toggle1");

            setCommonPlanButton = commonViewRoot.FindChildByName("Btn_03").GetComponent<Button>();
            setPartnerPlanButton = partnerViewRoot.FindChildByName("Btn_03").GetComponent<Button>();
            closeButton = transform.gameObject.FindChildByName("Image_Close").GetComponent<Button>();

            partnerInfoRoot = transform.gameObject.FindChildByName("View_PartnerInfo");
            partnerAttrRoot = partnerInfoRoot.FindChildByName("AttrGrid");
            partnerSkillRoot = partnerInfoRoot.FindChildByName("SkillGrid");
            partnerAttrItemObj = partnerInfoRoot.FindChildByName("AttrItem");
            partnerSkillItemObj = partnerInfoRoot.FindChildByName("SkillItem");
            partnerCloseButton = partnerInfoRoot.FindChildByName("Image_Close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);

            setCommonPlanButton.onClick.AddListener(listener.OnClickSetCommonPlanButton);
            setPartnerPlanButton.onClick.AddListener(listener.OnClickSetPartnerPlanButton);
            partnerCloseButton.onClick.AddListener(listener.OnClickPartnerCloseButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickSetCommonPlanButton();

            void OnClickSetPartnerPlanButton();

            void OnClickPartnerCloseButton();
        }
    }

    public class UI_Plan : UIBase, UI_Plan_Layout.IListener
    {
        UI_Plan_Layout layout = new UI_Plan_Layout();

        /// <summary>
        /// 当前选中的方案类型///
        /// </summary>
        public static Sys_Plan.EPlanType currentChoosePlanType;

        public static Dictionary<Sys_Plan.EPlanType, UI_Plan_Layout.PlanItemCommon> commonPlanItems = new Dictionary<Sys_Plan.EPlanType, UI_Plan_Layout.PlanItemCommon>();
        public static UI_Plan_Layout.PlanItemPartner partnerPlanItem;

        /// <summary>
        /// 当前选中的方案序号///
        /// </summary>
        public static uint currentChooseIndex;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Plan.Instance.eventEmitter.Handle<uint, uint>(Sys_Plan.EEvents.OnChangePlanSuccess, OnChangePlanSuccess, toRegister);
            Sys_Plan.Instance.eventEmitter.Handle<ulong, uint, uint>(Sys_Plan.EEvents.OnChangeFightPet, OnChangeFightPet, toRegister);
            Sys_Plan.Instance.eventEmitter.Handle<uint, uint>(Sys_Plan.EEvents.AddNewPlan, OnAddNewPlan, toRegister);
            Sys_Plan.Instance.eventEmitter.Handle<uint, uint, string>(Sys_Plan.EEvents.ChangePlanName, OnChangePlanName, toRegister);
            Sys_Plan.Instance.eventEmitter.Handle<uint>(Sys_Plan.EEvents.OnClickPartner, OnClickPartner, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdateLevel, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, toRegister);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnFormRefreshNotification, OnFormRefreshNotification, toRegister);
        }

        void OnFormRefreshNotification()
        {
            RefreshPlanDatas();
        }

        void OnClickPartner(uint partnerID)
        {
            layout.partnerInfoRoot.SetActive(true);

            var partnerInfo = Sys_Partner.Instance.GetPartnerInfo(partnerID);
            List<AttributeRow> attrs = Sys_Partner.Instance.GetPartnerAttr(partnerInfo.InfoId, partnerInfo.Level, EAttryType.Basic);
            layout.partnerAttrRoot.DestoryAllChildren();
            for (int index = 0, len = attrs.Count; index < len; index++)
            {
                GameObject itemObj = GameObject.Instantiate(layout.partnerAttrItemObj);
                UI_Plan_Layout.PartnerAttrItem item = new UI_Plan_Layout.PartnerAttrItem(itemObj);
                item.Refresh(attrs[index], partnerID);

                itemObj.transform.SetParent(layout.partnerAttrRoot.transform, false);
            }

            layout.partnerSkillRoot.DestoryAllChildren();

            var datas = Sys_Partner.Instance.GetRuneSkillsActive(partnerInfo.InfoId);
            for (int index = 0, len = datas.Count; index < len; index++)
            {
                GameObject itemObj = GameObject.Instantiate(layout.partnerSkillItemObj);
                UI_Plan_Layout.PartnerSkillItem item = new UI_Plan_Layout.PartnerSkillItem(itemObj);
                item.Refresh(datas[index][0], datas[index][1]);

                itemObj.transform.SetParent(layout.partnerSkillRoot.transform, false);
            }
        }

        void OnChangePlanSuccess(uint planType, uint index)
        {
            InfoRefresh(planType, index);
        }

        void OnChangeFightPet(ulong petID, uint petAttributeIndex, uint PetAttributeCorrectIndex)
        {
            if (petID != 0)
            {
                InfoRefresh((uint)Sys_Plan.EPlanType.PetAttribute, petAttributeIndex);
                InfoRefresh((uint)Sys_Plan.EPlanType.PetAttributeCorrect, PetAttributeCorrectIndex);
            }
            else
            {
                RefreshPlanDatas();
            }
        }

        void OnAddNewPlan(uint planType, uint index)
        {     
            InfoRefresh(planType, Sys_Plan.Instance.curPlanIndexs[planType]);
        }

        void OnChangePlanName(uint planType, uint index, string name)
        {
            InfoRefresh(planType, Sys_Plan.Instance.curPlanIndexs[planType]);
        }

        void InfoRefresh(uint planType, uint index)
        {
            if (planType == (uint)Sys_Plan.EPlanType.Partner)
            {
                partnerPlanItem.Refresh(Sys_Plan.Instance.allPlans[planType][index]);
                partnerPlanItem.toggle.SetSelected(true, false);
                partnerPlanItem.toggle.onValueChanged.Invoke(true);
            }
            else
            {
                commonPlanItems[(Sys_Plan.EPlanType)planType].Refresh(Sys_Plan.Instance.allPlans[planType][index]);
                if ((uint)currentChoosePlanType == planType)
                {
                    commonPlanItems[(Sys_Plan.EPlanType)planType].toggle.SetSelected(true, false);
                    commonPlanItems[(Sys_Plan.EPlanType)planType].toggle.onValueChanged.Invoke(true);
                }
            }
        }

        protected override void OnShow()
        {
            RefreshPlanDatas();
        }

        protected override void OnHide()
        {
            //currentChoosePlanType = Sys_Plan.EPlanType.None;
            //currentChooseIndex = 0;
        }

        void RefreshPlanDatas()
        {
            var strs = CSVParam.Instance.GetConfData(1560).str_value.Split('|');
            layout.itemRoot.DestoryAllChildren(null, true);

            commonPlanItems.Clear();
            partnerPlanItem = null;

            CreateCommonPlanItem(Sys_Plan.EPlanType.RoleAttribute, uint.Parse(strs[3]));
            CreateCommonPlanItem(Sys_Plan.EPlanType.Family, uint.Parse(strs[0]));
            CreateCommonPlanItem(Sys_Plan.EPlanType.Talent, uint.Parse(strs[4]));
            CreateCommonPlanItem(Sys_Plan.EPlanType.PetAttribute, uint.Parse(strs[1]));
            if (CSVCheckseq.Instance.GetConfData(10561).IsValid())
            {
                CreateCommonPlanItem(Sys_Plan.EPlanType.PetAttributeCorrect, uint.Parse(strs[2]));
            }

            CreatePartnerPlanItem(uint.Parse(strs[5]));
        }

        void CreateCommonPlanItem(Sys_Plan.EPlanType planType, uint level)
        {
            if (Sys_Role.Instance.Role.Level < level)
                return;

            if (Sys_Plan.Instance.allPlans.ContainsKey((uint)planType))
            {
                GameObject itemObj = GameObject.Instantiate(layout.planItemCommonObj);
                UI_Plan_Layout.PlanItemCommon item = new UI_Plan_Layout.PlanItemCommon(itemObj);
                item.Refresh(Sys_Plan.Instance.allPlans[(uint)planType][Sys_Plan.Instance.curPlanIndexs[(uint)planType]], (plan) =>
                {
                    layout.commonViewRoot.SetActive(true);
                    layout.partnerViewRoot.SetActive(false);

                    currentChoosePlanType = planType;
                    currentChooseIndex = plan.Index;
                    CreateCommonPlanChooseItems(planType);                   
                });
                itemObj.transform.SetParent(layout.itemRoot.transform, false);

                commonPlanItems[planType] = item;
            }
        }

        void CreateCommonPlanChooseItems(Sys_Plan.EPlanType planType)
        {
            layout.commonPlanChooseItemRoot.DestoryAllChildren();

            foreach (Sys_Plan.Plan planData in Sys_Plan.Instance.allPlans[(uint)planType].Values)
            {
                GameObject itemObj = GameObject.Instantiate(layout.commonPlanChooseItemObj);
                UI_Plan_Layout.CommonPlanChooseItem item = new UI_Plan_Layout.CommonPlanChooseItem(itemObj);
                item.Refresh(planData, (plan) =>
                {
                    currentChooseIndex = plan.Index;
                    Sys_Plan.Instance.ChangePlan(plan.PlanType, plan.Index);
                });
                
                itemObj.transform.SetParent(layout.commonPlanChooseItemRoot.transform, false);
            }
        }

        void CreatePartnerPlanItem(uint level)
        {
            if (Sys_Role.Instance.Role.Level < level)
                return;

            if (Sys_Plan.Instance.allPlans.ContainsKey((uint)Sys_Plan.EPlanType.Partner))
            {
                if (Sys_Plan.Instance.allPlans[(uint)Sys_Plan.EPlanType.Partner].Count > 0)
                {
                    GameObject itemObj = GameObject.Instantiate(layout.PlanItemPartnerObj);
                    UI_Plan_Layout.PlanItemPartner item = new UI_Plan_Layout.PlanItemPartner(itemObj);
                    item.Refresh(Sys_Plan.Instance.allPlans[(uint)Sys_Plan.EPlanType.Partner][Sys_Plan.Instance.curPlanIndexs[(uint)Sys_Plan.EPlanType.Partner]], (plan) =>
                    {
                        layout.commonViewRoot.SetActive(false);
                        layout.partnerViewRoot.SetActive(true);

                        currentChoosePlanType = Sys_Plan.EPlanType.Partner;
                        currentChooseIndex = plan.Index;
                        CreatePartnerPlanChooseItems();
                    });
                    itemObj.transform.SetParent(layout.itemRoot.transform, false);

                    partnerPlanItem = item;
                }
            }
        }

        void CreatePartnerPlanChooseItems()
        {
            layout.partnerPlanChooseItemRoot.DestoryAllChildren();

            foreach (Sys_Plan.Plan planData in Sys_Plan.Instance.allPlans[(uint)Sys_Plan.EPlanType.Partner].Values)
            {
                GameObject itemObj = GameObject.Instantiate(layout.partnerPlanChooseItemObj);
                UI_Plan_Layout.PartnerPlanChooseItem item = new UI_Plan_Layout.PartnerPlanChooseItem(itemObj);
                item.Refresh(planData, (plan) =>
                {
                    currentChooseIndex = plan.Index;
                    Sys_Plan.Instance.ChangePlan(plan.PlanType, plan.Index);
                });

                itemObj.transform.SetParent(layout.partnerPlanChooseItemRoot.transform, false);
            }
        }

        /// <summary>
        /// 点击了关闭按钮///
        /// </summary>
        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_Plan);
        }

        public void OnClickSetCommonPlanButton()
        {
            if (currentChoosePlanType == Sys_Plan.EPlanType.Family)
            {
                UIManager.OpenUI(EUIID.UI_Family_Empowerment);
            }
            else if (currentChoosePlanType == Sys_Plan.EPlanType.RoleAttribute)
            {
                UIManager.OpenUI(EUIID.UI_Attribute, false, 2);
            }
            else if (currentChoosePlanType == Sys_Plan.EPlanType.PetAttributeCorrect)
            {
                uint uiid = 3;
                MessageEx practiceEx = new MessageEx();
                practiceEx.messageState = (EPetMessageViewState)uiid;
                practiceEx.subPage = 1;
                Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
            }
            else if (currentChoosePlanType == Sys_Plan.EPlanType.PetAttribute)
            {
                UIManager.OpenUI(EUIID.UI_Pet_AddPoint, false, Sys_Pet.Instance.GetFightPetClient(Sys_Pet.Instance.fightPet.GetUid()));
            }
            else if (currentChoosePlanType == Sys_Plan.EPlanType.Talent)
            {
                UIManager.OpenUI(EUIID.UI_SkillUpgrade, false, new List<int> { 1 });
            }
        }

        public void OnClickSetPartnerPlanButton()
        {
            var partnerUIParam = new PartnerUIParam();
            partnerUIParam.tabIndex = 1;
            UIManager.OpenUI(EUIID.UI_Partner, false, partnerUIParam);
        }

        public void OnClickPartnerCloseButton()
        {
            layout.partnerInfoRoot.SetActive(false);
        }

        void OnUpdateLevel()
        {
            RefreshPlanDatas();
        }

        void OnTaskStatusChanged(TaskEntry taskEntry, ETaskState oldState, ETaskState newState)
        {
            RefreshPlanDatas();
        }
    }
}