using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Adventure_Info : UIComponent
    {
        public EAdventurePageType PageType { get; } = EAdventurePageType.Info;
        #region 界面组件
        private Text txtUserName;
        private Text txtLevel;
        private Slider sliderExp;
        private Text txtExp;
        private Image imgIcon;
        private Image imgHeadIcon;
        private Text txtMedalName;
        private Button btnInfo;
        private Button btnLeft;
        private Button btnRight;

        private GameObject progressCell;

        private Text txtTitleLevel;
        private GameObject attrNormalCell;
        private GameObject attrSpecialCell;
        private GameObject scrollCell;
        private RectTransform content;
        private GridLayoutGroup group;
        private UICenterOnChild uiCenterOnChild;
        #endregion
        private uint level;
        private List<uint> levelIds = new List<uint>();
        private uint SelectLevel = 1;
        private Dictionary<uint, UI_ScrollCell> dictScrollCell = new Dictionary<uint, UI_ScrollCell>();

        #region 系统函数
        protected override void Loaded()
        {
            base.Loaded();
            InitData();
            Parse();
        }

        public override void Show()
        {
            base.Show();
            UpdateView();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Adventure.Instance.eventEmitter.Handle(Sys_Adventure.EEvents.OnLevelInfoUpdate, OnLevelInfoUpdate, toRegister);
        }
        public override void OnDestroy()
        {
            dictScrollCell.Clear();
            base.OnDestroy();
        }
        #endregion

        #region function
        private void Parse()
        {
            Transform viewInfo = transform.Find("View_Info");
            txtUserName = viewInfo.Find("Text_Name").GetComponent<Text>();
            txtLevel = viewInfo.Find("Text_Level/Value").GetComponent<Text>();
            sliderExp = viewInfo.Find("Slider_Exp").GetComponent<Slider>();
            txtExp = viewInfo.Find("Text_Percent").GetComponent<Text>();
            imgIcon = viewInfo.Find("Icon").GetComponent<Image>();
            imgHeadIcon = viewInfo.Find("Head").GetComponent<Image>();
            txtMedalName = viewInfo.Find("Title_bg/Text").GetComponent<Text>();
            btnInfo = viewInfo.Find("Btn_details").GetComponent<Button>();
            btnInfo.onClick.AddListener(OnBtnInfoClick);

            progressCell = transform.Find("View_Progress/Scroll View/Viewport/Content/Item").gameObject;
            progressCell.SetActive(false);

            Transform viewAttr = transform.Find("View_Right");
            txtTitleLevel = viewAttr.Find("Title/Text_Level").GetComponent<Text>();
            attrNormalCell = viewAttr.Find("Attr_Group/Attr_Grid/Viewport/Content/Attr").gameObject;
            attrNormalCell.SetActive(false);
            attrSpecialCell = viewAttr.Find("Privilege_Group/Attr_Grid/Viewport/Content/Attr").gameObject;
            attrSpecialCell.SetActive(false);
            scrollCell = viewAttr.Find("Scroll View01/Viewport/Content/Toggle").gameObject;
            scrollCell.SetActive(false);
            content = viewAttr.Find("Scroll View01/Viewport/Content").GetComponent<RectTransform>();
            group = viewAttr.Find("Scroll View01/Viewport/Content").GetComponent<GridLayoutGroup>();
            uiCenterOnChild = viewAttr.Find("Scroll View01").gameObject.GetNeedComponent<UICenterOnChild>();
            uiCenterOnChild.onCenter = OnCenter;
            btnLeft = viewAttr.Find("Scroll View01/Btn_PageLeft").GetComponent<Button>();
            btnLeft.onClick.AddListener(OnBtnLeftClick);
            btnRight = viewAttr.Find("Scroll View01/Btn_PageRight").GetComponent<Button>();
            btnRight.onClick.AddListener(OnBtnRightClick);
        }
        private void InitData()
        {
            //levelIds.AddRange(CSVAdventureLevel.Instance.GetDictData().Keys);
            levelIds.AddRange(CSVAdventureLevel.Instance.GetKeys());
        }
        public void UpdateView()
        {
            Sys_Adventure.Instance.AdventureGetInfoReq();
            UpdateInfoView();
            UpdateProgressView();
            UpdateAttrView(level);
            UpdateScrollView();
        }
        private void UpdateInfoView()
        {
            txtUserName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
            CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId);
            if (heroData != null && imgHeadIcon != null)
            {
                ImageHelper.SetIcon(imgHeadIcon, heroData.headid);
            }
            level = Sys_Adventure.Instance.Level;
            uint exp = Sys_Adventure.Instance.Exp;
            txtLevel.text = LanguageHelper.GetTextContent(2009305, level.ToString());
            CSVAdventureLevel.Data data = CSVAdventureLevel.Instance.GetConfData(level);
            CSVAdventureLevel.Data nextData = CSVAdventureLevel.Instance.GetConfData(level + 1);
            if (data != null)
            {
                uint nextExp = 0;
                if (nextData != null)
                {
                    nextExp = nextData.exp;
                    txtExp.text = LanguageHelper.GetTextContent(2009377, exp.ToString(), nextExp.ToString());
                    txtExp.gameObject.SetActive(true);
                    sliderExp.gameObject.SetActive(true);
                }
                else
                {
                    txtExp.gameObject.SetActive(false);
                    sliderExp.gameObject.SetActive(false);
                }
                float value = 1;
                if (exp <= 0 && nextExp > 0)
                {
                    value = 0;
                }
                else if (exp > nextExp || nextExp == 0)
                {
                    value = 1;
                }
                else
                {
                    value = (float)exp / (float)nextExp;
                }
                sliderExp.value = value;
                if (imgIcon != null)
                {
                    ImageHelper.SetIcon(imgIcon, data.icon);
                }
                txtMedalName.text = LanguageHelper.GetTextContent(data.name);
            }
            else
            {
                txtExp.text = LanguageHelper.GetTextContent(2009377, "0", "0");
            }
        }
        private void UpdateProgressView()   
        {
            //冒险进度
            FrameworkTool.DestroyChildren(progressCell.transform.parent.gameObject, progressCell.name);
            Dictionary<EAdventureProgressType, List<uint>> dic = Sys_Adventure.Instance.GetProgressInfoDict();
            List<uint> proList = Sys_Adventure.Instance.ListProIds;
            int len = proList.Count;
            for (int i = 0; i < len; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(progressCell, progressCell.transform.parent);
                go.SetActive(true);
                UI_ProgressCell cell = new UI_ProgressCell(proList[i]);
                cell.Init(go.transform);
                cell.UpdateCellView(dic[(EAdventureProgressType)proList[i]]);
            }
        }
        private void UpdateAttrView(uint _level)
        {
            uint level = _level > 0 ? _level : 1;
            SelectLevel = level;
            txtTitleLevel.text = LanguageHelper.GetTextContent(2009305, level.ToString());
            CSVAdventureLevel.Data data = CSVAdventureLevel.Instance.GetConfData(level);
            if(data != null)
            {
                UpdateNormalAttrView(data.addAttribute);
                UpdateSpecialAttrView(data);
            }
        }
        private void UpdateNormalAttrView(List<List<uint>> attrs)
        {
            FrameworkTool.DestroyChildren(attrNormalCell.transform.parent.gameObject, attrNormalCell.name);
            int len = attrs.Count;
            for (int i = 0; i < len; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(attrNormalCell, attrNormalCell.transform.parent);
                go.SetActive(true);
                List<uint> attrData = attrs[i];
                UI_NormalAttr normalAttr = new UI_NormalAttr(attrData[0], attrData[1]);
                normalAttr.Init(go.transform);
                normalAttr.UpdateCellView();
            }
        }
        private void UpdateSpecialAttrView(CSVAdventureLevel.Data data)
        {
            bool hasCell = false;
            FrameworkTool.DestroyChildren(attrSpecialCell.transform.parent.gameObject, attrSpecialCell.name);
            if (data.addPrivilegeAttribute != null)
            {
                List<List<uint>> attrs = data.addPrivilegeAttribute;
                int len = attrs.Count;
                for (int i = 0; i < len; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(attrSpecialCell, attrSpecialCell.transform.parent);
                    go.SetActive(true);
                    List<uint> attrData = attrs[i];
                    UI_SpecialAttr specialAttr = new UI_SpecialAttr(attrData[0], attrData[1]);
                    specialAttr.Init(go.transform);
                    specialAttr.UpdateCellView();
                    hasCell = true;
                }
            }
            if (data.addPrivilege != null)
            {
                List<List<uint>> attrs = data.addPrivilege;
                int len = attrs.Count;
                for (int i = 0; i < len; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(attrSpecialCell, attrSpecialCell.transform.parent);
                    go.SetActive(true);
                    List<uint> attrData = attrs[i];
                    UI_SpecialAttr specialAttr = new UI_SpecialAttr(attrData[0], attrData[1], data.privilegeText[i]);
                    specialAttr.Init(go.transform);
                    specialAttr.UpdatePrivilegeCellView();
                    hasCell = true;
                }
            }
            if (!hasCell)
            {
                GameObject go = GameObject.Instantiate<GameObject>(attrSpecialCell, attrSpecialCell.transform.parent);
                go.SetActive(true);
                UI_SpecialAttr specialAttr = new UI_SpecialAttr(0, 0);
                specialAttr.Init(go.transform);
                specialAttr.UpdateDefaultCellView();
            }
        }
        private void UpdateScrollView()
        {
            dictScrollCell.Clear();
            FrameworkTool.DestroyChildren(scrollCell.transform.parent.gameObject, scrollCell.name);
            int len = levelIds.Count;
            for (int i = 0; i < len; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(scrollCell, scrollCell.transform.parent);
                go.SetActive(true);
                UI_ScrollCell cell = new UI_ScrollCell(levelIds[i]);
                cell.Init(go.transform);
                cell.Register(OnCellClick);
                dictScrollCell.Add(levelIds[i], cell);
                cell.UpdateCellView();
            }
            uint level = Sys_Adventure.Instance.Level > 0 ? Sys_Adventure.Instance.Level : 1;
            uiCenterOnChild.InitPageArray();
            dictScrollCell[level]?.SetSelected(true);
        }
        #endregion

        #region 响应事件
        private void OnBtnInfoClick()
        {
            uint level = Sys_Adventure.Instance.Level;
            CSVAdventureLevel.Data levelData = CSVAdventureLevel.Instance.GetConfData(level);
            string txt = LanguageHelper.GetTextContent(600000190, Sys_Role.Instance.Role.Name.ToStringUtf8(), LanguageHelper.GetTextContent(levelData.name));
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { TitlelanId = 2022222, StrContent = txt });
            Sys_Adventure.Instance.ReportClickEventHitPoint("Info_Help");
        }

        private void OnLevelInfoUpdate()
        {
            UpdateInfoView();
        }
        private void OnCellClick(uint levelId)
        {
            UpdateAttrView(levelId);
            uiCenterOnChild.SetCurrentPageIndex((int)levelId - 1, true);
            Sys_Adventure.Instance.ReportClickEventHitPoint("Info_Attr_levelId:" + levelId.ToString());
        }
        /// <summary> 居中事件 </summary>
        private void OnCenter(GameObject go)
        {
            
        }
        private void OnBtnLeftClick()
        {
            if (SelectLevel > levelIds[0])
            {
                uint levelId = SelectLevel - 1;
                UpdateAttrView(levelId);
                uiCenterOnChild.SetCurrentPageIndex((int)levelId - 1, true);
                dictScrollCell[levelId]?.SetSelected(true);
            }
        }
        private void OnBtnRightClick()
        {
            if (SelectLevel < levelIds[levelIds.Count - 1])
            {
                uint levelId = SelectLevel + 1;
                UpdateAttrView(levelId);
                uiCenterOnChild.SetCurrentPageIndex((int)levelId - 1, true);
                dictScrollCell[levelId]?.SetSelected(true);
            }
        }
        #endregion

        //冒险进度cell类
        public class UI_ProgressCell : UIComponent
        {
            private uint proId;

            private Image imgIcon;
            private Text txtName;
            private Text txtPro;
            public UI_ProgressCell(uint id) : base()
            {
                proId = id;
            }

            protected override void Loaded()
            {
                base.Loaded();
                imgIcon = transform.Find("Icon").GetComponent<Image>();
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtPro = transform.Find("Text_Progress").GetComponent<Text>();
            }

            public void UpdateCellView(List<uint> listPro)
            {
                CSVAdventureProgress.Data data = CSVAdventureProgress.Instance.GetConfData(proId);
                ImageHelper.SetIcon(imgIcon, data.icon);
                txtName.text = LanguageHelper.GetTextContent(data.name);
                txtPro.text = LanguageHelper.GetTextContent(2009377, listPro[0].ToString(), listPro[1].ToString());
            }
        }

        //普通属性cell类
        public class UI_NormalAttr : UIComponent
        {
            private uint attrId;
            private uint attrValue;
            private Text txtProperty;
            private Text txtNum;

            public UI_NormalAttr(uint id, uint value) : base()
            {
                attrId = id;
                attrValue = value;
            }
            protected override void Loaded()
            {
                txtProperty = transform.GetComponent<Text>();
                txtNum = transform.Find("Text_Number").GetComponent<Text>();
            }

            public void UpdateCellView()
            {
                txtProperty.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(attrId).name);
                txtNum.text = attrValue.ToString();
            }
        }
        //特权属性cell类
        public class UI_SpecialAttr : UIComponent
        {
            private uint attrId;
            private uint attrValue;
            private uint attrDesc;
            private Text txtProperty;

            public UI_SpecialAttr(uint id, uint value, uint desc = 0) : base()
            {
                attrId = id;
                attrValue = value;
                attrDesc = desc;
            }
            protected override void Loaded()
            {
                txtProperty = transform.GetComponent<Text>();
            }

            public void UpdateCellView()
            {
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(attrId);
                string str;
                if (data.show_type == 1)
                {
                    str = attrValue.ToString();
                }
                else
                {
                    str = ((float)attrValue / 100f) + "%";
                }
                txtProperty.text = LanguageHelper.GetTextContent(data.name) + str;
            }

            public void UpdatePrivilegeCellView()
            {
                txtProperty.text = LanguageHelper.GetTextContent(attrDesc, attrValue.ToString());
            }

            public void UpdateDefaultCellView()
            {
                txtProperty.text = LanguageHelper.GetTextContent(600000099);
            }
        }
        //scrollCell
        public class UI_ScrollCell : UIComponent
        {
            private uint levelId;

            private Text txtLevel;
            private CP_Toggle toggle;
            private System.Action<uint> _action;

            public UI_ScrollCell(uint id)
            {
                levelId = id;
            }
            protected override void Loaded()
            {
                base.Loaded();
                txtLevel = transform.Find("Text").GetComponent<Text>();
                toggle = transform.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
            }
            public void UpdateCellView()
            {
                txtLevel.text = levelId.ToString();
            }
            public void Register(System.Action<uint> action)
            {
                _action = action;
            }
            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke(levelId);
                }
            }
            public void SetSelected(bool isOn)
            {
                toggle.SetSelected(isOn, true);
            }
        }
    }
}

