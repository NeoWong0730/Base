
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System.Linq;
using Packet;
using System;

namespace Logic
{
    public class UI_Family_Empowermen_Layout
    {
        public class PlanItem
        {
            GameObject gameObject;

            public CP_Toggle toggle;
            Text darkText;
            Text lightText;
            Button button;

            int index;
            Sys_Experience.ExperiencePlanData planData;
            Action<int> choose;

            public PlanItem(GameObject _gameObject, Sys_Experience.ExperiencePlanData _planData, int _index, Action<int> _choose)
            {
                gameObject = _gameObject;
                planData = _planData;
                index = _index;
                choose = _choose;

                gameObject.SetActive(true);
                toggle = gameObject.GetComponent<CP_Toggle>();
                darkText = gameObject.FindChildByName("Text_Menu_Dark").GetComponent<Text>();
                lightText = gameObject.FindChildByName("Text_Menu_Light").GetComponent<Text>();
                button = gameObject.FindChildByName("Button_Rename").GetComponent<Button>();
                toggle.onValueChanged.AddListener(OnClickToggle);
                button.onClick.AddListener(OnClickButton);

                SetName(planData.Name);
            }

            public void SetName(string name)
            {
                TextHelper.SetText(darkText, name);
                TextHelper.SetText(lightText, name);
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    choose.Invoke(index);
                }
            }

            void OnClickButton()
            {
                void OnRename(int schIndex, int __, string newName)
                {
                    Sys_Experience.Instance.ReqPlanRename(newName, (uint)schIndex);
                }

                var arg = new UI_ChangeSchemeName.ChangeNameArgs()
                {
                    arg1 = index,
                    arg2 = 0,
                    oldName = Sys_Experience.Instance.experiencePlanDatas[index].Name,
                    onYes = OnRename
                };
                UIManager.OpenUI(EUIID.UI_ChangeSchemeName, true, arg);
            }
        }

        public class PlanAdd
        {
            GameObject gameObject;

            Button button;

            public PlanAdd(GameObject _gameObject)
            {
                gameObject = _gameObject;

                gameObject.SetActive(true);
                button = gameObject.GetComponent<Button>();
                button.onClick.AddListener(OnClickButton);
            }

            void OnClickButton()
            {
                if (!CSVCheckseq.Instance.GetConfData(12105).IsValid())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013901));
                    return;
                }

                bool valid = (Sys_Ini.Instance.Get<IniElement_IntArray>(1432, out var rlt) && rlt.value.Length >= 3);
                int limit = rlt.value[1];
                if (Sys_Experience.Instance.experiencePlanDatas.Count >= limit)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013702));
                    return;
                }
                void OnConform()
                {
                    if (Sys_Bag.Instance.GetItemCount(2) < rlt.value[2])
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5602));
                    }
                    else
                    {
                        Sys_Experience.Instance.ReqAddNewPlan();
                    }
                }
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10013807, rlt.value[2].ToString());
                PromptBoxParameter.Instance.SetConfirm(true, OnConform);
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);

                
            }
        }

        public Transform transform;
        public Text level;
        public Text usedPoint;
        public Text leftPoint;
        public Button useBtn;
        public Button resetBtn;
        public Button closeBtn;
        public Button addPointBtn;
        public Button cancelBtn;
        public Button confirmBtn;
        public Button helpBtn;
        public Button changePlanButton;
        public GameObject changePlanUnSelect;
        public GameObject changePlanSelect;

        public GameObject tipsGo;
        public GameObject attrGird;
        public GameObject attrItemGo;
        public GameObject lockGo;

        public GameObject planView;
        public GameObject planRoot;
        public GameObject planItemPrefab;
        public GameObject planAddPrefab;

        public void Init(Transform transform)
        {
            this.transform = transform;
            level = transform.Find("Animator/Text_Level/Value").GetComponent<Text>();
            usedPoint = transform.Find("Animator/Image_Used/Text_Point").GetComponent<Text>();
            leftPoint = transform.Find("Animator/Image_Remain/Text_Point").GetComponent<Text>();
            closeBtn = transform.Find("View_TipsBg01_Largest/Btn_Close").GetComponent<Button>();
            useBtn = transform.Find("Animator/Btn_Use").GetComponent<Button>();
            resetBtn = transform.Find("Animator/Image_Used/Button_Reset").GetComponent<Button>();
            addPointBtn = transform.Find("Animator/BtnGroup/Btn_Add").GetComponent<Button>();
            cancelBtn = transform.Find("Animator/BtnGroup/Btn_Cancel").GetComponent<Button>();
            confirmBtn = transform.Find("Animator/BtnGroup/Btn_Confirm").GetComponent<Button>();
            helpBtn = transform.Find("Animator/Btn_Help").GetComponent<Button>();
            changePlanButton = transform.gameObject.FindChildByName("Btn_ChangePlan").GetComponent<Button>();
            changePlanUnSelect = changePlanButton.gameObject.FindChildByName("Text_UnSelect");
            changePlanSelect = changePlanButton.gameObject.FindChildByName("Text_Select");

            tipsGo = transform.Find("Animator/View_attr/Scroll_View/Viewport/Tips").gameObject;
            attrGird = transform.Find("Animator/View_attr/Scroll_View/Viewport/Attr_Grid").gameObject;
            attrItemGo = transform.Find("Animator/View_attr/Scroll_View/Viewport/Attr_Grid/Attr01").gameObject;
            lockGo = transform.Find("Animator/View_attr/Scroll_View/Viewport/View_title").gameObject;

            planView = transform.gameObject.FindChildByName("View_Lable");
            planRoot = planView.FindChildByName("TabList");
            planItemPrefab = planView.FindChildByName("TabItem01");
            planAddPrefab = planView.FindChildByName("Button_Add");
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnCloseBtnClicked);
            useBtn.onClick.AddListener(listener.OnUseBtnClicked);
            resetBtn.onClick.AddListener(listener.OnResetBtnClicked);
            addPointBtn.onClick.AddListener(listener.OnAddPointBtnClicked);
            cancelBtn.onClick.AddListener(listener.OnCancelBtnClicked);
            confirmBtn.onClick.AddListener(listener.OnConfirmBtnClicked);
            helpBtn.onClick.AddListener(listener.OnHelpBtnClicked);
            changePlanButton.onClick.AddListener(listener.OnClickChangePlanButton);
        }

        public interface IListener
        {
            void OnAddPointBtnClicked();
            void OnCancelBtnClicked();
            void OnCloseBtnClicked();
            void OnConfirmBtnClicked();
            void OnHelpBtnClicked();
            void OnResetBtnClicked();
            void OnUseBtnClicked();

            void OnClickChangePlanButton();
        }
    }

    public class UI_Family_Empowerment_Attr : UIComponent
    {
        private uint Id;
        private uint attrId;
        private uint addnumPre;
        public  uint rank;
        public uint addnum;
        public uint perLevelAddAttr;
        private ExperienceInfo info;
        private CSVExperienceAttr.Data csvAttrData;

        private Text attrName;
        private Text number;
        private Text precent;
        private Button addBtn;
        private Button subBtn;
        private GameObject pointGo;
        private Image attrBg;
        private GameObject pointLight;
        private GameObject fx_up;

        public UI_Family_Empowerment_Attr(uint _attrId,uint _id,uint _rank) : base()
        {
            Id = _id;
            attrId = _attrId;
            rank = _rank;
        }

        protected override void Loaded()
        {
            attrName = transform.Find("Name").GetComponent<Text>();
            number = transform.Find("Text_Bg/Number").GetComponent<Text>();
            precent = transform.Find("Text_Num").GetComponent<Text>();
            addBtn = transform.Find("Btn_Add").GetComponent<Button>();
            addBtn.onClick.AddListener(OnAddBtnClicked);
            subBtn = transform.Find("Btn_Min").GetComponent<Button>();
            subBtn.onClick.AddListener(OnSubBtnClicked);
            pointGo = transform.Find("PointGroup/Point_Dark").gameObject;
            attrBg = transform.Find("Text_Bg").GetComponent<Image>();
            pointLight = transform.Find("PointGroup/Point_Dark/Point_Light").gameObject;
            fx_up = transform.Find("Fx_UI_Family_01").gameObject;
        }

        private void OnSubBtnClicked()
        {
            uint temprank = 0;
            Sys_Experience.Instance.rankAddpoints.Clear();
            foreach (var item in Sys_Experience.Instance.experiencePlanDatas[UI_Family_Empowerment.currentChooseIndex].infoDic)
            {
                if (Sys_Experience.Instance.index2Rank[item.Key] != temprank)
                {
                    temprank = Sys_Experience.Instance.index2Rank[item.Key];
                    if (GetRankAddNum(temprank) != 0)
                    {
                        Sys_Experience.Instance.rankAddpoints.Add(temprank, GetRankAddNum(temprank));
                        break;
                    }
                }
            }
            foreach (var item in Sys_Experience.Instance.rankAddpoints)
            {
                uint maxAddRank = GetMaxAddRank();
                if (rank!=maxAddRank&& GetTotalAddNum()-GetRankAddNum(maxAddRank) + Sys_Experience.Instance.experiencePlanDatas[UI_Family_Empowerment.currentChooseIndex].UsePoint == CSVExperienceAttrRank.Instance.GetConfData(maxAddRank).need_point)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5604));
                    return;
                }

            }
            if (addnum > 0)
            {
                foreach (var item in Sys_Experience.Instance.experiencePlanDatas[UI_Family_Empowerment.currentChooseIndex].infoDic)
                {
                    uint nowrank = Sys_Experience.Instance.index2Rank[item.Key];
                    if ((GetTotalAddNum() + Sys_Experience.Instance.experiencePlanDatas[UI_Family_Empowerment.currentChooseIndex].UsePoint) == CSVExperienceAttrRank.Instance.GetConfData(nowrank).need_point)
                    {
                        Sys_Experience.Instance.eventEmitter.Trigger(Sys_Experience.EEvents.OnAddRankInPreAdd, nowrank, false);
                    }
                }
                addnum--;
                UpdateAddNum(false);
                if (addnum == 0)
                {
                    subBtn.gameObject.SetActive(false);
                }
            }
        }

        private void OnAddBtnClicked()
        {
            uint leftpoint = Sys_Experience.Instance.experiencePlanDatas[UI_Family_Empowerment.currentChooseIndex].LeftPoint;
            if (addnum < leftpoint && GetTotalAddNum() < leftpoint)
            {
                addnum++;
                subBtn.gameObject.SetActive(true);
                UpdateAddNum(true);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent( 5603));
            }
            foreach(var item in Sys_Experience.Instance.experiencePlanDatas[UI_Family_Empowerment.currentChooseIndex].infoDic)
            {
                uint nowrank = Sys_Experience.Instance.index2Rank[item.Key];
                if ((GetTotalAddNum() + Sys_Experience.Instance.experiencePlanDatas[UI_Family_Empowerment.currentChooseIndex].UsePoint) == CSVExperienceAttrRank.Instance.GetConfData(nowrank).need_point)
                {
                    Sys_Experience.Instance.eventEmitter.Trigger(Sys_Experience.EEvents.OnAddRankInPreAdd, nowrank, true);
                }
            }

        }

        private void UpdateAddNum(bool isAdd)
        {
            uint curTotalAddPoint = info.AddPoint + addnum;
            if (addnum == 0)
            {
                number.text = "<color=#56422E>" + info.AddPoint.ToString() + "</color> ";
            }
            else
            {
                number.text = "<color=#57C03F>" + curTotalAddPoint.ToString() + "</color> ";
            }
            if (Sys_Experience.Instance.addinfoDic.ContainsKey(Id))
            {
                CmdExperienceAttrAddReq.Types.AttrAddInfo addinfo = new CmdExperienceAttrAddReq.Types.AttrAddInfo();
                addinfo.IndexId = Id;
                addinfo.Point = addnum;
                Sys_Experience.Instance.addinfoDic[Id] = addinfo;
            }
            else
            {
                CmdExperienceAttrAddReq.Types.AttrAddInfo addinfo = new CmdExperienceAttrAddReq.Types.AttrAddInfo();
                addinfo.IndexId = Id;
                addinfo.Point = addnum;
                Sys_Experience.Instance.addinfoDic.Add(Id, addinfo);
            }
            //预加点时开启下一层级
            if (curTotalAddPoint >= csvAttrData.double_cost_level)
            {
                if (pointGo.transform.parent.childCount == 1)
                {
                    FrameworkTool.CreateChildList(pointGo.transform.parent, 2);
                    pointGo.transform.parent.GetChild(1).name = "1";
                }
                uint preLevel = 0;
                preLevel = (curTotalAddPoint - csvAttrData.double_cost_level) / 2 + csvAttrData.double_cost_level;
                
                uint lightCount = curTotalAddPoint % 2;
                if (lightCount == 0)
                {
                    pointLight.SetActive(false);
                    if (isAdd)
                    {
                        fx_up.SetActive(false);
                        fx_up.SetActive(true);
                    }
                }
                else
                {
                    pointLight.SetActive(true);
                }
                if (preLevel == info.Level)
                {
                    precent.text = "<color=#56422E>" + addnumPre.ToString() + "%</color> ";
                }
                else
                {
                    precent.text = "<color=#57C03F>" + (preLevel * perLevelAddAttr).ToString() + "%</color> ";
                }
                if (preLevel == csvAttrData.max_level)
                {
                    addBtn.gameObject.SetActive(false);
                }
                else
                {
                    addBtn.gameObject.SetActive(true);
                }
            }
            else
            {
                FrameworkTool.DestroyChildren(pointGo.transform.parent.gameObject, pointGo.transform.name);
                if (curTotalAddPoint == 0)
                {
                    precent.text = "<color=#56422E>" + addnumPre.ToString() + "%</color> ";
                }
                else
                {
                    precent.text = "<color=#57C03F>" + (perLevelAddAttr * curTotalAddPoint).ToString() + "%</color>";
                }
                if (isAdd)
                {
                    fx_up.SetActive(false);
                    fx_up.SetActive(true);
                }
            }
            Sys_Experience.Instance.eventEmitter.Trigger(Sys_Experience.EEvents.OnUpdateLeftPoint, addnum);
        }

        private uint GetTotalAddNum()
        {
            uint totalAddpoint = 0;
            foreach (var v in Sys_Experience.Instance.addinfoDic)
            {
                totalAddpoint += v.Value.Point;
            }
            return totalAddpoint;
        }

        private uint GetRankAddNum(uint rank)
        {
            uint num = 0;
            foreach (var v in Sys_Experience.Instance.addinfoDic)
            {

                if (Sys_Experience.Instance.index2Rank[v.Key] == rank)
                {
                    num += v.Value.Point;
                }
            }
            return num;
        }

        private uint GetMaxAddRank()
        {
            uint maxrank = 0;
            foreach (var item in Sys_Experience.Instance.rankAddpoints)
            {
                if (item.Key > maxrank&&item.Value!=0)
                {
                    maxrank = item.Key;
                }
            }
            return maxrank;
        }

        public void RefreshItem(bool isLocked)
        {
            info = Sys_Experience.Instance.experiencePlanDatas[UI_Family_Empowerment.currentChooseIndex].infoDic[Id];
            csvAttrData = CSVExperienceAttr.Instance.GetConfData(Id);
            attrName.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(attrId).name);
            perLevelAddAttr = csvAttrData.add_value / 100;
            addnumPre = info.Level * perLevelAddAttr;
            ImageHelper.SetImageGray(attrBg, isLocked);
            SetAddState(false, isLocked);
        }

        public void SetAddState(bool isAdd,bool isLocked)
        {
            addnum = 0;
            if (isAdd)
            {
                if (isLocked)
                return;
                number.text = info.AddPoint.ToString();
            }
            else
            {
                number.text = addnumPre.ToString() + "%" + "(" + info.AddPoint.ToString() + ")";
            }
            if (info.Level < csvAttrData.max_level)
            {
                addBtn.gameObject.SetActive(isAdd);
            }
            else
            {
                addBtn.gameObject.SetActive(false);
            }
            subBtn.gameObject.SetActive(false);
            pointGo.transform.parent.gameObject.SetActive(isAdd);
            precent.gameObject.SetActive(isAdd);
            precent.text = "<color=#56422E>" + addnumPre.ToString() + "%</color> ";
            if (info.Level >= csvAttrData.double_cost_level)
            {
                pointLight.SetActive(false);
                FrameworkTool.CreateChildList(pointGo.transform.parent, 2);
                pointGo.transform.parent.GetChild(1).name = "1";
                if (info.AddPoint % 2 == 0)
                {
                    pointLight.SetActive(false);
                }
                else
                {
                    pointLight.SetActive(true);
                }
            }
            else
            {
                FrameworkTool.DestroyChildren(pointGo.transform.parent.gameObject, pointGo.transform.name);
                pointLight.SetActive(false);
            }

        }

        public bool IsLocked(uint currank)
        {
            if(Sys_Experience.Instance.experiencePlanDatas[UI_Family_Empowerment.currentChooseIndex] .UsePoint< CSVExperienceAttrRank.Instance.GetConfData(currank).need_point)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }   

    public class UI_Family_Empowerment : UIBase, UI_Family_Empowermen_Layout.IListener
    {
        private UI_Family_Empowermen_Layout layout = new UI_Family_Empowermen_Layout();
        private Dictionary<uint, int> rankAttrDic = new Dictionary<uint, int>();
        private Dictionary<uint, GameObject> rankGoDic = new Dictionary<uint, GameObject>();
        private List<UI_Family_Empowerment_Attr> attrList = new List< UI_Family_Empowerment_Attr>();

        List<UI_Family_Empowermen_Layout.PlanItem> planItems = new List<UI_Family_Empowermen_Layout.PlanItem>();
        public static int currentChooseIndex;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            UpdatePlans();
            currentChooseIndex = Sys_Experience.Instance.currentIndex;
            planItems[currentChooseIndex].toggle.SetSelected(true, true);
            UpdateViewInfo();
            UpdateUseButton();
        }

        void UpdateViewInfo()
        {
            SetValue();
            SetRankDic();
            SetAttrGrids();
            AddList();
            ForceRebuildLayout(layout.attrGird.transform.parent.gameObject);
            AddPointShow(false);
        }

        protected override void OnHide()
        {
            DefaultAttr();
            AddPointShow(false);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Experience.Instance.eventEmitter.Handle<uint>(Sys_Experience.EEvents.OnUpdateLeftPoint, OnUpdateLeftPoint, toRegister);
            Sys_Experience.Instance.eventEmitter.Handle(Sys_Experience.EEvents.OnUpdateExperienceInfo, OnUpdateExperienceInfo, toRegister);
            Sys_Experience.Instance.eventEmitter.Handle(Sys_Experience.EEvents.OnExperienceUpgrade, OnExperienceUpgrade, toRegister);
            Sys_Experience.Instance.eventEmitter.Handle<uint,bool>(Sys_Experience.EEvents.OnAddRankInPreAdd, OnAddRankInPreAdd, toRegister);

            Sys_Experience.Instance.eventEmitter.Handle(Sys_Experience.EEvents.OnAddNewPlan, OnAddNewPlan, toRegister);
            Sys_Experience.Instance.eventEmitter.Handle(Sys_Experience.EEvents.OnChangePlan, OnChangePlan, toRegister);
            Sys_Experience.Instance.eventEmitter.Handle<int>(Sys_Experience.EEvents.OnChangePlanName, OnChangePlanName, toRegister);
        }

        void UpdateUseButton()
        {
            if (Sys_Experience.Instance.currentIndex == currentChooseIndex)
            {
                layout.changePlanButton.enabled = false;
                layout.changePlanSelect.SetActive(true);
                layout.changePlanUnSelect.SetActive(false);
                ImageHelper.SetImageGray(layout.changePlanButton.GetComponent<Image>(), true);
            }
            else
            {
                layout.changePlanButton.enabled = true;
                layout.changePlanSelect.SetActive(false);
                layout.changePlanUnSelect.SetActive(true);
                ImageHelper.SetImageGray(layout.changePlanButton.GetComponent<Image>(), false);
            }
        }

        void OnAddNewPlan()
        {
            UpdatePlans();
            planItems[currentChooseIndex].toggle.SetSelected(true, true);
            OnUpdateExperienceInfo();
        }

        void OnChangePlan()
        {
            UpdateUseButton();
        }

        void OnChangePlanName(int index)
        {
            planItems[index].SetName(Sys_Experience.Instance.experiencePlanDatas[index].Name);
        }

        void UpdatePlans()
        {
            planItems.Clear();
            layout.planRoot.DestoryAllChildren();

            for (int index = 0, len = Sys_Experience.Instance.experiencePlanDatas.Count; index < len; index++)
            {
                GameObject planItemObj = GameObject.Instantiate(layout.planItemPrefab);
                UI_Family_Empowermen_Layout.PlanItem planItem = new UI_Family_Empowermen_Layout.PlanItem(planItemObj, Sys_Experience.Instance.experiencePlanDatas[index], index, (index2) =>
                {
                    currentChooseIndex = index2;
                    //UpdatePlans();
                    OnUpdateExperienceInfo();
                    UpdateUseButton();
                });
                planItemObj.transform.SetParent(layout.planRoot.transform, false);
                planItems.Add(planItem);
            }

            GameObject planAddObj = GameObject.Instantiate(layout.planAddPrefab);
            UI_Family_Empowermen_Layout.PlanAdd planAdd = new UI_Family_Empowermen_Layout.PlanAdd(planAddObj);
            planAddObj.transform.SetParent(layout.planRoot.transform, false);
        }

        private void OnAddRankInPreAdd(uint nextRank,bool isOpen)
        {
           for(int i=0;i<attrList.Count;++i)
            {
                if (attrList[i].rank == nextRank)
                {
                    attrList[i].RefreshItem(!isOpen);
                    attrList[i].SetAddState(isOpen ,!isOpen);
                }
            }
        }

        private void OnExperienceUpgrade()
        {
            SetValue();
            AddPointShow(false);
        }

        private void OnUpdateLeftPoint(uint point)
        {
            uint totalAddpoint = 0;
            foreach(var v in Sys_Experience.Instance.addinfoDic)
            {
                totalAddpoint += v.Value.Point;
            }
            layout.usedPoint.text =( Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].UsePoint+ totalAddpoint).ToString();
            layout.leftPoint.text = (Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].LeftPoint- totalAddpoint).ToString();
        }

        private void OnUpdateExperienceInfo()
        {
            SetValue();
            for(int i=0;i< attrList.Count;++i) { attrList[i].RefreshItem(attrList[i].IsLocked(attrList[i].rank)); }
            AddPointShow(false);
        }

        private void SetValue()
        {
            layout.level.text = Sys_Experience.Instance.exPerienceLevel.ToString();
            layout.usedPoint.text = Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].UsePoint.ToString();
            layout.leftPoint.text = Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].LeftPoint.ToString();
        }

        private void AddList()
        {
            Sys_Experience.Instance.index2Rank.Clear();
            foreach (var item in rankAttrDic)
            {
                SetPerAttrItem(item.Key);
            }
        }

        private void SetRankDic()
        {
            rankAttrDic.Clear();
            foreach (var item in Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].infoDic)
            {
                uint rank = CSVExperienceAttr.Instance.GetConfData(item.Key).rank;
                if (rankAttrDic.ContainsKey(rank))
                {
                    rankAttrDic[rank]++;
                }
                else
                {
                    rankAttrDic.Add(rank, 1);
                }
            }
        } 

        private void SetAttrGrids()
        {
            rankGoDic.Clear();
            Dictionary<uint, int> dic = rankAttrDic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            foreach (var item in dic)
            {
                if (item.Key != 1)
                {
                    GameObject lockGo = GameObject.Instantiate<GameObject>(layout.lockGo, layout.lockGo.transform.parent);
                    uint point = CSVExperienceAttrRank.Instance.GetConfData(item.Key).need_point;
                    lockGo.transform.Find("Text_Title").GetComponent<Text>().text = LanguageHelper.GetTextContent(2021206, point.ToString());
                    GameObject attrGrid = GameObject.Instantiate<GameObject>(layout.attrGird, layout.attrGird.transform.parent);
                    GameObject attr = attrGrid.transform.Find("Attr01").gameObject;
                    rankGoDic.Add(item.Key, attr);
                }
                else
                {
                    rankGoDic.Add(item.Key, layout.attrItemGo);
                }
            }
            layout.lockGo.SetActive(false);
        }

        private void SetPerAttrItem(uint rank)
        {
            foreach (var item in Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].infoDic)
            {
                uint curRank = CSVExperienceAttr.Instance.GetConfData(item.Key).rank;
                if (curRank == rank)
                {
                    GameObject attrGo = GameObject.Instantiate<GameObject>(rankGoDic[rank], rankGoDic[rank].transform.parent);
                    uint attrId= CSVExperienceAttr.Instance.GetConfData(item.Key).attr_id;
                    UI_Family_Empowerment_Attr attr = new UI_Family_Empowerment_Attr(attrId,item.Key,curRank);
                    attr.Init(attrGo.transform);
                    Sys_Experience.Instance.index2Rank.Add(item.Key, curRank);
                    attr.RefreshItem(attr.IsLocked(curRank));
                    attrGo.SetActive(true);
                    attrList.Add(attr);
                }
            }
            rankGoDic[rank].SetActive(false);
            ForceRebuildLayout(rankGoDic[rank].transform.parent.gameObject);
        }

        private void DefaultAttr()
        {
            layout.lockGo.SetActive(true);
            layout.attrGird.SetActive(true);
            for (int i = 0; i < attrList.Count; ++i)
            { attrList[i].OnDestroy(); }
            attrList.Clear();
            rankGoDic.Clear();
            rankAttrDic.Clear();
            FrameworkTool.DestroyChildren(layout.attrGird.transform.parent.gameObject, layout.attrGird.transform.name, layout.lockGo.transform.name,layout.tipsGo.transform.name);
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

        private void AddPointShow(bool isAdd)
        {
            Sys_Experience.Instance.addinfoDic.Clear();
            for (int i = 0; i < attrList.Count; ++i)
            {
                attrList[i].SetAddState(isAdd, attrList[i].IsLocked(attrList[i].rank));
            }
            layout.cancelBtn.gameObject.SetActive(isAdd);
            layout.confirmBtn.gameObject.SetActive(isAdd);
            layout.addPointBtn.gameObject.SetActive(!isAdd);
            layout.tipsGo.gameObject.SetActive(isAdd);

            layout.changePlanButton.gameObject.SetActive(!isAdd);
        }

        #region ButtonClicked

        public void OnAddPointBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_Family_Empowerment, "OnAddPointBtnClicked");
            if (Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].LeftPoint == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5603));
                return;
            }
            AddPointShow(true);
        }

        public void OnCancelBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_Family_Empowerment, "OnCancelBtnClicked");
            AddPointShow(false);
            layout.usedPoint.text = Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].UsePoint.ToString();
            layout.leftPoint.text = Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].LeftPoint.ToString();
        }

        public void OnCloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_Family_Empowerment, "OnCloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_Family_Empowerment);
        }

        public void OnConfirmBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_Family_Empowerment, "OnConfirmBtnClicked");
            Sys_Experience.Instance.AttrAddReq(Sys_Experience.Instance.addinfoDic, (uint)currentChooseIndex);
            AddPointShow(false);
        }

        public void OnResetBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_Family_Empowerment, "OnResetBtnClicked");
            AddPointShow(false);
            if (Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].UsePoint == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(104308));
                layout.usedPoint.text = Sys_Experience.Instance.experiencePlanDatas[currentChooseIndex].UsePoint.ToString();
                return;
            }
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(5601);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                if (Sys_Bag.Instance.GetItemCount(2) < 5000)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5602));
                }
                else
                {
                    Sys_Experience.Instance.ResetReq((uint)currentChooseIndex);
                }
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnUseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_Family_Empowerment, "OnUseBtnClicked");
            if (Sys_Family.Instance.familyData.isInFamily)
            {
                Sys_Family.Instance.SendGuildGetGuildInfoReq();
            }
            UIManager.OpenUI(EUIID.UI_Family_DeedsLv_Popup);
        }

        public void OnHelpBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_Family_Empowerment, "OnHelpBtnClicked");
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2021220) });
        }

        public void OnClickChangePlanButton()
        {
            Sys_Experience.Instance.ReqChangePlan((uint)currentChooseIndex);
        }

        #endregion
    }
}
